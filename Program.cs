using Microsoft.EntityFrameworkCore;
using ParkeringsApp.Models;
using ParkeringsApp.Classes;
using Spectre.Console;
using Figgle;

namespace ParkeringsApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool appRunning = true;

            while (appRunning)
            {
                string menuSelection = Menus.ShowHeaderAndMenu("=== Welcome to the Parking App ===",
                    new[] { "Log in", "Create an account", "Exit" });

                switch (menuSelection)
                {
                    case "Log in":
                        User loggedInUser = Menus.Login();
                        if (loggedInUser != null)
                        {
                            Menus.ShowMainMenu(loggedInUser);
                        }
                        else
                        {
                            AnsiConsole.Markup("\n[red]Login failed. Press Enter to try again...[/]");
                            Console.ReadLine();
                        }
                        break;

                    case "Create an account":
                        UserManager.CreateAccount();
                        break;

                    case "Exit":
                        Console.WriteLine("\nExiting the app. Goodbye!");
                        appRunning = false;
                        break;
                }
            }
        }
    }
}