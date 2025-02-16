using Cashflow.Domain.Abstractions.DataAccess;
using Cashflow.Domain.Abstractions.RequestPipeline;
using Cashflow.Domain.Entities;
using Cashflow.Domain.Enums;
using Cashflow.Domain.Events.TransactionControl;
using Cashflow.Domain.Exceptions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cashflow.Domain.Features.TransactionControl.EventHandlers;

public class AlterTransactionRegisteredEventHandler : IConsumer<AlterTransactionRegisteredEvent>
{
    private readonly ILogger<AlterTransactionRegisteredEventHandler> _logger;
    private readonly ICashflowDbContext _dbContext;
    private readonly IAuthenticatedUser _authenticatedUser;
    
    public AlterTransactionRegisteredEventHandler(ILogger<AlterTransactionRegisteredEventHandler> logger,
                                                  ICashflowDbContext dbContext,
                                                  IAuthenticatedUser authenticatedUser)
    {
        _logger = logger;
        _dbContext = dbContext;
        _authenticatedUser = authenticatedUser;
    }                   
    
    public async Task Consume(ConsumeContext<AlterTransactionRegisteredEvent> context)
    {
        _authenticatedUser.BindMessageTrace(context.Message.TraceIdentifier, context.Message.UserId);
        
        _logger.LogInformation("Received a alter transaction to process for message with Trace {0}.", context.Message.TraceIdentifier);
        
        var message = context.Message;
        var transaction = new Transaction(message.Motive, message.DoneAt, TransactionMethodType.Alter, message.AlteredValue);

        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
        
        if (message.BankAccountId is not null)
        {
            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(b => b.Id == message.BankAccountId);
            if (bankAccount is null)
            {
                throw new EntityNotFoundException<BankAccount>();
            }
            
            transaction.DistributeTo(bankAccount);
        }

        if (message.CategoryId is not null)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == message.CategoryId);
            if (category is null)
            {
                throw new EntityNotFoundException<Category>();
            }
            
            transaction.UseFor(category);
        }

        await _dbContext.SaveChangesAsync();
    }
}
