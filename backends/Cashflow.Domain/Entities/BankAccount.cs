using Cashflow.Core;
using Cashflow.Domain.Events.FinancialDistribution;
using Cashflow.Domain.Events.TransactionControl;

namespace Cashflow.Domain.Entities;

public class BankAccount : OwnableEntity<BankAccount>
{
    public short AccountTypeId { get; init; }
    public AccountType AccountType { get; init; }
    public decimal CurrentValue { get; private set; }
    public string Name { get; private set; }

    public ICollection<Transaction> Transactions { get; init; } = new List<Transaction>();

    public BankAccount() : base()
    { }
    
    public BankAccount(short accountTypeId, string name)
        : base()
    {
        AccountTypeId = accountTypeId;
        Name = name;
        CurrentValue = 0.0M;
    }

    public void RenameTo(string name)
    {
        if (name == Name)
        {
            return;
        }

        RaiseEvent(new BankAccountRenamedEvent(this, Name));
        Name = name;
    }

    public void AlterAccountValueTo(decimal value, DateTime doneAt)
    {
        RaiseEvent(AlterTransactionRegisteredEvent.ForBankAccount(this, value, "Bank Account Current Funds Registration", doneAt));
    }

    public void HaveCurrentFundsOf(decimal value)
    {
        CurrentValue = value;
    }
}