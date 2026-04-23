using Azure.Messaging.ServiceBus;
using MachineData;
using MachineData.Data;


namespace MachineWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ServiceBusProcessor _busProcessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Worker(ILogger<Worker> logger, ServiceBusProcessor busProcessor, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _busProcessor = busProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ✅ Error handler FIRST — outside everything
        _busProcessor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Service Bus error: {ErrorMessage}", args.Exception.Message);
            return Task.CompletedTask;
        };

        // ✅ Message handler SECOND
        _busProcessor.ProcessMessageAsync += async args =>
        {
            string body = args.Message.Body.ToString();
            _logger.LogInformation("Received message: {MessageBody}", body);

            var input = System.Text.Json.JsonSerializer.Deserialize<MachineInputs>(body);

            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.MachineInputs.Add(input);
            await db.SaveChangesAsync();

            await args.CompleteMessageAsync(args.Message);
            _logger.LogInformation("Saved to Oracle: {MessageBody}", body);
        };

        // ✅ Start processing LAST
        await _busProcessor.StartProcessingAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}