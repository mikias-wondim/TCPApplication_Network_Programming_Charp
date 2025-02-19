namespace TCP.Server.Commands;

public static class CommandProcessor
{
    public static string Process(string command)
    {
        return command switch
        {
            "GET_TEMP" => TempratureCommands.GetTempratureReading(),
            "GET_STATUS" => StatusCommands.GetStatusReading(),
            _ => "ERROR: Unknown command"
        };
    }
}