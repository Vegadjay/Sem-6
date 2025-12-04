class Student{
    public string? name;
    public int rollnumber;
    public int marks;

    public void displayStudent() {
        Console.Write($"{name} has {marks} marks and roll number is {rollnumber}");
    }
}