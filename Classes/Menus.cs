using ParkeringsApp.Models;
using Spectre.Console;
using Figgle;

namespace ParkeringsApp.Classes
{
    public static class Menus
    {
        // Method for displaying header and interactive menu
        public static string ShowHeaderAndMenu(string subtitle, string[] options)
        {
            Console.Clear();

            // Display header
            var header = Figgle.FiggleFonts.Rounded.Render("ParkIt");
            AnsiConsole.Markup($"[deepskyblue3_1]{header}[/]\n");
            AnsiConsole.Markup($"[deepskyblue2]{subtitle}[/]\n");

            // Create interactive menu
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\n[deepskyblue2]Select an option:[/]")
                    .PageSize(options.Length)
                    .HighlightStyle("deepskyblue3_1")
                    .AddChoices(options)
            );
        }

        // Login method for authenticating a user
        public static User Login()
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                Console.Write("\nEnter email: ");
                string email = Console.ReadLine()!;

                Console.Write("Enter password: ");
                string password = Console.ReadLine()!;

                var user = ourDatabase.Users.SingleOrDefault(u => u.Email == email && u.Password == password);

                if (user != null)
                {
                    Console.WriteLine($"Welcome, {user.FullName}!");
                    return user;  // Return the logged-in user
                }
                else
                {
                    
                    return null!;  // Login failed
                }
            }
        }

        // Main Menu display method
        public static void ShowMainMenu(User loggedInUser)
        {
            bool isRunning = true;

            while (isRunning)
            {
                // Reusing ShowHeaderAndMenu for Main Menu
                string menuSelection = ShowHeaderAndMenu(
                    $"=== Main Menu === (Logged in as: {loggedInUser.FullName})",
                    new[] { "Register car", "Start a parking", "Ongoing parking", "Receipts", "List all zones", "Edit Profile", "Log out" }
                );

                switch (menuSelection)
                {
                    case "Register car":
                        CarManager.AddNewCar(loggedInUser);
                        break;

                    case "Start a parking":
                        using (var ourDatabase = new ParkingAppDbContext())
                        {
                            ParkingSessionManager.StartParkingSession(ourDatabase, loggedInUser);
                        }
                        break;

                    case "Ongoing parking":
                        using (var ourDatabase = new ParkingAppDbContext())
                        {
                            ActiveParkingManager.ManageActiveParking(ourDatabase, loggedInUser.UserId);
                        }
                        break;

                    case "Receipts":
                        ReceiptManager.ShowReceipts(loggedInUser.UserId);
                        break;

                    case "List all zones":
                        ZoneManager.ListAllZones();
                        Console.WriteLine("\nPress any key to return to the main menu...");
                        Console.ReadKey();
                        break;

                    case "Edit Profile":
                        UserManager.EditProfileOrDelete(loggedInUser);
                        break;

                    case "Log out":
                        Console.WriteLine("Logging out...");
                        isRunning = false;
                        break;
                }
            }
        }
    }
}