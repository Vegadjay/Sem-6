
class BankAccount {
    public string? name;
    public int accountNumber;
    public int balance;

    public void Deposit(int amount) {
        balance += amount;
    }

    public void Withdraw(int amount) {
        if(balance < amount) {
            InvalidBankBalanceException.InvalidBankBalance();
        }
        else {
            balance -= amount;
        }
    }

    public void DisplayBalance() {
        Console.WriteLine($"The balance is {balance}");
    }
}


public class InvalidBankBalanceException: Exception {
    public static void InvalidBankBalance() {
        throw new InvalidOperationException("Bank balance is invalid");
    }
}