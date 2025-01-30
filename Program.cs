using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ParkeringsApp.Models;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;

namespace ParkeringsApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool appRunning = true;

            while (appRunning)
            {
                // Display the start menu
                Console.Clear();
                Console.WriteLine("=== Welcome to the Parking App ===");
                Console.WriteLine("1. Log in");
                Console.WriteLine("2. Create an account");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option (1-3): ");

                string startChoice = Console.ReadLine()!;

                switch (startChoice)
                {
                    case "1":
                        User loggedInUser = Login(); // Get the user object from the Login method
                        if (loggedInUser != null)
                        {
                            ShowMainMenu(loggedInUser); // Call the main menu with the logged-in user
                        }
                        else
                        {
                            Console.WriteLine("Login failed. Press Enter to try again...");
                            Console.ReadLine();
                        }
                        break;
                    case "2":
                        //CreateAccount();
                        break;
                    case "3":
                        Console.WriteLine("Exiting the app. Goodbye!");
                        appRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

            }
        }

        static User Login()
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                Console.Write("Enter email: ");
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
                    Console.WriteLine("Invalid email or password. Try again.");
                    return null;  // Login failed
                }
            }
        }

        static void CreateAccount(ParkingAppDbContext OurDatabase)
        {
            Console.Clear();
            Console.WriteLine("=== Skapa konto ===");

            Console.Write("Ange ett unikt användar-ID (nummer): ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Ogiltigt ID. Det måste vara ett nummer.");
                Console.ReadKey();
                return;
            }

            // Kontrollera om ID redan finns i databasen
            var existingUser = OurDatabase.Users.SingleOrDefault(u => u.UserId == userId);
            if (existingUser != null)
            {
                Console.WriteLine("Användar-ID är redan upptaget. Välj ett annat.");
                Console.ReadKey();
                return;
            }

            Console.Write("Ange lösenord för ditt nya konto: ");
            string password = Console.ReadLine()!;

            // Skapa en ny användare
            var newUser = new User
            {
                UserId = userId,
                Password = password // OBS: Hasha lösenordet i framtiden!
            };

            // Spara den nya användaren i databasen
            OurDatabase.Users.Add(newUser);
            OurDatabase.SaveChanges();

            Console.WriteLine("Konto skapades framgångsrikt!");
            Console.ReadKey();
        }
        static void ShowMainMenu(User loggedInUser)
        {
            bool isRunning = true;

            while (isRunning)
            {
                // Display the main menu
                Console.Clear();
                Console.WriteLine($"=== Main Menu === (Logged in as: {loggedInUser.FullName})");
                Console.WriteLine("1. Start a parking");
                Console.WriteLine("2. Ongoing parking");
                Console.WriteLine("3. Receipts");
                Console.WriteLine("4. List all zones");
                Console.WriteLine("5. Edit Profile");
                Console.WriteLine("6. Log out");
                Console.Write("Choose an option (1-6): ");

                string userInputMenuChoice = Console.ReadLine()!;

                switch (userInputMenuChoice)
                {
                    case "1":
                        using (var ourDatabase = new ParkingAppDbContext())
                        {
                            StartParkingSession(ourDatabase, loggedInUser);
                        }
                        break;
                    case "2":
                        using (var ourDatabase = new ParkingAppDbContext())
                        {
                            ManageActiveParking(ourDatabase, loggedInUser.UserId);
                        }

                        break;
                    case "3":
                        ShowReceipts(loggedInUser.UserId);
                        break;
                    case "4":
                        ListAllZones();
                        break;
                    case "5":
                        EditProfileOrDelete(loggedInUser); // Pass loggedInUser
                        if (!UserExists(loggedInUser.UserId)) // Check if the user exists
                        {
                            Console.WriteLine("You have been deleted. Exiting the menu...");
                            isRunning = false; // Exit if the user was deleted
                        }
                        break;
                    case "6":
                        Console.WriteLine("Logging out...");
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        // Start -- StartParking -- 
        public static void StartParkingSession(ParkingAppDbContext ourDatabase, User loggedInUser)
        {
            // Step 1: Select a zone
            int zoneId = SelectZone(ourDatabase);

            // Step 2: Set start and end time
            var (startTime, endTime) = SetTimes();

            // Step 3: Choose a car
            int carId = ChooseCar(ourDatabase, loggedInUser.UserId);

            // Step 4: Confirm the session details
            if (ConfirmSession(zoneId, startTime, endTime, carId))
            {
                // Step 5: Save the parking session
                SaveActiveParkingSession(ourDatabase, loggedInUser.UserId, zoneId, startTime, endTime, carId);
            }
            else
            {
                Console.WriteLine("Parking session was not confirmed. Exiting...");
            }
        }

        public static int SelectZone(ParkingAppDbContext ourDatabase)
        {
            // Get all zones from the database
            var zones = ourDatabase.Zones.ToList();

            Console.WriteLine("Available zones:");

            // Display the zones to the user, showing the ZoneId (not the index)
            foreach (var zone in zones)
            {
                Console.WriteLine($"ZoneId: {zone.ZoneId} - Fee: {zone.Fee} - Address: {zone.Adress}");
            }

            // Ask the user to enter the actual ZoneId
            Console.Write("Enter the ZoneId you want to select: ");
            int zoneChoice;

            // Validate the input to make sure it matches an existing ZoneId
            while (!int.TryParse(Console.ReadLine(), out zoneChoice) || !zones.Any(z => z.ZoneId == zoneChoice))
            {
                Console.WriteLine("Invalid ZoneId. Please enter a valid ZoneId from the list.");
            }

            // Return the chosen zone ID
            return zoneChoice;
        }


        public static (DateTime startTime, DateTime? endTime) SetTimes()
        {
            DateTime startTime;

            // Ask if the user wants to use the current time for the start time
            Console.Write("Do you want to use the current time as the start time? (y/n): ");
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
            Console.Write("Enter end time (yyyy-MM-dd HH:mm): ");
            DateTime? endTime = DateTime.TryParse(Console.ReadLine(), out DateTime parsedEndTime) ? parsedEndTime : (DateTime?)null;

            return (startTime, endTime);
        }


        public static int ChooseCar(ParkingAppDbContext ourDatabase, int loggedinUserId)
        {
            // Get all cars registered by the user
            var cars = ourDatabase.Cars.Where(c => c.UserId == loggedinUserId).ToList();

            Console.WriteLine("Your registered cars:");

            // Display the cars to the user
            foreach (var car in cars)
            {
                Console.WriteLine($"Car ID: {car.CarId} - {car.Model}");
            }

            // Ask the user to choose a car by CarId
            Console.Write("Select a car by entering its Car ID: ");
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
                Console.WriteLine("Invalid Car ID. Please try again.");
                return ChooseCar(ourDatabase, loggedinUserId); // Recursively ask again if the CarId is invalid
            }
        }


        public static bool ConfirmSession(int zoneId, DateTime startTime, DateTime? endTime, int carId)
        {
            // Display session details
            Console.WriteLine("\nSession details:");
            Console.WriteLine($"Zone ID: {zoneId}");
            Console.WriteLine($"Start Time: {startTime}");
            Console.WriteLine($"End Time: {(endTime.HasValue ? endTime.Value.ToString() : "Not specified")}");
            Console.WriteLine($"Car ID: {carId}");

            // Ask for confirmation
            Console.Write("Do you want to confirm this session? (y/n): ");
            string confirmation = Console.ReadLine()!.ToLower();

            return confirmation == "y";
        }

        public static void SaveActiveParkingSession(ParkingAppDbContext ourDatabase, int loggedinUserId, int zoneId, DateTime startTime, DateTime? endTime, int carId)
        {
            // Create the ActiveParking object
            ActiveParking newSession = new ActiveParking
            {
                UserId = loggedinUserId,
                ZoneId = zoneId,
                StartTime = startTime,
                EndTime = endTime,
                CarId = carId,
                Status = "Active" // Default to Active
            };

            // Add the new session to the database
            ourDatabase.ActiveParkings.Add(newSession);
            ourDatabase.SaveChanges(); // Save changes to the DB

            Console.WriteLine("Parking session started successfully!");
        }

        // End -- StartParking -- 

        // Start -- ManageActiveParking -- 

        public static void ManageActiveParking(ParkingAppDbContext OurDatabase, int loggedinUserId)
        {
            // Fetch the active parking sessions for the user
            var activeParkings = GetActiveParkings(OurDatabase, loggedinUserId);

            // Display the active parking sessions to the user
            DisplayActiveParkings(activeParkings);

            // If no active parking sessions are found, exit early
            if (activeParkings.Count == 0)
            {
                return;
            }

            // Ask the user to select a parking session to manage
            Console.Write("Select a parking session to manage (enter the number): ");
            int sessionChoice = int.Parse(Console.ReadLine()!);

            // Validate the session choice
            if (sessionChoice < 1 || sessionChoice > activeParkings.Count)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            var selectedSession = activeParkings[sessionChoice - 1];

            Console.WriteLine($"Managing parking session at Zone {selectedSession.ZoneId} with Car ID {selectedSession.CarId}");

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
                    Console.WriteLine("Returning to the main menu...");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
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
                Console.WriteLine("You have no active parking sessions.");
                return;
            }

            Console.WriteLine("Your active parking sessions:");

            // Display the active parking sessions to the user
            for (int i = 0; i < activeParkings.Count; i++)
            {
                var activeParking = activeParkings[i];
                Console.WriteLine($"{i + 1}. Zone ID: {activeParking.ZoneId} - Car ID: {activeParking.CarId} - Start Time: {activeParking.StartTime} - End Time: {activeParking.EndTime} - Status: {activeParking.Status}");
            }
        }

        public static int GetActionChoice()
        {
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Stop the parking session.");
            Console.WriteLine("2. Change the end time.");
            Console.WriteLine("3. Return back to main menu.");
            Console.Write("Enter your choice (1, 2 or 3): ");
            return int.Parse(Console.ReadLine()!);
        }

        public static void StopParkingSession(ActiveParking selectedSession, ParkingAppDbContext ourDatabase)
        {
            // Stop the parking session by updating its status to "Completed"
            selectedSession.Status = "Completed";
            selectedSession.EndTime = DateTime.Now; // Set the end time to the current time

            var receipt = new Receipt
            {
                UserId = selectedSession.UserId,
                CarId = selectedSession.CarId,
                ZoneId = selectedSession.ZoneId,
                StartTime = selectedSession.StartTime,
                EndTime = selectedSession.EndTime.Value, // EndTime is guaranteed to have a value here
                Amount = CalculateAmount(selectedSession, ourDatabase) // Calculate fee dynamically
            };

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

            TimeSpan duration = selectedSession.EndTime.Value - selectedSession.StartTime;
            decimal hours = (decimal)duration.TotalHours;

            return Math.Ceiling(hours) * zone.Fee; // Round up and multiply by zone fee
        }

        public static void ChangeEndTime(ActiveParking selectedSession, ParkingAppDbContext ourDatabase)
        {
            // Ask for the new end time
            Console.Write("Enter new end time (yyyy-MM-dd HH:mm): ");
            DateTime newEndTime = DateTime.Parse(Console.ReadLine());

            // Update the end time of the parking session
            selectedSession.EndTime = newEndTime;
            OurDatabase.SaveChanges();
            Console.WriteLine("End time updated successfully.");
            Console.WriteLine("\nPress any key to return to main menu");
            Console.ReadKey();
        }


        // End -- ManageActiveParking -- 

        static void ShowReceipts(int loggedInUser)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                // Hämta alla kvitton för den inloggade användaren
                var userReceipts = ourDatabase.Receipts
                    .Include(r => r.Car) // Inkludera bilinformation
                    .Include(r => r.Zone) // Inkludera zonsinformation
                    .Include(r => r.Payment) // Inkludera betalningsmetod
                    .Where(r => r.UserId == loggedInUser)
                    .ToList();

                if (userReceipts.Any())
                {
                    Console.WriteLine("\n=== Your Receipts ===");
                    foreach (var receipt in userReceipts)
                    {
                        Console.WriteLine($"Receipt ID: {receipt.TransactionId}");
                        Console.WriteLine($"Car: {receipt.Car.PlateNumber} ({receipt.Car.Model})");
                        Console.WriteLine($"Zone: {receipt.Zone.Adress} (Fee: {receipt.Zone.Fee} SEK/hour)");
                        Console.WriteLine($"Payment Method: {receipt.Payment.PaymentType}");
                        Console.WriteLine($"Start Time: {receipt.StartTime}");
                        Console.WriteLine($"End Time: {receipt.EndTime}");
                        Console.WriteLine($"Total Amount: {receipt.Amount} SEK");
                        Console.WriteLine("-----------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("No receipts found for your account.");
                }
            }

            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
        }

        static void ListAllZones()
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var zones = ourDatabase.Zones.ToList();

                if (zones.Any())
                {
                    Console.WriteLine("=== Available Parking Zones ===");
                    foreach (var zone in zones)
                    {
                        Console.WriteLine($"Zone ID: {zone.ZoneId}");
                        Console.WriteLine($"Address: {zone.Adress}");
                        Console.WriteLine($"Fee: {zone.Fee} SEK/hour");
                        Console.WriteLine("-----------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("No parking zones found.");
                }
            }

            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
        }
        static void EditProfileOrDelete(User loggedInUser)
        {
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Edit Profile");
            Console.WriteLine("2. Delete User");
            Console.Write("\nEnter your choice (1 or 2): ");

            var userEditProfileChoice = Console.ReadLine();

            if (userEditProfileChoice == "1")
            {
                EditProfile(loggedInUser);
            }
            else if (userEditProfileChoice == "2")
            {
                DeleteUser(loggedInUser.UserId);
            }
            else
            {
                Console.WriteLine("Invalid choice. Press any key to return to the main menu...");
                Console.ReadKey();
            }
        }
        static bool UserExists(int userId)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                return ourDatabase.Users.Any(u => u.UserId == userId);
            }
        }
        static void EditProfile(User loggedInUser)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var loggedInUserId = ourDatabase.Users.SingleOrDefault(user => user.UserId == loggedInUser.UserId);

                if (loggedInUserId == null)
                {
                    Console.WriteLine("\nUser not found. Please log in again.");
                    return;
                }

                Console.WriteLine($"\nEditing profile for: {loggedInUserId.FullName}");

                // Update FullName
                Console.WriteLine("Enter full name (leave blank to keep current):");
                var fullName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    loggedInUserId.FullName = fullName;
                }

                // Update Email
                Console.WriteLine("Enter new email (leave blank to keep current): ");
                var email = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(email))
                {
                    if (ourDatabase.Users.Any(user => user.Email == email && user.UserId != loggedInUser.UserId))
                    {
                        Console.WriteLine("Email already in use. Canceling profile update.");
                        return;
                    }
                    loggedInUserId.Email = email;
                }

                // Update Address
                Console.WriteLine("Enter new address (leave blank to keep current): ");
                var address = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(address))
                {
                    loggedInUserId.Adress = address;
                }

                // Update PhoneNumber
                Console.Write("Enter new phone number (leave blank to keep current): ");
                var phoneNumber = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    loggedInUserId.PhoneNumber = phoneNumber;
                }

                // Update Password
                Console.Write("\nEnter new password (leave blank to keep current): ");
                var password = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(password))
                {
                    loggedInUserId.Password = password;
                }

                // Save changes to the database
                try
                {
                    ourDatabase.SaveChanges();
                    Console.WriteLine("Profile updated successfully! Press any key to return to the main menu...");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating profile: {ex.Message}");
                    Console.WriteLine("Press any key to return to the main menu...");
                    Console.ReadKey();
                }
            }
        }

        static void DeleteUser(int loggedInUser)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var userToDelete = ourDatabase.Users
                    .Include(u => u.Cars) // Include user's cars
                    .Include(u => u.PaymentMethods) // Include payment methods (many-to-many)
                    .SingleOrDefault(u => u.UserId == loggedInUser);

                if (userToDelete == null)
                {
                    Console.WriteLine("User not found. Please log in again. Press any key to return to login menu.");
                    Console.ReadKey();
                    return;
                }

                while (true) // Loop until user provides a valid answer
                {
                    Console.WriteLine($"\nAre you sure you want to delete your account, {userToDelete.FullName}? This action cannot be undone.");
                    Console.WriteLine("Type 'YES' to confirm, or 'NO' to go back to the main menu.");
                    Console.Write("Your choice: ");
                    string confirmation = Console.ReadLine()!.Trim().ToUpper();

                    if (confirmation == "YES")
                    {
                        // Step 1: Delete Receipts related to the user's cars
                        var userCarIds = userToDelete.Cars.Select(c => c.CarId).ToList();
                        var userReceipts = ourDatabase.Receipts.Where(r => userCarIds.Contains(r.CarId));
                        ourDatabase.Receipts.RemoveRange(userReceipts);

                        // Step 2: Delete Cars owned by the user
                        ourDatabase.Cars.RemoveRange(userToDelete.Cars);

                        // Step 3: Remove Many-to-Many Payment Method Relations
                        userToDelete.PaymentMethods.Clear();

                        // Step 4: Delete the user
                        ourDatabase.Users.Remove(userToDelete);

                        // Save all changes to the database
                        ourDatabase.SaveChanges();

                        Console.WriteLine("\nUser deleted successfully. Press any key to return to the login menu...");
                        Console.ReadKey();
                        return;
                    }
                    else if (confirmation == "NO")
                    {
                        Console.WriteLine("\nAccount deletion canceled. Press any key to return to the main menu...");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please type 'YES' or 'NO'.");
                    }
                }
            }
        }
    }
}