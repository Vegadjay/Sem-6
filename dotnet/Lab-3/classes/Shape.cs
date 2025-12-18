namespace dotnet
{
    public class Shape
    {
        public virtual void CalculateArea() {
            Console.WriteLine("Invalid input");
        }
    }
    public class CircleShape : Shape {
        public double radius;
        public CircleShape(double radius) { 
            this.radius = radius;
        }
        public override void CalculateArea() {
            Console.WriteLine("Area of shape is : "+radius*radius*3.14);
        }
    }
    public class RectangleShape : Shape {
        public int length;
        public int breadth;
        public RectangleShape(int length, int breadth)
        {
            this.length = length;
            this.breadth = breadth;
        }
        public override void CalculateArea()
        {
            Console.WriteLine("Area of shape is : " + length*breadth);
        }
    }
    public class TriangleShape : Shape {
        public int hight;
        public int width;
        public TriangleShape(int hight, int width)
        {
            this.hight = hight;
            this.width = width;
        }
        public override void CalculateArea()
        {
            Console.WriteLine("Area of shape is : " + 0.5*hight*width);
        }
    }
}
