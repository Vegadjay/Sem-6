interface IMovable {
    public void Move();
}

interface ISound {
    public void MakeSound();
}

class Robot : IMovable, ISound {
    public void Move() {
        Console.WriteLine("Robot is moving");
    }
    public void MakeSound() {
        Console.WriteLine("Robot is sounding");
    }
}