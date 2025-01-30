using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ParkeringsApp.Models;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Text;

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
                        CreateAccount();
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

        public static void CreateAccount()
        {
            string fullName = GetValidatedInput("Enter Full Name: ");
            string email = GetValidatedEmail();
            string address = GetValidatedInput("Enter Address (optional): ", false);
            string phoneNumber = GetValidatedInput("Enter Phone Number (optional): ", false);
            string password = GetValidatedPassword();

            if (IsEmailTaken(email))
            {
                Console.WriteLine("Email is already in use. Please try a different one.");
                return;
            }

            SaveUserToDatabase(fullName, email, address, phoneNumber, password);
            Console.WriteLine("User registered successfully!");
        }

        private static string GetValidatedInput(string prompt, bool required = true)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()!.Trim();

                if (!required && string.IsNullOrEmpty(input))
                    return null;

                if (!string.IsNullOrEmpty(input))
                    return input;

                Console.WriteLine("Input cannot be empty. Please try again.");
            }
        }

        private static string GetValidatedEmail()
        {
            while (true)
            {
                Console.Write("Enter Email: ");
                string email = Console.ReadLine()!.Trim();
                       
                if (!string.IsNullOrEmpty(email) && email.Contains("@") && email.Contains("."))
                    return email;

                Console.WriteLine("Invalid email format. Please try again.");
            }
        }

        private static string GetValidatedPassword()
        {
            while (true)
            {
                Console.Write("Enter Password (at least 6 characters): ");
                string password = Console.ReadLine()!.Trim();

                if (!string.IsNullOrEmpty(password) && password.Length >= 6)
                    return password;

                Console.WriteLine("Password must be at least 6 characters long.");
            }
        }

        private static bool IsEmailTaken(string email)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                return ourDatabase.Users.Any(u => u.Email == email);
            }
                
        }

        private static void SaveUserToDatabase(string fullName, string email, string adress, string phoneNumber, string password)
        {
            User newUser = new User
            {
                FullName = fullName,
                Email = email,
                Adress = adress,
                PhoneNumber = phoneNumber,
                Password = password
            };
            using (var ourDatabase = new ParkingAppDbContext())
            {
                ourDatabase.Users.Add(newUser);
                ourDatabase.SaveChanges();
            }
            
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
                        StartParking();
                        break;
                    case "2":
                        OngoingParking();
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