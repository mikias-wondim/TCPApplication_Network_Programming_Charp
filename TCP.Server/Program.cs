using Serilog;
using TCP.Server.Services;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) =>
    {
        services.AddSingleton<ITcpServer, TcpServer>();
    })
    .UseSerilog((context, logger) =>
    {
        logger.WriteTo.Console();
        logger.WriteTo.File(context.Configuration["Logging:FilePath"] ?? string.Empty);
    })
    .Build();

var server = host.Services.GetRequiredService<ITcpServer>();
await server.StartAsync();