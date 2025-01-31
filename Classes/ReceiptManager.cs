using Microsoft.EntityFrameworkCore;
using ParkeringsApp.Models;
using Spectre.Console;


namespace ParkeringsApp.Classes
{
    public class ReceiptManager
    {
        public static void ShowReceipts(int loggedInUser)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                // Fetch all receipts for the logged-in user
                var userReceipts = ourDatabase.Receipts
                    .Include(r => r.Car) // Include car information
                    .Include(r => r.Zone) // Include zone information
                    .Include(r => r.Payment) // Include payment method
                    .Where(r => r.UserId == loggedInUser)
                    .ToList();

                if (userReceipts.Any())
                {
                    Console.WriteLine("\nYour Receipts");

                    // Create a table
                    var table = new Table()
                        .AddColumn("Receipt ID")
                        .AddColumn("Car")
                        .AddColumn("Zone")
                        .AddColumn("Payment Method")
                        .AddColumn("Start Time")
                        .AddColumn("End Time")
                        .AddColumn("Total Amount");

                    // Add rows to the table for each receipt
                    foreach (var receipt in userReceipts)
                    {
                        table.AddRow(
                            receipt.TransactionId.ToString(),
                            $"{receipt.Car.PlateNumber} ({receipt.Car.Model})",
                            $"{receipt.Zone.Adress} (Fee: {receipt.Zone.Fee} SEK/hour)",
                            receipt.Payment.PaymentType,
                            receipt.StartTime.ToString(),
                            receipt.EndTime.ToString()!,
                            $"{receipt.Amount} SEK"
                        );
                    }

                    // Render the table
                    AnsiConsole.Write(table);
                }
                else
                {
                    Console.WriteLine("\nNo receipts found for your account.");
                }
            }

            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
        }
    }
}