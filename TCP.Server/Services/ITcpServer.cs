namespace TCP.Server.Services;

public interface ITcpServer
{
    Task StartAsync();
    Task StopAsync();
}