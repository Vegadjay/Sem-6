interface IPrintable {
    public void PrintDetails();
}

class Book : IPrintable {
    public void PrintDetails() {
        Console.WriteLine("Book is fetching.....");
    }
}



class Magazine : IPrintable {
    public void PrintDetails() {
        Console.WriteLine("Magazine is fetching.....");
    }
}