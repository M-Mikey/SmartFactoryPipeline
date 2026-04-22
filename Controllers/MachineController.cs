using MachineData.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MachineData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineController : ControllerBase
    {
        private readonly ILogger<MachineController> _logger;
        private readonly AppDbContext _context;

        public MachineController(ILogger<MachineController> logger,AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost(Name = "MachinData")]
        public IActionResult RecieveData([FromBody] MachineInputs machineInputs)
        {
            if(machineInputs == null)
            {
                return BadRequest(new { message = "Invalid data" });
            }
            _logger.LogInformation($"Received data from machine {machineInputs.machineId} at {machineInputs.timestamp} with temperature {machineInputs.temperature}");

            _context.MachineInputs.Add(machineInputs);
            _context.SaveChanges();
            return Ok(new { message="Data received successfully", data=machineInputs });
        }
    }
}
