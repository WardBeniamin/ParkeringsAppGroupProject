using ParkeringsApp.Models;
using Spectre.Console;

public class CarManager
{
    public static void AddNewCar(User loggedInUser)
    {
        Console.Write("\nPlease enter your plate number: ");
        string plateNumber = Console.ReadLine()!;
        Console.Write("Please enter the car model: ");
        string model = Console.ReadLine()!;
        
        if (IsPlateNumberTaken(plateNumber))
        {
            AnsiConsole.Markup("[red]Car plates is already in use. Please try a different one.[/]");
            Console.ReadKey();
            return;
        }

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
        Console.WriteLine("\nPress any key to return to main menu");
        Console.ReadKey();
    }

    public static bool IsPlateNumberTaken(string carPlates)
    {
        using (var dbContext = new ParkingAppDbContext())
        {
            return dbContext.Cars.Any(c => c.PlateNumber == carPlates);
        }
    }

}