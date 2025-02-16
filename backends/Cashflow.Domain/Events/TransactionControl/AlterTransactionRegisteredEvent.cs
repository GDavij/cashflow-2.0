using System.Text.Json.Serialization;
using Cashflow.Core.Events;
using Cashflow.Domain.Entities;

namespace Cashflow.Domain.Events.TransactionControl;

public class AlterTransactionRegisteredEvent : BaseEvent
{
    public long? BankAccountId { get; set; }
    public long? CategoryId { get; set; }
    public string Motive { get; set; }
    public DateTime DoneAt { get; set; }
    public decimal AlteredValue { get; set; }
    
    [JsonConstructor]
    public AlterTransactionRegisteredEvent() : base(true)
    { }

    private AlterTransactionRegisteredEvent(
        long? bankAccountId,
        long? categoryId,
        decimal alteredValue,
        string motive,
        DateTime doneAt) : base(true)
    {
        BankAccountId = bankAccountId;
        CategoryId = categoryId;
        AlteredValue = alteredValue;
        Motive = motive;
        DoneAt = doneAt;
    }

    public static AlterTransactionRegisteredEvent ForBankAccount(BankAccount bankAccount, decimal value, string motive, DateTime doneAt)
    {
        return new AlterTransactionRegisteredEvent(bankAccount.Id, null, value, motive, doneAt);
    }
    
    public override string Description()
    {
        if (BankAccountId is not null)
        {
            return $"Registering an Alter Transaction for bank account with Id {BankAccountId} with a value of {AlteredValue:N} for user with Id {UserId}.";
        }

        return $"Registering an Alter Transaction with a value of {AlteredValue:N} for user with Id {UserId}.";
    }
}