namespace Rang.Bdd.Sample.Domain.Interface;

// account
public interface IAccount
{
    public decimal GetCurrentFunds();
    public bool HasFundsToWithdraw(decimal amount);
    public void AddDebitTransaction(decimal amount);
}

public enum AccountTransactionTypes
{
    Debit,
    Credit
}

public interface IAccountTransaction
{
    AccountTransactionTypes Type { get; }
    public decimal Amount { get; }
}

// card
public interface IAtmCard
{
    public bool IsValid();
    public IAccount GetAccount();
}


// ATM
public interface IAtm
{
    public bool HasCardInSlot();
    public decimal GetCurrentCashAmount();
    public void InsertCard(IAtmCard atmCard);
    public void EjectCard();
    public IAtmOperationResult WithdrawCash(decimal amountToWithdraw);
}

public interface IAtmOperationResult
{
    public AtmOperationResultStatus GetStatus(); 
}

public enum AtmOperationResultStatus
{
    WithdrawalSuccessful,
    CardIsNotInSlot,
    InvalidCard,
    NotEnoughFundsInAccount,
    UnableToDispenseRequestedAmount
}