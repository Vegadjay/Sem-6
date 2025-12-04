namespace dotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            Student s1 = new Student();
            s1.name = "Unknown";
            s1.rollnumber = 121;
            s1.marks = 98;

            // s1.displayStudent();

            Rectangle r1 = new Rectangle(10, 20);
            // r1.calculateArea();

            DivideZero dz = new DivideZero();
            dz.divide(10,5);

            BankAccount ba = new BankAccount();
            ba.name = "John Doe";
            ba.accountNumber = 1234567890;
            ba.balance = 1000;
            ba.Deposit(500);
            ba.Withdraw(2000);
            ba.DisplayBalance();
        }
    }
}