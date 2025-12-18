using System;

public abstract class Payment
{
    public decimal Amount { get; set; }
    public Payment(decimal amount)
    {
        Amount = amount;
    }
    public abstract void MakePayment();
}

public class CreditCardPayment : Payment
{
    public CreditCardPayment(decimal amount) : base(amount) { }

    public override void MakePayment()
    {
        try
        {
            if (Amount < 100)
                throw new ArgumentException("Amount must be at least ₹100 for payment.");

            Console.WriteLine($"Credit Card payment of ₹{Amount} successful.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Credit Card Payment failed: {ex.Message}");
        }
    }
}

public class UPIPayment : Payment
{
    public UPIPayment(decimal amount) : base(amount) { }

    public override void MakePayment()
    {
        try
        {
            if (Amount < 100)
                throw new ArgumentException("Amount must be at least ₹100 for payment.");

            Console.WriteLine($"UPI payment of ₹{Amount} successful.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UPI Payment failed: {ex.Message}");
        }
    }
}
