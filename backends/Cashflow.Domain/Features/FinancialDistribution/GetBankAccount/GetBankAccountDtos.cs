namespace Cashflow.Domain.Features.FinancialDistribution.GetBankAccount;

public record AccountTypeDto(long Id, string Name);
public record TransactionDto(long Id, string Description, DateTime DoneAt, decimal Value, string TransactionMethod, CategoryDto? Category);
public record CategoryDto(long Id, string Name);
