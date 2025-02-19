using Serilog;
using TCP.Client.Services;
using TCP.Client;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddSingleton<ITcpClientService, TcpClientService>();
        services.AddHostedService<TcpClientWorker>();  
    }).UseSerilog((context, logger) =>
    {
        logger.WriteTo.Console();
        logger.WriteTo.File(context.Configuration["Logging:FilePath"] ?? string.Empty);
    })
    .Build();

await host.RunAsync();