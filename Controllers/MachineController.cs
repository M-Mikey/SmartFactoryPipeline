using Azure.Messaging.ServiceBus;
using MachineData.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MachineData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineController : ControllerBase
    {
        private readonly ILogger<MachineController> _logger;
        private readonly AppDbContext _context;
        private readonly ServiceBusSender _serviceBusSender;

        public MachineController(ILogger<MachineController> logger, AppDbContext context, ServiceBusSender serviceBusSender)
        {
            _logger = logger;
            _context = context;
            _serviceBusSender = serviceBusSender;
        }

        [HttpPost(Name = "MachinData")]
        public async Task<IActionResult> RecieveData([FromBody] MachineInputs machineInputs)
        {
            if(machineInputs == null)
            {
                return BadRequest(new { message = "Invalid data" });
            }
            //_logger.LogInformation($"Received data from machine {machineInputs.machineId} at {machineInputs.timestamp} with temperature {machineInputs.temperature}");

            //_context.MachineInputs.Add(machineInputs);
            //_context.SaveChanges();

            string messageBody= JsonSerializer.Serialize(machineInputs);

            ServiceBusMessage message = new ServiceBusMessage(messageBody);
            await _serviceBusSender.SendMessageAsync(message);
            return Ok(new { message="Data received successfully", data=machineInputs });
        }
    }
}
