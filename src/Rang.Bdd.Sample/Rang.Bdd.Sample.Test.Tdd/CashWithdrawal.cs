using Rang.Bdd.Sample.Domain.Implementation;
using Rang.Bdd.Sample.Domain.Interface;

namespace Rang.Bdd.Sample.Test.Tdd;

public class CashWithdrawal
{
    [Fact]
    public void Bank_CreateAccount_WithInitialAmountOf1000_Success()
    {
        // arrange
        decimal initialAmount= 1000;

        // act
        var account = Bank.CreateAccount(initialAmount);

        // assert
        Assert.NotNull(account);
        Assert.Equal(initialAmount, account.GetCurrentFunds());
    }

    [Fact]
    public void Bank_CreateAtmCard_WithExistingAccountAndValidExpirationDate_Success()
    {
        // arrange
        decimal initialAmount= 1000;
        var validExpirationDateForAtmCard= Bank.GetValidExpirationDateTimeForAtmCard();
        var account = Bank.CreateAccount(initialAmount);

        // act
        var atmCard = Bank.CreateAtmCard(account, validExpirationDateForAtmCard);

        // assert
        Assert.NotNull(atmCard);
        Assert.True(atmCard.IsValid());
    }

    [Fact]
    public void Bank_CreateAtmCard_WithExistingAccountAndInvalidExpirationDate_Success()
    {
        // arrange
        decimal initialAmount= 1000;
        var validExpirationDateForAtmCard = Bank.GetInvalidExpirationDateTimeForAtmCard();
        var account = Bank.CreateAccount(initialAmount);

        // act
        var atmCard = Bank.CreateAtmCard(account, validExpirationDateForAtmCard);

        // assert
        Assert.NotNull(atmCard);
        Assert.False(atmCard.IsValid());
    }

    [Fact]
    public void Bank_CreateAtm_WithInitialAmountOf8500_Success()
    {
        // arrange
        decimal initialAmount= 8500;

        // act
        var atm = Bank.CreateAtm(initialAmount);

        // assert
        Assert.NotNull(atm);
        Assert.Equal(initialAmount, atm.GetCurrentCashAmount());
    }

    [Fact]
    public void Atm_HasCardInSlot_WithNoCardInSlot_False()
    {
        // arrange
        decimal initialAmount = 8500;
        var atm = Bank.CreateAtm(initialAmount);

        // act
        var result = atm.HasCardInSlot();

        // assert
        Assert.False(result);
    }

    [Fact]
    public void Atm_InsertCard_Success()
    {
        // arrange
        decimal initialAccountAmount = 1000;
        var account = Bank.CreateAccount(initialAccountAmount);
        var validExpirationDateForAtmCard = Bank.GetValidExpirationDateTimeForAtmCard();
        var atmCard = Bank.CreateAtmCard(account, validExpirationDateForAtmCard);
        decimal initialAtmAmount = 8500;
        var atm = Bank.CreateAtm(initialAtmAmount);
        
        // act
        atm.InsertCard(atmCard);

        // assert
        Assert.True(atm.HasCardInSlot());
    }

    [Fact]
    public void Atm_HasCardInSlot_WithCardInSlot_True()
    {
        // arrange
        decimal initialAccountAmount = 1000;
        var account = Bank.CreateAccount(initialAccountAmount);
        var validExpirationDateForAtmCard = Bank.GetValidExpirationDateTimeForAtmCard();
        var atmCard = Bank.CreateAtmCard(account, validExpirationDateForAtmCard);
        decimal initialAtmAmount = 8500;
        var atm = Bank.CreateAtm(initialAtmAmount);
        atm.InsertCard(atmCard);

        // act
        var result = atm.HasCardInSlot();

        // assert
        Assert.True(result);
    }

    [Fact]
    public void Atm_Withdraw500Cash_WithAccountWithEnoughFundsAndValidCardAndAtmWithEnoughCashButNoCardInSlot_CardIsNotInSlot()
    {
        // arrange
        decimal initialAccountAmount = 1000;
        var account = Bank.CreateAccount(initialAccountAmount);
        var validExpirationDateForAtmCard = Bank.GetValidExpirationDateTimeForAtmCard();
        var atmCard = Bank.CreateAtmCard(account, validExpirationDateForAtmCard);
        decimal initialAtmAmount = 8500;
        var atm = Bank.CreateAtm(initialAtmAmount);
        decimal amountToWithdraw = 500;

        // act 
        var withdrawResult = atm.WithdrawCash(amountToWithdraw);

        // assert
        Assert.Equal(AtmOperationResultStatus.CardIsNotInSlot, withdrawResult.GetStatus());
        //  account state:
        Assert.Equal(1000, account.GetCurrentFunds());
        //  atm state:
        Assert.False(atm.HasCardInSlot());
        Assert.Equal(8500, atm.GetCurrentCashAmount());
    }

    [Fact]
    public void Atm_Withdraw500Cash_WithAccountWithEnoughFundsAndInvalidCardAndAtmWithEnoughCash_InvalidCard()
    {
        // arrange
        decimal initialAccountAmount = 1000;
        var account = Bank.CreateAccount(initialAccountAmount);
        var invalidExpirationDateForAtmCard = Bank.GetInvalidExpirationDateTimeForAtmCard();
        var atmCard = Bank.CreateAtmCard(account, invalidExpirationDateForAtmCard);
        decimal initialAtmAmount = 8500;
        var atm = Bank.CreateAtm(initialAtmAmount);
        atm.InsertCard(atmCard);
        decimal amountToWithdraw = 500;

        // act 
        var withdrawResult = atm.WithdrawCash(amountToWithdraw);

        // assert
        Assert.Equal(AtmOperationResultStatus.InvalidCard, withdrawResult.GetStatus());
        //  account state:
        Assert.Equal(1000, account.GetCurrentFunds());
        //  atm state:
        Assert.True(atm.HasCardInSlot());
        Assert.Equal(8500, atm.GetCurrentCashAmount());
    }

    [Fact]
    public void Atm_Withdraw500Cash_WithAccountWithNotEnoughFundsAndValidCardAndAtmWithEnoughCash_NotEnoughFundsInAccount()
    {
        // arrange
        decimal initialAccountAmount = 100;
        var account = Bank.CreateAccount(initialAccountAmount);
        var validExpirationDateForAtmCard = Bank.GetValidExpirationDateTimeForAtmCard();
        var atmCard = Bank.CreateAtmCard(account, validExpirationDateForAtmCard);
        decimal initialAtmAmount = 8500;
        var atm = Bank.CreateAtm(initialAtmAmount);
        atm.InsertCard(atmCard);
        decimal amountToWithdraw = 500;

        // act 
        var withdrawResult = atm.WithdrawCash(amountToWithdraw);

        // assert
        Assert.Equal(AtmOperationResultStatus.NotEnoughFundsInAccount, withdrawResult.GetStatus());
        //  account state:
        Assert.Equal(100, account.GetCurrentFunds());
        //  atm state:
        Assert.True(atm.HasCardInSlot());
        Assert.Equal(8500, atm.GetCurrentCashAmount());
    }

    [Fact]
    public void Atm_Withdraw500Cash_WithAccountWithEnoughFundsAndValidCardAndAtmWithEnoughCash_Success()
    {
        // arrange
        decimal initialAccountAmount = 1000;
        var account = Bank.CreateAccount(initialAccountAmount);
        var validExpirationDateForAtmCard = Bank.GetValidExpirationDateTimeForAtmCard();
        var atmCard = Bank.CreateAtmCard(account, validExpirationDateForAtmCard);
        decimal initialAtmAmount = 8500;
        var atm = Bank.CreateAtm(initialAtmAmount);
        atm.InsertCard(atmCard);
        decimal amountToWithdraw = 500;

        // act 
        var withdrawResult = atm.WithdrawCash(amountToWithdraw);

        // assert
        Assert.Equal(AtmOperationResultStatus.WithdrawalSuccessful, withdrawResult.GetStatus());
        //  account state:
        Assert.Equal(500, account.GetCurrentFunds());
        //  atm state:
        Assert.True(atm.HasCardInSlot());
        Assert.Equal(8000, atm.GetCurrentCashAmount());
    }

    
}