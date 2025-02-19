using TCP.Client.Services;

namespace TCP.Client
{
    public class TcpClientWorker(ITcpClientService clientService, ILogger<TcpClientWorker> logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("TCP Client Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await clientService.SendCommandAsync("GET_TEMP");
                    await clientService.SendCommandAsync("GET_STATUS");

                    logger.LogInformation("Waiting for the next cycle...");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("Shutdown requested.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in client worker loop.");
                }
            }

            logger.LogInformation("TCP Client Worker stopped.");
        }
    }
}