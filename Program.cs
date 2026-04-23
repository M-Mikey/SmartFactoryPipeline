using Azure.Messaging.ServiceBus;
using MachineData.Data;
using MachineWorker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

string serviceBusConnectionString = builder.Configuration["ServiceBus:ConnectionString"];
string queueName = builder.Configuration["ServiceBus:QueueName"];

var serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
var processor = serviceBusClient.CreateProcessor(queueName);

builder.Services.AddSingleton(serviceBusClient);
builder.Services.AddSingleton(processor);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();