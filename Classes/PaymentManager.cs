using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ParkeringsApp.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkeringsApp.Classes
{
    public class PaymentManager
    {

        public static void RegisterPaymentMethod(int userId) 
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var paymentMethods = ourDatabase.PaymentMethods.ToList();
                Console.WriteLine("\nAvailable payment methods:");
                for (int i = 0; i < paymentMethods.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {paymentMethods[i].PaymentType} (ID: {paymentMethods[i].PaymentId})");
                }
                Console.Write("\nEnter the Payment ID from the list above to add to the user: ");

                int paymentId;
                while (true)
                {
                    string input = Console.ReadLine()!;

                    // Try to parse the input as an integer and validate against the available payment IDs
                    if (int.TryParse(input, out paymentId) && paymentMethods.Any(pm => pm.PaymentId == paymentId))
                    {
                        // Proceed to add the payment method to the user
                        CreatePaymentMethodToUser(userId, paymentId);
                        Console.WriteLine("\nPress any key to return to main menu");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid Payment ID from the list.");
                    }
                }
            }
        }

        //CRUD Methods
        public static void CreatePaymentMethodToUser(int userId, int paymentId)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {

                var user = ourDatabase.Users.Include(u => u.Payments)
                                .FirstOrDefault(u => u.UserId == userId);
                var paymentMethod = ourDatabase.PaymentMethods.Find(paymentId);

                if (user != null && paymentMethod != null)
                {
                    user.Payments.Add(paymentMethod);
                    ourDatabase.SaveChanges();
                    Console.WriteLine("Payment method added successfully!");
                }
                else
                {
                    Console.WriteLine("User or Payment Method not found.");
                }
            }
        }

        public static void DeletePaymentMethodFromUser(int userId, int paymentId)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var user = ourDatabase.Users.Include(u => u.Payments)
                                        .FirstOrDefault(u => u.UserId == userId);

                if (user != null)
                {
                    var paymentToRemove = user.Payments.FirstOrDefault(p => p.PaymentId == paymentId);
                    if (paymentToRemove != null)
                    {
                        user.Payments.Remove(paymentToRemove);
                        ourDatabase.SaveChanges();
                        Console.WriteLine("Payment method removed successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Payment method not found for this user.");
                    }
                }
            }
        }

        public static void UpdateUserPaymentMethod(int userId, int oldPaymentId, int newPaymentId)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var user = ourDatabase.Users.Include(u => u.Payments)
                                        .FirstOrDefault(u => u.UserId == userId);

                var oldPayment = ourDatabase.PaymentMethods.Find(oldPaymentId);
                var newPayment = ourDatabase.PaymentMethods.Find(newPaymentId);

                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return;
                }
                if (oldPayment == null || newPayment == null)
                {
                    Console.WriteLine("One of the payment methods was not found.");
                    return;
                }

                // Check if user actually has the old payment method
                if (user.Payments.Contains(oldPayment))
                {
                    user.Payments.Remove(oldPayment); // Remove old payment method
                    user.Payments.Add(newPayment); // Add new payment method
                    ourDatabase.SaveChanges();
                    Console.WriteLine("Payment method updated successfully!");
                }
                else
                {
                    Console.WriteLine("The user does not have this payment method.");
                }
            }
        }

        public static void ListUserPaymentMethods(int userId)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var user = ourDatabase.Users.Include(u => u.Payments)
                                        .FirstOrDefault(u => u.UserId == userId);

                if (user != null)
                {
                    Console.WriteLine($"Payment methods for {user.FullName}:");
                    foreach (var method in user.Payments)
                    {
                        Console.WriteLine($"- {method.PaymentType}");
                    }
                }
                else
                {
                    Console.WriteLine("User not found.");
                }
            }
        }
    }
}
