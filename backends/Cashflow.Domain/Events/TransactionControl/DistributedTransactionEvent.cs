using Cashflow.Core.Events;
using Cashflow.Domain.Entities;

namespace Cashflow.Domain.Events.TransactionControl;

public class DistributedTransactionEvent : BaseEvent
{
    public long? BankAccountId { get; set; }
    public long? CategoryId { get; set; }
    public long TransactionId { get; set; }
    
    private DistributedTransactionEvent(long? bankAccountId, long? categoryId, long transactionId) : base(true)
    {
        BankAccountId = bankAccountId;
        CategoryId = categoryId;
        TransactionId = transactionId;
    }
    
    public static DistributedTransactionEvent ForBankAccount(BankAccount bankAccount, Transaction transaction)
    {
        return new DistributedTransactionEvent(bankAccount.Id, null, transaction.Id);
    }

    public override string Description()
    {
        if (BankAccountId is not null)
        {
            return $"Distributed Transaction with Id {TransactionId} to Bank Account with Id {BankAccountId} for User with Id {UserId}.";
        }
     
        return $"Distributed Transaction with Id {TransactionId} to Category with Id {CategoryId} for User with Id {UserId}.";
    }
}