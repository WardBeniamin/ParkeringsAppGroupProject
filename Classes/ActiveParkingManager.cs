using Microsoft.EntityFrameworkCore;
using ParkeringsApp.Models;
using Spectre.Console;

namespace ParkeringsApp.Classes
{
    public class ActiveParkingManager
    {
        public static void ManageActiveParking(ParkingAppDbContext OurDatabase, int loggedinUserId)
        {
            // Fetch the active parking sessions for the user
            var activeParkings = GetActiveParkings(OurDatabase, loggedinUserId);

            // Display the active parking sessions to the user
            DisplayActiveParkings(activeParkings);

            // If no active parking sessions are found, exit early
            if (activeParkings.Count == 0)
            {
                Console.Write("\nYou have no active parking");
                Console.WriteLine("Press any key to return to main menu");
                Console.ReadKey();
                return;
            }

            // Ask the user to select a parking session to manage
            int sessionChoice;
            while (true)
            {
                Console.Write("\nSelect a parking session to manage (enter ticket number): ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    AnsiConsole.Markup("[red]Input cannot be empty. Please enter a valid ticket number.[/]\n");
                    continue;
                }

                if (!int.TryParse(input, out sessionChoice))
                {
                    AnsiConsole.Markup("[red]Invalid input. Please enter a number.[/]\n");
                    continue;
                }

                if (sessionChoice < 1 || sessionChoice > activeParkings.Count)
                {
                    AnsiConsole.Markup("[red]Invalid choice. Please enter a valid ticket number.[/]\n");
                    continue;
                }

                break;
            }

            var selectedSession = activeParkings[sessionChoice - 1];

            Console.WriteLine($"\nManaging parking session at Zone {selectedSession.ZoneId} with Car ID {selectedSession.CarId}");

            // Get the user's action choice (either stop the session or change the end time)
            int actionChoice = GetActionChoice();

            switch (actionChoice)
            {
                case 1:
                    StopParkingSession(selectedSession, OurDatabase);
                    break;

                case 2:
                    ChangeEndTime(selectedSession, OurDatabase);
                    break;

                case 3:
                    Console.WriteLine("\nReturning to the main menu...");
                    return;

                default:
                    AnsiConsole.Markup("[red]\nInvalid choice. Please enter 1, 2, or 3.[/]");
                    break;
            }

        }
        public static List<ActiveParking> GetActiveParkings(ParkingAppDbContext OurDatabase, int userId)
        {
            // Get all active parking sessions for the user
            return OurDatabase.ActiveParkings
                              .Where(ActiveParking => ActiveParking.UserId == userId && ActiveParking.Status == "Active")
                              .ToList();
        }
        public static void DisplayActiveParkings(List<ActiveParking> activeParkings)
        {
            // If no active parking sessions are found, display a message and return
            if (activeParkings.Count == 0)
            {
                Console.WriteLine("\nYou have no active parking sessions.");
                return;
            }

            Console.WriteLine("\nYour active parking sessions:");

            // Create a table
            var table = new Table()
                .AddColumn("Ticket Number")
                .AddColumn("Zone ID")
                .AddColumn("Car ID")
                .AddColumn("Start Time")
                .AddColumn("End Time")
                .AddColumn("Status");

            // Add rows to the table for each active parking session
            for (int i = 0; i < activeParkings.Count; i++)
            {
                var activeParking = activeParkings[i];
                table.AddRow(
                    (i + 1).ToString(),
                    activeParking.ZoneId.ToString(),
                    activeParking.CarId.ToString(),
                    activeParking.StartTime.ToString(),
                    activeParking.EndTime.ToString()!,
                    activeParking.Status
                );
            }

            // Render the table
            AnsiConsole.Write(table);
        }
        public static int GetActionChoice()
        {
            Console.WriteLine("\nWhat would you like to do?");
            Console.WriteLine("1. Stop the parking session.");
            Console.WriteLine("2. Change the end time.");
            Console.WriteLine("3. Return back to main menu.");
            Console.Write("Enter your choice (1, 2 or 3): ");
            return int.Parse(Console.ReadLine()!);
        }
        public static void StopParkingSession(ActiveParking selectedSession, ParkingAppDbContext ourDatabase)
        {
            
            int paymentId = GetUserSelectedPaymentMethod(selectedSession.UserId);
            selectedSession.Status = "Completed";
            selectedSession.EndTime = DateTime.Now; // Set the end time to the current time

            var receipt = new Receipt
            {
                UserId = selectedSession.UserId,
                CarId = selectedSession.CarId,
                ZoneId = selectedSession.ZoneId,
                StartTime = selectedSession.StartTime,
                EndTime = selectedSession.EndTime.Value, // EndTime is guaranteed to have a value here
                Amount = CalculateAmount(selectedSession, ourDatabase), // Calculate fee dynamically
                PaymentId = paymentId
            };

            ourDatabase.Receipts.Add(receipt);
            ourDatabase.ActiveParkings.Remove(selectedSession);
            ourDatabase.SaveChanges();
            Console.WriteLine("Parking session stopped successfully.");
            Console.WriteLine("\nPress any key to return to main menu");
            Console.ReadKey();
        }
        private static decimal CalculateAmount(ActiveParking selectedSession, ParkingAppDbContext ourDatabase)
        {
            var zone = ourDatabase.Zones.FirstOrDefault(z => z.ZoneId == selectedSession.ZoneId);
            if (zone == null)
                throw new Exception("Zone not found.");

            TimeSpan duration = selectedSession.EndTime!.Value - selectedSession.StartTime;
            decimal hours = (decimal)duration.TotalHours;

            return Math.Ceiling(hours) * zone.Fee; // Round up and multiply by zone fee
        }
        public static void ChangeEndTime(ActiveParking selectedSession, ParkingAppDbContext ourDatabase)
        {
            // Ask for the new end time
            Console.Write("Enter new end time (yyyy-MM-dd HH:mm): ");
            DateTime newEndTime = DateTime.Parse(Console.ReadLine()!);

            // Update the end time of the parking session
            selectedSession.EndTime = newEndTime;
            ourDatabase.SaveChanges();
            Console.WriteLine("End time updated successfully.");
            Console.WriteLine("\nPress any key to return to main menu");
            Console.ReadKey();
        }

        public static int GetUserSelectedPaymentMethod(int userId)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var user = ourDatabase.Users.Include(u => u.Payments)
                                            .FirstOrDefault(u => u.UserId == userId);

                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return -1; // Return -1 to indicate failure
                }

                if (user.Payments.Count == 0)
                {
                    Console.WriteLine("No available payment methods found.");
                    return -1; // Return -1 if the user has no payment methods
                }

                Console.WriteLine($"Payment methods for {user.FullName}:");

                // Dictionary to map user input to payment IDs
                Dictionary<int, int> paymentOptions = new Dictionary<int, int>();
                int optionNumber = 1;

                foreach (var method in user.Payments)
                {
                    Console.WriteLine($"{optionNumber}. {method.PaymentType}");
                    paymentOptions[optionNumber] = method.PaymentId;
                    optionNumber++;
                }

                // Ask the user to choose a valid payment method
                int selectedOption;
                while (true)
                {
                    Console.Write("\nEnter the number of the payment method you want to use: ");
                    string input = Console.ReadLine()!;

                    if (int.TryParse(input, out selectedOption) && paymentOptions.ContainsKey(selectedOption))
                    {
                        return paymentOptions[selectedOption]; // Return the selected payment ID
                    }
                    AnsiConsole.Markup("[red]Invalid selection. Please choose a valid option.[/]");
                }
            }
        }

    }
}
