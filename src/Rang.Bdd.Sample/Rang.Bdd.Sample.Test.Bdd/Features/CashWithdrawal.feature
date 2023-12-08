Feature: CashWithdrawal

Feature: ATM Cash Withdrawal

Story:
As a bank user
I want to withdraw cash from my bank account while on non business hours
so that I can use my money when I need it. 

Refined Story:
As card holder
I want to use my ATM card to withdraw money from my bank account at any ATM
so that I can use my money when I need it. 

The following scenarios are the acceptance criteria of the story above:

@AtmCashWithdrawal
Scenario: Account has suficient funds
	Given that account holds 1000 money
	And the ATM card has a valid expiration date
	And the ATM has 8500 cash
	And the ATM card has been inserted into the ATM
	When the user request 500 cash
	Then ensure the account is debited 500
	And ensure ATM has 8000 after money is dispensed
	And ensure the card is returned
