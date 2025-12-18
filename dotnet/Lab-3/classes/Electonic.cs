public abstract class Electonic {
    public abstract void TurnOn();
}

public class Fan: Electonic {
    public override void TurnOn() {
        Console.WriteLine("Fan is turning on");
    }
}

public class Light: Electonic {
    public override void TurnOn()
    {
        Console.WriteLine("Light is on...");
    }
}