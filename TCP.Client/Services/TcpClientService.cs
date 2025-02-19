using System.Net.Sockets;
using System.Text;

namespace TCP.Client.Services;

public class TcpClientService(ILogger<TcpClientService> logger, IConfiguration config) : ITcpClientService
{
    private TcpClient? _client;
    private StreamWriter? _writer;
    private StreamReader? _reader;
    
    private async Task<bool> ConnectAsync()
    {
        try
        {
            var serverIp = config["Client:ServerIp"] ?? "127.0.0.1";
            var serverPort = config.GetValue<int>("Client:ServerPort");

            _client = new TcpClient();
            await _client.ConnectAsync(serverIp, serverPort);
            var stream = _client.GetStream();
            
            _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            _reader = new StreamReader(stream, Encoding.UTF8);
            
            logger.LogInformation("Connected to {serverIp}:{serverPort}", serverIp, serverPort);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to connect: {ex}", ex);
            return false;
        }
    }

    public async Task SendCommandAsync(string command)
    {
        try
        {
            if (_client is not { Connected: true })
            {
                var connected = await ConnectAsync();
                if (!connected) return;
            }

            await SendAndReceiveAsync(command);
        }
        catch (Exception ex)
        {
            logger.LogError("Error: {ex}", ex);
        }
        finally
        {
            CloseConnection();
        }
    }

    private async Task SendAndReceiveAsync(string command)
    {
        if (_writer == null || _reader == null)
        {
            logger.LogError("Connection is not established.");
            return;
        }

        await _writer.WriteLineAsync(command);
        var response = await _reader.ReadLineAsync();
        logger.LogInformation("Server Response: {response}", response);
    }

    private void CloseConnection()
    {
        _reader?.Dispose();
        _writer?.Dispose();
        _client?.Close();
        _client = null;
        logger.LogInformation("Connection closed.");
    }
}
