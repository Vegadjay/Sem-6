class DivideZero {
    public void divide(int a, int b) {
        try {
            int result = a / b;
            Console.WriteLine($"The result is {result}");
        } catch (DivideByZeroException e) {
            Console.WriteLine("Error: Division by zero");
        }
    }
}