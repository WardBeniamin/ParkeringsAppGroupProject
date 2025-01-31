using ParkeringsApp.Models;

public class CarManager
{
    public static void AddNewCar(User loggedInUser)
    {
        Console.Write("\nPlease enter your plate number: ");
        string plateNumber = Console.ReadLine()!;
        Console.Write("Please enter the car model: ");
        string model = Console.ReadLine()!;

        Car newCar = new Car
        {
            PlateNumber = plateNumber,
            Model = model,
            UserId = loggedInUser.UserId
        };

        using (var dbContext = new ParkingAppDbContext())
        {
            dbContext.Cars.Add(newCar);
            dbContext.SaveChanges();
        }
        Console.WriteLine("Car has been added successfully!");
    }
}