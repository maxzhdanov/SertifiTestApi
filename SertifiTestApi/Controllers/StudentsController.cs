using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SertifiTestApi.Models;
using SertifiTestApi.Services;

namespace SertifiTestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ILogger<StudentsController> _logger;
        private readonly IAggregateService _aggregateService;

        public StudentsController(ILogger<StudentsController> logger, IAggregateService aggregateService)
        {
            _logger = logger;
            _aggregateService = aggregateService;
        }

        [HttpGet("GetAggregate")]
        [ProducesResponseType(typeof(StudentsAggregate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentsAggregate>> GetAggregate()
        {
            try
            {
                var result = await _aggregateService.GetAggregate();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPut("SubmitAggregate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> SubmitAggregate(StudentsAggregateSubmitRequest request)
        {
            try
            {
                var result = await _aggregateService.SubmitAggregate(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(400, ex.Message);
            }
        }
    }
}
