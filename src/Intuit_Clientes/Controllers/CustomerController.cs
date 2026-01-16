using Intuit_Clientes.DTO;
using Intuit_Clientes.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Intuit_Clientes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CustomerDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerDTO>>> Get()
        {
            var customers = await _customerService.GetCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomerDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerDTO>>> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            return customer != null ? Ok(customer) : NotFound();
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int?))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int?>> Create(CustomerCreateDTO command)
        {
            var result = await _customerService.Create(command);
            return result != null && result > 0 ? CreatedAtAction(
                nameof(GetById),
                new { id = result },
                result
            ) : NotFound();
        }

        [HttpPut("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int?>> Update(int id, CustomerUpdateDTO command)
        {
            var result = await _customerService.Update(id, command);
            return result.Match<ActionResult>(
                rowsAffected => NoContent(),
                errors => BadRequest(new { errors })
            );
        }

        [HttpDelete("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int?>> Delete(int id)
        {
            var result = await _customerService.Delete(id);
            return result.Match<ActionResult>(
                rowsAffected => NoContent(),
                errors => BadRequest(new { errors })
            );
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CustomerDTO>))]
        public async Task<ActionResult<List<CustomerDTO>>> Search([FromQuery] string? param)
        {
            //Como mejora la haría paginada la busqueda, para mejorar la performance
            var customers = await _customerService.Search(param ?? string.Empty);
            return Ok(customers);
        }
    }
}