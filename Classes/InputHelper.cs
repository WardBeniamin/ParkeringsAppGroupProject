using Spectre.Console;

public class InputHelper
{
    public static string GetValidatedInput(string prompt, bool required = true)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()!.Trim();

            if (!required && string.IsNullOrEmpty(input))
                return null!;

            if (!string.IsNullOrEmpty(input))
                return input;

            AnsiConsole.Markup("[red]Input cannot be empty. Please try again.[/]");
        }
    }

    public static string GetValidatedEmail()
    {
        while (true)
        {
            Console.Write("Enter Email: ");
            string email = Console.ReadLine()!.Trim();

            if (!string.IsNullOrEmpty(email) && email.Contains("@") && email.Contains("."))
                return email;

            AnsiConsole.Markup("[red]Invalid email format. Please try again.[/]");
        }
    }

    public static string GetValidatedPassword()
    {
        while (true)
        {
            Console.Write("Enter Password (at least 6 characters): ");
            string password = Console.ReadLine()!.Trim();

            if (!string.IsNullOrEmpty(password) && password.Length >= 6)
                return password;

            AnsiConsole.Markup("[red]Password must be at least 6 characters long.[/]");
        }
    }
}