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
                        User loggedInUser = Login();
                        if (loggedInUser != null)
                        {
                            ShowMainMenu(loggedInUser); // Call the main parking app menu if login is successful
                        }
                        else
                        {
                            Console.WriteLine("Login failed, press enter to try again");
                            Console.ReadLine();
                        }
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
            string password = Console.ReadLine();

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
                Console.WriteLine("=== Main Menu ===");
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
                        StartParking();
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
                        int loggedInUserId = 1;
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
        static void StartParking()
        {
            Console.WriteLine("Starting a parking session...");
            // Add logic for starting parking
        }

        static void OngoingParking()
        {
            Console.WriteLine("Showing ongoing parking sessions...");
            // Add logic for ongoing parking
        }

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
                    ShowMainMenu(loggedInUser);
                }
                else
                {
                    Console.WriteLine("Invalid choice. Returning to main menu...");
                    ShowMainMenu(loggedInUser);
                }
            }
        }
    }
}