using ParkeringsApp.Classes;
using ParkeringsApp.Models;
using Spectre.Console;

public class ParkingSessionManager
{
    public static void StartParkingSession(ParkingAppDbContext dbContext, User loggedInUser)
    {
        int zoneId = SelectZone(dbContext); // Step 1: Select a zone
        var (startTime, endTime) = TimeHelper.SetTimes(); // Step 2: Set start and end time
        int carId = ChooseCar(dbContext, loggedInUser.UserId); // Step 3: Choose a car

        if (ConfirmSession(zoneId, startTime, endTime, carId)) // Step 4: Confirm the session details
        {
            SaveActiveParkingSession(dbContext, loggedInUser.UserId, zoneId, startTime, endTime, carId); // Step 5: Save the parking session
        }
        else
        {
            AnsiConsole.Markup("[red]Parking session was not confirmed. Exiting...[/]");
        }
    }

    public static int SelectZone(ParkingAppDbContext ourDatabase)
    {
        // Get all zones from the database
        var zones = ourDatabase.Zones.ToList();

        Console.WriteLine("\nAvailable zones:");

        ZoneManager.ListAllZones();

        // Ask the user to enter the actual ZoneId
        Console.Write("\nEnter the ZoneId you want to select: ");
        int zoneChoice;

        // Validate the input to make sure it matches an existing ZoneId
        while (!int.TryParse(Console.ReadLine(), out zoneChoice) || !zones.Any(z => z.ZoneId == zoneChoice))
        {
            AnsiConsole.Markup("[red]\nInvalid ZoneId. Please enter a valid ZoneId from the list.[/]");
        }

        // Return the chosen zone ID
        return zoneChoice;
    }
    public static (DateTime startTime, DateTime? endTime) SetTimes()
    {
        DateTime startTime;

        // Ask if the user wants to use the current time for the start time
        Console.Write("\nDo you want to use the current time as the start time? (y/n): ");
        string useCurrentTime = Console.ReadLine()?.ToLower()!;

        if (useCurrentTime == "y")
        {
            startTime = DateTime.Now; // Use current date and time
            Console.WriteLine($"Using current time: {startTime:yyyy-MM-dd HH:mm}");
        }
        else
        {
            // Ask for the start time if the user doesn't want the current time
            Console.Write("Enter start time (yyyy-MM-dd HH:mm): ");
            startTime = DateTime.Parse(Console.ReadLine()!);
        }

        // Ask for the end time
        Console.Write("\nEnter end time (yyyy-MM-dd HH:mm): ");
        DateTime? endTime = DateTime.TryParse(Console.ReadLine(), out DateTime parsedEndTime) ? parsedEndTime : (DateTime?)null;

        return (startTime, endTime);
    }
    public static int ChooseCar(ParkingAppDbContext ourDatabase, int loggedinUserId)
    {
        // Get all cars registered by the user
        var cars = ourDatabase.Cars.Where(c => c.UserId == loggedinUserId).ToList();

        Console.WriteLine("\nYour registered cars:");

        // Display the cars to the user
        foreach (var car in cars)
        {
            Console.WriteLine($"Car ID: {car.CarId} - {car.Model}");
        }

        // Ask the user to choose a car by CarId
        Console.Write("\nSelect a car by entering its Car ID: ");
        int carChoice = int.Parse(Console.ReadLine()!);

        // Check if the entered CarId is valid
        var selectedCar = cars.FirstOrDefault(c => c.CarId == carChoice);

        if (selectedCar != null)
        {
            // Return the chosen car ID
            return selectedCar.CarId;
        }
        else
        {
            AnsiConsole.Markup("[red]Invalid Car ID. Please try again.[/]");
            return ChooseCar(ourDatabase, loggedinUserId); // Recursively ask again if the CarId is invalid
        }
    }
    public static bool ConfirmSession(int zoneId, DateTime startTime, DateTime? endTime, int carId)
    {
        Console.WriteLine($"\nSession details: Zone ID: {zoneId}, Start Time: {startTime}, End Time: {endTime?.ToString() ?? "Not specified"}, Car ID: {carId}");
        Console.Write("Do you want to confirm this session? (y/n): ");
        return Console.ReadLine()?.ToLower() == "y";
    }

    public static void SaveActiveParkingSession(ParkingAppDbContext dbContext, int loggedInUserId, int zoneId, DateTime startTime, DateTime? endTime, int carId)
    {
        ActiveParking newSession = new ActiveParking
        {
            UserId = loggedInUserId,
            ZoneId = zoneId,
            StartTime = startTime,
            EndTime = endTime,
            CarId = carId,
            Status = "Active"
        };

        dbContext.ActiveParkings.Add(newSession);
        dbContext.SaveChanges();
        Console.WriteLine("Parking session started successfully!");
        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }
}