using Microsoft.AspNetCore.Mvc;
using ServerASP.BL.Dtos;
using ServerASP.BL.Repositories;
using ServerASP.Infrastructure.DbModels;
using System.Net;
using System.Reflection.Metadata.Ecma335;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServerASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeRepository _employeeRepository;
        //private readonly ILogger _logger;

        public EmployeeController(EmployeeRepository repository/*, ILogger logger*/)
        {
            _employeeRepository = repository;
            //_logger = logger;
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<EmployeeDto>> GetById(int employeeId)
        {
            try
            {
                return await _employeeRepository.GetById(employeeId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllEmployees")]
        public async Task<ActionResult<List<EmployeeDto>>> GetEmployees()
        {
            try
            {
                return await _employeeRepository.GetEmployees();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateEmployee")]
        public async Task<ActionResult> CreateEmployee(EmployeeDto model)
        {
            try
            {
                await _employeeRepository.CreateEmployee(model);

                return Ok();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteEmployee")]
        public async Task<ActionResult> DeleteEmployee(int employeeId)
        {
            try
            {
                await _employeeRepository.DeleteEmployee(employeeId);

                return Ok();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateEmployee")]
        public async Task<ActionResult> UpdateEmployee(EmployeeDto model)
        {
            try
            {
                await _employeeRepository.UpdateEmployee(model);

                return Ok();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateEmployeeRange")]
        public async Task<ActionResult> UpdateEmployeeRange(List<EmployeeDto> models)
        {
            try
            {
                await _employeeRepository.UpdateEmployeeRange(models);

                return Ok();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }
        }
    }
}
