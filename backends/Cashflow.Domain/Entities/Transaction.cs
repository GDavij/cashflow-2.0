using Cashflow.Core;
using Cashflow.Domain.Enums;
using Cashflow.Domain.Events.TransactionControl;

namespace Cashflow.Domain.Entities;

public class Transaction : OwnableEntity<Transaction>
{
    public long? BankAccountId { get; private set; }
    public BankAccount? BankAccount { get; private set; }
    public long? CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public string Description { get; private set; }
    public DateTime DoneAt { get; private set; }
    public short Month { get; private set; }
    public short TransactionMethodId { get; private set; }
    public TransactionMethod TransactionMethod { get; private set; }
    public decimal Value { get; private set; }
    public int Year { get; private set; }

    public Transaction() 
    { }
    
    public Transaction(string description, DateTime doneAt, TransactionMethodType method, decimal value)
    {
        Description = description;
        DoneAt = doneAt;
        Year = doneAt.Year;
        Month = (short)doneAt.Month;
        TransactionMethodId = (short)method;
        Value = value;
    }

    public void DistributeTo(BankAccount bankAccount)
    {
        BankAccountId = bankAccount.Id;

        bankAccount.HaveCurrentFundsOf(Value);
        RaiseEvent(DistributedTransactionEvent.ForBankAccount(bankAccount, this));
    }

    public void UseFor(Category category)
    {
        Category = category;
    }
}