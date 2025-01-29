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