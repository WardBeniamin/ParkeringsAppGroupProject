using ParkeringsApp.Models;

namespace ParkeringsApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var ourDatabase = new ParkingAppDbContext())
            {
                var allCars = ourDatabase.Cars.ToList();

                Console.WriteLine("nemo");
            }
        }
    }
}
