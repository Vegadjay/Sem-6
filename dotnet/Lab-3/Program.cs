namespace dotnet;
class Program {
    static void Main(string[] args) {
        // ! Lab-1
        
        // Animal a = new Animal();
        // a.Eat();
        // Console.WriteLine();
        // Dog d = new Dog();
        // d.Eat();
        // Console.WriteLine();
        // d.Bark();

        // ! Lab-2

        // Vehicle v = new Vehicle();
        // v.Type();
        // Car c = new Car();
        // c.CarVehicleType();
        // ElectricVehicle ev = new ElectricVehicle();
        // ev.ElectricVehicleType();
        // ev.Type();
        // ev.CarVehicleType();

        // ! Program-3
        
    //     Shape shape;
    //     string s = Console.ReadLine();
    //     switch (s)
    //     {
    //         case "r":
    //             {
    //                 Console.WriteLine("Enter length:");
    //                 int length = Convert.ToInt32(Console.ReadLine());
    //                 Console.WriteLine("Enter width:");
    //                 int width = Convert.ToInt32(Console.ReadLine());
    //                 shape = new RectangleShape(length, width);
    //                 break;
    //             }
    //         case "c":
    //             {
    //                 Console.WriteLine("Enter radius:");
    //                 double radius = Convert.ToDouble(Console.ReadLine());
    //                 shape = new CircleShape(radius);
    //                 break;
    //             }
    //         default:
    //             {
    //                 shape = new Shape();
    //                 Console.WriteLine("Invalid input");
    //                 break;
    //             }
    //     }
    //     shape.CalculateArea();
    
        // ! Lab-4
        
        // Electonic e = new Fan();
        // e.TurnOn();
        // Electonic l = new Light();
        // l.TurnOn();


        // ! Lab-5

        // Book b = new Book();
        // b.PrintDetails();

        // Magazine m = new Magazine();
        // m.PrintDetails();


        // ! Lab-6

        // Robot r = new Robot();
        // r.Move();
        // r.MakeSound();

        // ! Lab-7

        Payment p = new CreditCardPayment(100);
        p.MakePayment();
        Payment p2 = new UPIPayment(100);
        p2.MakePayment();   
    
    }
}