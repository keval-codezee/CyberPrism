using CyberPrism.Core.Models;
using CyberPrism.Server.Constants;
using CyberPrism.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberPrism.Server.Controllers
{
    [ApiController]
    [Route(ServerConstants.RouteApiController)]
    public class DashboardController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        public DashboardController(DatabaseService databaseService) => _databaseService = databaseService;

        // Responsibility: Get real-time dashboard data (e.g., efficiency, yield).
        // Logic: Calls _databaseService.GetDashboardDataAsync() and returns OK with the data.
		[HttpGet]
        public async Task<ActionResult<DashboardData>> Get()
        {
            var data = await _databaseService.GetDashboardDataAsync();
            return Ok(data);      // The data object returned to the client
			// When the client (Web or WPF) receives this 200 OK, it parses the JSON and displays it.
		}
    }

    [ApiController]
    [Route(ServerConstants.RouteApiController)]
    public class ProductionController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        public ProductionController(DatabaseService databaseService) => _databaseService = databaseService;

        [HttpGet]
        public async Task<ActionResult<List<ProductionJob>>> Get()
        {
            var jobs = await _databaseService.GetProductionJobsAsync();
            return Ok(jobs);
        }

		// [HttpPost]: Create a new task. Includes business validation in the API layer (ID uniqueness check).
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductionJob job)
        {
            // Check if job already exists
            var existingJob = await _databaseService.GetProductionJobByIdAsync(job.JobId);
            if (existingJob != null)
            {
                return BadRequest(new { Message = string.Format(ServerConstants.MsgJobExists, job.JobId) });
            }

            var createdJob = await _databaseService.CreateProductionJobAsync(job);
            return Ok(new { Message = string.Format(ServerConstants.MsgJobCreated, job.JobId), JobId = createdJob.JobId });
        }
    }

    [ApiController]
    [Route(ServerConstants.RouteApiController)]
    public class AnalyticsController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        public AnalyticsController(DatabaseService databaseService) => _databaseService = databaseService;

        [HttpGet]
        public async Task<ActionResult<List<SensorDataPoint>>> Get()
        {
            var data = await _databaseService.GetAnalyticsDataAsync(10);
            return Ok(data);
        }
    }

    [ApiController]
    [Route(ServerConstants.RouteApiController)]
    public class SettingsController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        public SettingsController(DatabaseService databaseService) => _databaseService = databaseService;

        [HttpGet]
        public async Task<ActionResult<AppSettings>> Get()
        {
            var settings = await _databaseService.GetSettingsAsync();
            return Ok(settings);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AppSettings settings)
        {
            await _databaseService.UpdateSettingsAsync(settings);
            return Ok(new { Message = ServerConstants.MsgSettingsUpdated });
        }
    }
}

// Summary:
// IndustrialControllers.cs acts as the system's dispatcher. 
// It handles HTTP requests and responses, delegating actual data processing to DatabaseService.
// This implements a clean "Separation of Concerns" architecture.
