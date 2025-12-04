class Rectangle(int height, int width)
{
    public int height = height;
    public int width = width;

    public void calculateArea() {
        Console.Write($"The area of the rectangle is {height * width}");
    }
}