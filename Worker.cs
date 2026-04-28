using Azure.Messaging.ServiceBus;
using MachineData;
using MachineData.Data;


namespace MachineWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ServiceBusProcessor _busProcessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly CircuitBreaker _circuitBreaker;

    public Worker(ILogger<Worker> logger, ServiceBusProcessor busProcessor, IServiceScopeFactory serviceScopeFactory, CircuitBreaker circuitBreaker)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _circuitBreaker = circuitBreaker;
        _busProcessor = busProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
     
        _busProcessor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Service Bus error: {ErrorMessage}", args.Exception.Message);
            return Task.CompletedTask;
        };

        
        _busProcessor.ProcessMessageAsync += async args =>
        {
            
            string body = args.Message.Body.ToString();
            _logger.LogInformation("Received message: {MessageBody}", body);

            var input = System.Text.Json.JsonSerializer.Deserialize<MachineInputs>(body);

            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (_circuitBreaker.IsOpen())
            {
                _logger.LogWarning("Circuit is OPEN. Skipping DB save.");
                return; 
            }
            try
            {
                db.MachineInputs.Add(input);
               
                await db.SaveChangesAsync();

                await args.CompleteMessageAsync(args.Message);
                _circuitBreaker.RecordSuccess();
                _logger.LogInformation("Saved to Oracle: {MessageBody}", body);
            }
            catch (Exception ex)
            {
                _circuitBreaker.RecordFailure();
                _logger.LogError(ex, "Error saving to Oracle (DeliveryCount={DeliveryCount}): {MessageBody}", args.Message.DeliveryCount, body);
                throw;
            }

        };

       
        await _busProcessor.StartProcessingAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}