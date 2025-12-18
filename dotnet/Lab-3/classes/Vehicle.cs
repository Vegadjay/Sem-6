class Vehicle {
    public  void Type() {
        Console.WriteLine("Type is vehicle");
    }
}

class Car: Vehicle {
    public void CarVehicleType() {
        Console.WriteLine("Type of Car");
    }
}


class ElectricVehicle: Car {
    public void ElectricVehicleType() {
        Console.WriteLine("Type of Car is electric vehicle");
    }
}