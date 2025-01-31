using Microsoft.EntityFrameworkCore;
using ParkeringsApp.Models;
using Spectre.Console;

namespace ParkeringsApp.Classes 
{
    public class UserManager
    {
        public static void CreateAccount()
        {
            string fullName = InputHelper.GetValidatedInput("\nEnter Full Name: ");
            string email = InputHelper.GetValidatedEmail();
            string address = InputHelper.GetValidatedInput("Enter Address: ", false);
            string phoneNumber = InputHelper.GetValidatedInput("Enter Phone Number: ", false);
            string password = InputHelper.GetValidatedPassword();

            if (IsEmailTaken(email))
            {
                AnsiConsole.Markup("[red]Email is already in use. Please try a different one.[/]");
                return;
            }

            SaveUserToDatabase(fullName, email, address, phoneNumber, password);
            Console.WriteLine("User registered successfully!");
        }

        public static bool IsEmailTaken(string email)
        {
            using (var dbContext = new ParkingAppDbContext())
            {
                return dbContext.Users.Any(u => u.Email == email);
            }
        }

        public static void SaveUserToDatabase(string fullName, string email, string adress, string phoneNumber, string password)
        {
            User newUser = new User
            {
                FullName = fullName,
                Email = email,
                Adress = adress,
                PhoneNumber = phoneNumber,
                Password = password
            };

            using (var dbContext = new ParkingAppDbContext())
            {
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
            }
        }

        public static void EditProfileOrDelete(User loggedInUser)
        {
            Console.WriteLine("\nWhat would you like to do?");
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
                if (!UserExists(loggedInUser.UserId)) // After deletion, verify if user is deleted
                {
                    AnsiConsole.Markup("[red]You have been deleted. Exiting the menu...[/]");
                    return; // Exit or handle as needed
                }
            }
            else
            {
                AnsiConsole.Markup("[red]\nInvalid choice. Press any key to return to the main menu...[/]");
                Console.ReadKey();
            }
        }
        public static bool UserExists(int userId)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                // Check if a user exists with the given userId
                return ourDatabase.Users.Any(u => u.UserId == userId);
            }
        }
        public static void EditProfile(User loggedInUser)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var loggedInUserId = ourDatabase.Users.SingleOrDefault(user => user.UserId == loggedInUser.UserId);

                if (loggedInUserId == null)
                {
                    AnsiConsole.Markup("[red]\nUser not found. Please log in again.[/]");
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
                        AnsiConsole.Markup("[red]Email already in use. Canceling profile update.[/]");
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
                    Console.WriteLine("\nProfile updated successfully! Press any key to return to the main menu...");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    AnsiConsole.Markup($"[red]\nError updating profile: {ex.Message}[/]");
                    Console.WriteLine("\nPress any key to return to the main menu...");
                    Console.ReadKey();
                }
            }
        }
        public static void DeleteUser(int loggedInUser)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var userToDelete = ourDatabase.Users
                    .Include(u => u.Cars) // Include user's cars
                    .Include(u => u.Payments) // Include payment methods (many-to-many)
                    .SingleOrDefault(u => u.UserId == loggedInUser);

                if (userToDelete == null)
                {
                    AnsiConsole.Markup("[red]User not found. Please log in again. Press any key to return to login menu.[/]");
                    Console.ReadKey();
                    return;
                }

                while (true) // Loop until user provides a valid answer
                {
                    Console.WriteLine($"\nAre you sure you want to delete your account, {userToDelete.FullName}? This action cannot be undone.");
                    Console.WriteLine("Type 'YES' to confirm, or 'NO' to go back to the main menu.");
                    Console.Write("\nYour choice: ");
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
                        userToDelete.Payments.Clear();

                        // Step 4: Delete the user
                        ourDatabase.Users.Remove(userToDelete);

                        // Save all changes to the database
                        try
                        {
                            ourDatabase.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.Markup($"[red]\nError deleting user: {ex.Message}[/]");
                        }

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
                        AnsiConsole.Markup("[red]\nInvalid choice. Please type 'YES' or 'NO'.[/]");
                    }
                }
            }
        }
    }
}