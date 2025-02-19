using System.Net;
using System.Net.Sockets;
using System.Text;
using TCP.Server.Commands;

namespace TCP.Server.Services;

public class TcpServer: ITcpServer
{
    private readonly ILogger<TcpServer> _logger;
    private readonly IConfiguration _config;
    private readonly TcpListener _listener;
    private bool _isRunning;
    
    public TcpServer(ILogger<TcpServer> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        var port = _config.GetValue<int>("Server:Port");
        _listener = new TcpListener(IPAddress.Any, port);
    }
    
    public async Task StartAsync()
    {
        _isRunning = true;
        _listener.Start();
        _logger.LogInformation("Server started on port {port}", _config["Server:Port"]);

        while (_isRunning)
        {
            var client = await _listener.AcceptTcpClientAsync();
            _ = HandleClientAsync(client);
        }
    }

    public Task StopAsync()
    {
        _isRunning = false;
        _listener.Stop();
        _logger.LogInformation("Server stopped.");
        return Task.CompletedTask;
    }
    
    private async Task HandleClientAsync(TcpClient client)
    {
        var clientEndPoint = client.Client.RemoteEndPoint?.ToString();
        _logger.LogInformation("Client connected from {clientEndPoint}", clientEndPoint);

        await using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        await using var writer = new StreamWriter(stream, Encoding.UTF8);
        writer.AutoFlush = true;

        while (_isRunning)
        {
            try
            {
                var request = await reader.ReadLineAsync();
                if (request == null) break;

                _logger.LogInformation("Received from {clientEndPoint}: {request}", clientEndPoint, request);
                var response = CommandProcessor.Process(request);
                await writer.WriteLineAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error handling client {clientEndPoint}: {ex}", clientEndPoint, ex);
                break;
            }
        }

        _logger.LogInformation("Client {clientEndPoint} disconnected.", clientEndPoint);
    }
}