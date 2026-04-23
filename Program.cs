using MachineData.Data;
using Microsoft.EntityFrameworkCore;
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

string serviceBusConnectionString = builder.Configuration["ServiceBus:ConnectionString"];
string queueName = builder.Configuration.GetValue<string>("ServiceBus:QueueName");
var serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
var sender = serviceBusClient.CreateSender(queueName);
builder.Services.AddSingleton(serviceBusClient);
builder.Services.AddSingleton(sender);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
