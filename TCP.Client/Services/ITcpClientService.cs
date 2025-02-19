namespace TCP.Client.Services;

public interface ITcpClientService
{
    Task SendCommandAsync(string command);
}