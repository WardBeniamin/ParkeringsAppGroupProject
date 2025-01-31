public class TimeHelper
{
    public static (DateTime startTime, DateTime? endTime) SetTimes()
    {
        DateTime startTime;

        Console.Write("\nDo you want to use the current time as the start time? (y/n): ");
        string useCurrentTime = Console.ReadLine()?.ToLower()!;

        if (useCurrentTime == "y")
        {
            startTime = DateTime.Now;
            Console.WriteLine($"Using current time: {startTime:yyyy-MM-dd HH:mm}");
        }
        else
        {
            Console.Write("Enter start time (yyyy-MM-dd HH:mm): ");
            startTime = DateTime.Parse(Console.ReadLine()!);
        }

        Console.Write("\nEnter end time (yyyy-MM-dd HH:mm): ");
        DateTime? endTime = DateTime.TryParse(Console.ReadLine(), out DateTime parsedEndTime) ? parsedEndTime : (DateTime?)null;

        return (startTime, endTime);
    }
}