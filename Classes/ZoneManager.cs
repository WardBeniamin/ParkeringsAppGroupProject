using ParkeringsApp.Models;
using Spectre.Console;

namespace ParkeringsApp.Classes
{
    public class ZoneManager
    {
        public static void ListAllZones()
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var zones = ourDatabase.Zones.ToList();

                if (zones.Any())
                {
                    Console.WriteLine("\nAvailable Parking Zones");

                    // Create a table
                    var table = new Table()
                        .AddColumn("Zone ID")
                        .AddColumn("Address")
                        .AddColumn("Fee (SEK/hour)");

                    // Add rows to the table
                    foreach (var zone in zones)
                    {
                        table.AddRow(zone.ZoneId.ToString(), zone.Adress!, $"{zone.Fee} SEK/hour");
                    }

                    // Render the table
                    AnsiConsole.Write(table);
                }
                else
                {
                    Console.WriteLine("\nNo parking zones found.");
                }
            }

        }
    }
}