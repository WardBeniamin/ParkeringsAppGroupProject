using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ParkeringsApp.Models;
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
                        // If logged in
                        ShowMainMenu(); // Call the main parking app menu if login is successful
                        break;
                    case "2":
                        Console.WriteLine("Function to create a new user goes here");
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

        static void ShowMainMenu()
        {
            bool isRunning = true;

            while (isRunning)
            {
                // Display the main menu
                Console.Clear();
                Console.WriteLine("=== Main Menu ===");
                Console.WriteLine("1. Start a parking");
                Console.WriteLine("2. Ongoing parking");
                Console.WriteLine("3. Receipts");
                Console.WriteLine("4. List all zones");
                Console.WriteLine("5. Edit Profile");
                Console.WriteLine("6. Log out");
                Console.Write("Choose an option (1-6): ");

                string userInputMenuChoice = Console.ReadLine()!;
                int loggedInUserId = 1;

                switch (userInputMenuChoice)
                {
                    case "1":
                        loggedInUserId = 1;
                        using (var context = new ParkingAppDbContext())
                        {
                            int userId = 1; // You can replace this with the logged-in user's ID
                            StartParkingSession(context, userId);
                        }
                        break;
                    case "2":
                        OngoingParking();
                        break;
                    case "3":
                        ShowReceipts();
                        break;
                    case "4":
                        ListAllZones();
                        break;
                    case "5":
                        loggedInUserId = 1;
                        EditProfileOrDelete(loggedInUserId);
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

        // Placeholder methods for main menu options

        // Start -- StartParking -- 
        public static void StartParkingSession(ParkingAppDbContext OurDatabase, int userId)
        {
            // Step 1: Select a zone
            int zoneId = SelectZone(OurDatabase);

            // Step 2: Set start and end time
            var (startTime, endTime) = SetTimes();

            // Step 3: Choose a car
            int carId = ChooseCar(OurDatabase, userId);

            // Step 4: Confirm the session details
            if (ConfirmSession(zoneId, startTime, endTime, carId))
            {
                // Step 5: Save the parking session
                SaveActiveParkingSession(OurDatabase, userId, zoneId, startTime, endTime, carId);
            }
            else
            {
                Console.WriteLine("Parking session was not confirmed. Exiting...");
            }
        }

        public static int SelectZone(ParkingAppDbContext OurDatabase)
        {
            // Get all zones from the database
            var zones = OurDatabase.Zones.ToList();

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
            string useCurrentTime = Console.ReadLine()?.ToLower();

            if (useCurrentTime == "y")
            {
                startTime = DateTime.Now; // Use current date and time
                Console.WriteLine($"Using current time: {startTime}");
            }
            else
            {
                // Ask for the start time if the user doesn't want the current time
                Console.Write("Enter start time (yyyy-MM-dd HH:mm): ");
                startTime = DateTime.Parse(Console.ReadLine());
            }

            // Ask for the end time
            Console.Write("Enter end time (yyyy-MM-dd HH:mm): ");
            DateTime? endTime = DateTime.TryParse(Console.ReadLine(), out DateTime parsedEndTime) ? parsedEndTime : (DateTime?)null;

            return (startTime, endTime);
        }


        public static int ChooseCar(ParkingAppDbContext OurDatabase, int userId)
        {
            // Get all cars registered by the user
            var cars = OurDatabase.Cars.Where(c => c.UserId == userId).ToList();

            Console.WriteLine("Your registered cars:");

            // Display the cars to the user
            foreach (var car in cars)
            {
                Console.WriteLine($"Car ID: {car.CarId} - {car.Model}");
            }

            // Ask the user to choose a car by CarId
            Console.Write("Select a car by entering its Car ID: ");
            int carChoice = int.Parse(Console.ReadLine());

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
                return ChooseCar(OurDatabase, userId); // Recursively ask again if the CarId is invalid
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
            string confirmation = Console.ReadLine().ToLower();

            return confirmation == "y";
        }

        public static void SaveActiveParkingSession(ParkingAppDbContext OurDatabase, int userId, int zoneId, DateTime startTime, DateTime? endTime, int carId)
        {
            // Create the ActiveParking object
            ActiveParking newSession = new ActiveParking
            {
                UserId = userId,
                ZoneId = zoneId,
                StartTime = startTime,
                EndTime = endTime,
                CarId = carId,
                Status = "Active" // Default to Active
            };

            // Add the new session to the database
            OurDatabase.ActiveParkings.Add(newSession);
            OurDatabase.SaveChanges(); // Save changes to the DB

            Console.WriteLine("Parking session started successfully!");
        }



        // End -- StartParking -- 


        static void OngoingParking()
        {
            Console.WriteLine("Showing ongoing parking sessions...");
            // Add logic for ongoing parking
        }

        static void ShowReceipts()
        {
            Console.WriteLine("Displaying receipts...");
            // Add logic for receipts
        }

        static void ListAllZones()
        {
            Console.WriteLine("Listing all parking zones...");
            // Add logic for listing zones
        }
        static void EditProfileOrDelete(int loggedInUserId)
        {
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Edit Profile");
            Console.WriteLine("2. Delete User");
            Console.Write("Enter your userEditProfileChoice (1 or 2): ");

            var userEditProfileChoice = Console.ReadLine();

            if (userEditProfileChoice == "1")
            {
                EditProfile(loggedInUserId);
            }
            else if (userEditProfileChoice == "2")
            {
                DeleteUser(loggedInUserId);
            }
            else
            {
                Console.WriteLine("Invalid userEditProfileChoice. Returning to the main menu.");
            }
        }

        static void EditProfile(int loggedInUserId)
        {
            Console.WriteLine("Editing loginUser profile...");
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var loggedInUser = ourDatabase.Users.SingleOrDefault(user => user.UserId == loggedInUserId);

                if (loggedInUser == null)
                {
                    Console.WriteLine("User not found. Please log in again.");
                    return;
                }

                Console.WriteLine($"Editing profile for {loggedInUser.FullName}");

                // Update FullName
                Console.WriteLine("Enter full name (leave blank to keep current):");
                var fullName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    loggedInUser.FullName = fullName;
                }

                // Update Email
                Console.WriteLine("Enter new email (leave blank to keep current): ");
                var email = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(email))
                {
                    if (ourDatabase.Users.Any(user => user.Email == email && user.UserId != loggedInUserId))
                    {
                        Console.WriteLine("Email already in use. Canceling profile update.");
                        return;
                    }
                    loggedInUser.Email = email;
                }

                // Update Address
                Console.WriteLine("Enter new address (leave blank to keep current): ");
                var address = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(address))
                {
                    loggedInUser.Adress = address;
                }

                // Update PhoneNumber
                Console.Write("Enter new phone number (leave blank to keep current): ");
                var phoneNumber = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    loggedInUser.PhoneNumber = phoneNumber;
                }

                // Update Password
                Console.Write("Enter new password (leave blank to keep current): ");
                var password = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(password))
                {
                    loggedInUser.Password = password;
                }

                // Save changes to the database
                try
                {
                    ourDatabase.SaveChanges();
                    Console.WriteLine("Profile updated successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating profile: {ex.Message}");
                }
            }
        }

        static void DeleteUser(int loggedInUserId)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var loggedInUser = ourDatabase.Users.SingleOrDefault(user => user.UserId == loggedInUserId);

                if (loggedInUser == null)
                {
                    Console.WriteLine("User not found. Please log in again.");
                    return;
                }

                Console.WriteLine($"Are you sure you want to delete your account, {loggedInUser.FullName}? This action cannot be undone.");
                Console.WriteLine("Type 'YES' to confirm, or 'CANCEL' to go back to the main menu.");
                Console.Write("Your choice: ");
                var confirmation = Console.ReadLine();

                if (confirmation?.ToUpper() == "YES")
                {
                    ourDatabase.Users.Remove(loggedInUser);

                    try
                    {
                        ourDatabase.SaveChanges();
                        Console.WriteLine("Account deleted successfully. Goodbye!");

                        loggedInUserId = 0; // Reset loginUser session
                        Console.WriteLine("You have been logged out.");

                        // Redirect to login menu
                        // Put the method for startmenu here
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting account: {ex.Message}");
                    }
                }
                else if (confirmation?.ToUpper() == "CANCEL")
                {
                    Console.WriteLine("Account deletion canceled. Returning to main menu...");
                    ShowMainMenu(); 
                }
                else
                {
                    Console.WriteLine("Invalid choice. Returning to main menu...");
                    ShowMainMenu(); 
                }
            }
        }
    }
}