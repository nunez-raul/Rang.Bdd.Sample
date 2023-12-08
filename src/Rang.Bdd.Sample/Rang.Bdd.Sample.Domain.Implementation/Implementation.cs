using Rang.Bdd.Sample.Domain.Interface;
using System.Security.Principal;
using System.Transactions;

namespace Rang.Bdd.Sample.Domain.Implementation;

public static class Bank
{
    public static IAccount CreateAccount(decimal initialAmount)
    {
        return new Account(initialAmount);
    }

    public static DateTime GetValidExpirationDateTimeForAtmCard()
    {
        return DateTime.UtcNow.AddDays(1);
    }

    public static DateTime GetInvalidExpirationDateTimeForAtmCard()
    {
        return DateTime.UtcNow.AddDays(-1);
    }

    public static IAtmCard CreateAtmCard(IAccount account, DateTime expirationDateTime)
    {
        return new AtmCard(account, expirationDateTime);
    }

    public static IAtm CreateAtm(decimal initialAmount)
    {
        return new Atm(initialAmount);
    }
}

public class Account: IAccount
{
    private decimal _currentFunds;
    private List<IAccountTransaction> _transactions = new();

    public Account(decimal initialAmount)
    {
        _currentFunds = initialAmount;
        _transactions.Add(new AccountTransaction(AccountTransactionTypes.Credit, _currentFunds));
    }

    public decimal GetCurrentFunds()
    {
        return _currentFunds;
    }

    public bool HasFundsToWithdraw(decimal amount)
    {
        return _currentFunds - amount >= 0;
    }

    public void AddDebitTransaction(decimal amount)
    {
        if (!HasFundsToWithdraw(amount))
            throw new ApplicationException("insufcient funds.");

        _transactions.Add(new AccountTransaction(AccountTransactionTypes.Debit, amount));
        _currentFunds= _currentFunds - amount;

        // return _currentFunds;
    }
}

public class AccountTransaction : IAccountTransaction
{
    private AccountTransactionTypes _type;
    private decimal _amount;
    private DateTime _whenItHappened;

    public AccountTransactionTypes Type => _type;
    public decimal Amount => _amount;

    public AccountTransaction(AccountTransactionTypes type, decimal amount)
    {
        _type = type;
        _amount = amount;
        _whenItHappened = DateTime.UtcNow;
    }
}

public class AtmCard: IAtmCard
{
    private readonly IAccount _account;
    private readonly DateTime _expirationDateTime;

    public AtmCard(IAccount account, DateTime espirationDateTime)
    {
        _account = account;
        _expirationDateTime = espirationDateTime;
    }

    public bool IsValid()
    {
        return _expirationDateTime > DateTime.UtcNow;
    }

    public IAccount GetAccount()
    {
        return _account;
    }
}

public class Atm : IAtm
{
    private decimal _currentCashAmount;
    private IAtmCard? _atmCard;

    public Atm(decimal initialAmount) 
    {
        _currentCashAmount= initialAmount;
        _atmCard= null;
    }

    public bool HasCardInSlot()
    {
        return _atmCard != null;
    }

    public decimal GetCurrentCashAmount()
    {
        return _currentCashAmount;
    }

    public void InsertCard(IAtmCard atmCard)
    {
        _atmCard= atmCard;
    }

    public void EjectCard()
    {
        _atmCard= null;
    }

    public IAtmOperationResult WithdrawCash(decimal amountToWithdraw)
    {
        if(! HasCardInSlot())
            return new AtmWithdrawalResult(AtmOperationResultStatus.CardIsNotInSlot);
        
        if(! _atmCard.IsValid())
            return new AtmWithdrawalResult(AtmOperationResultStatus.InvalidCard);

        if (! _atmCard.GetAccount().HasFundsToWithdraw(amountToWithdraw))
            return new AtmWithdrawalResult(AtmOperationResultStatus.NotEnoughFundsInAccount);

        DispenseCash(amountToWithdraw);
        _atmCard.GetAccount().AddDebitTransaction(amountToWithdraw);
        
        return new AtmWithdrawalResult(AtmOperationResultStatus.WithdrawalSuccessful);
    }

    private void DispenseCash(decimal amount)
    {
        _currentCashAmount= _currentCashAmount - amount;
    }
}

public readonly record struct AtmWithdrawalResult : IAtmOperationResult
{
    private readonly AtmOperationResultStatus _status;

    public AtmWithdrawalResult(AtmOperationResultStatus operationStatus)
    {
        _status = operationStatus;
    }

    public AtmOperationResultStatus GetStatus()
    {
        return _status;
    }
}