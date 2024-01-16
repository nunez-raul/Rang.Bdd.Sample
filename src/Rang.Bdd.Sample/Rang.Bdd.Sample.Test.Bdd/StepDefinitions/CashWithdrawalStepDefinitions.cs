using Rang.Bdd.Sample.Domain.Implementation;
using Rang.Bdd.Sample.Domain.Interface;

namespace Rang.Bdd.Sample.Test.Bdd.StepDefinitions
{
    [Binding]
    public class CashWithdrawalStepDefinitions
    {
        IAccount? _account;
        IAtmCard? _atmCard;
        IAtm? _atm;
        IAtmOperationResult? _atmOperationResult;

        [Given(@"that account holds (.*) money")]
        public void GivenThatAccountHoldsMoney(decimal initialAmount)
        {
            _account = Bank.CreateAccount(initialAmount);
        }

        [Given(@"the ATM card has a valid expiration date")]
        public void GivenTheATMCardHasAValidExpirationDate()
        {
            _atmCard = Bank.CreateAtmCard(_account, Bank.GetValidExpirationDateTimeForAtmCard());
        }

        [Given(@"the ATM has (.*) cash")]
        public void GivenTheATMHasCash(decimal initialAmount)
        {
            _atm = Bank.CreateAtm(initialAmount);
        }

        [Given(@"the ATM card has been inserted into the ATM")]
        public void GivenTheATMCardHasBeenInsertedIntoTheATM()
        {
            _atm.InsertCard(_atmCard);
        }

        [When(@"the user request (.*) cash")]
        public void WhenTheUserRequestCash(decimal amountToWithdraw)
        {
            _atmOperationResult = _atm.WithdrawCash(amountToWithdraw);
        }

        [Then(@"ensure the account is debited (.*)")]
        public void ThenEnsureTheAccountIsDebited(decimal amountToWithdraw)
        {
            Assert.Equal(amountToWithdraw, _account.GetCurrentFunds());
        }

        [Then(@"ensure ATM has (.*) after money is dispensed")]
        public void ThenEnsureATMHasAfterMoneyIsDispensed(decimal remainingCash)
        {
            Assert.Equal(remainingCash, _atm.GetCurrentCashAmount());
        }

        [Then(@"ensure the card is not returned yet")]
        public void ThenEnsureTheCardIsNotReturnedYet()
        {
            Assert.True(_atm.HasCardInSlot());
        }

        [Then(@"ensure the message ""([^""]*)"" is displayed")]
        public void ThenEnsureTheMessageIsDisplayed(string p0)
        {
            Assert.Equal(p0, _atm.GetDisplayedMessage());
        }

    }
}
