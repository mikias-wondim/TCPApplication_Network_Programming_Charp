namespace TCP.Server.Commands;

public static class TempratureCommands
{
    public static string GetTempratureReading() => $"Temperature reading:  {new Random().Next(20, 30)}°C";
}