using Cashflow.Domain.Abstractions.DataAccess;
using Cashflow.Domain.Abstractions.RequestPipeline;
using Cashflow.Domain.Entities;
using Cashflow.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cashflow.Domain.Features.FinancialDistribution.GetBankAccount;

public class GetBankAccountHandler
{
    private readonly ILogger<GetBankAccountHandler> _logger;
    private readonly ICashflowDbContext _dbContext;
    private readonly IAuthenticatedUser _authenticatedUser;

    public GetBankAccountHandler(ILogger<GetBankAccountHandler> logger, ICashflowDbContext dbContext, IAuthenticatedUser authenticatedUser)
    {
        _logger = logger;
        _dbContext = dbContext;
        _authenticatedUser = authenticatedUser;
    }

    public record Response
    {
        public long Id { get; init; }
        public AccountTypeDto Type { get; init; }
        public decimal CurrentValue { get; init; }
        public string Name { get; init; }
        public long TotalTransactionsRegistered { get; init; }
        public List<TransactionDto> LastTransactions { get; init; }
        public bool Active { get; init; }
    };

    public async Task<Response> HandleAsync(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attemping to get Bank account with Id {0} for user with id {1}", id,
            _authenticatedUser.Id);

        var result = await _dbContext.BankAccounts.Where(b => b.Id == id &&
                                                              b.OwnerId == _authenticatedUser.Id &&
                                                              !b.Deleted)
            .Select(b => new Response
            {
                Id = b.Id,
                Active = b.Active,
                Name = b.Name,
                Type = new AccountTypeDto(b.AccountType.Id, b.AccountType.Name),
                CurrentValue = b.CurrentValue
            }).FirstOrDefaultAsync(cancellationToken);
        
        if (result is null)
        {
            _logger.LogError("An attempt to get a non existent bank account was made.");
            throw new EntityNotFoundException<BankAccount>();
        }

        var transactionsQuery = from transactions in _dbContext.Transactions
                                                      join transactionMethod in _dbContext.TransactionMethods
                                                        on transactions.TransactionMethodId equals transactionMethod.Id
                                                      join category in _dbContext.Categories
                                                        on transactions.CategoryId equals category.Id into categoriesGroup
                                                      from category in categoriesGroup.DefaultIfEmpty()
                                                      where transactions.BankAccountId == id && !transactions.Deleted
                                                      orderby transactions.DoneAt descending
                                                     select new TransactionDto(transactions.Id,
                                                                               transactions.Description,
                                                                               transactions.DoneAt,
                                                                               transactions.Value,
                                                                               transactionMethod.Name,
                                                                               category != null ? new CategoryDto(category.Id, category.Name) : null);
        
        result = result with
        {
            TotalTransactionsRegistered = await transactionsQuery.CountAsync(cancellationToken),
            LastTransactions= await transactionsQuery.Skip(0).Take(10).ToListAsync(cancellationToken)
        };

        
        _logger.LogInformation("Got bank account with name {0}, having Id {1}.", result.Name, result.Id);
        return result;
    }
}