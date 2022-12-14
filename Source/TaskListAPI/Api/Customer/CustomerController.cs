using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace TaskListAPI
{
    [Route("customers")]
    public class CustomerController : BaseProtectedController
    {
        private static readonly ConcurrentDictionary<int, CustomerModel> _customers = new ();
        private readonly ILogger<CustomerController> _logger;
        private readonly CustomerConverter _customerConverter = new();

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        [Description("Get a list of all customers.")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<CustomerDto>),
            Description = "Customers successfully returned.")]
        [HttpGet]
        public IEnumerable<CustomerDto> GetAll()
        {
            var result = _customers.Values.Select(customer => _customerConverter.ModelToDto(customer));
            _logger.LogInformation(LogEvents.CustomersRetrieved,
                ResTaskListAPI.All_customers_retrieved_);
            return result;
        }
        
        [Description("Get customer by id.")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CustomerDto), Description = "Customer with given id found.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "Customer with given id not found.")]
        [HttpGet("{id:int}")]
        public CustomerDto Get([Required] [Description("The id of the customer")] int id)
        {
            if (_customers.TryGetValue(id, out var value))
            {
                var result = _customerConverter.ModelToDto(value);
                _logger.LogInformation(LogEvents.CustomerWithIdRetrieved,
                    string.Format(ResTaskListAPI.Customer_with_id__0__retrieved_, id));
                return result;
            }
            var msg = string.Format(ResTaskListAPI.Customer_with_id__0__not_found_, id);
            _logger.LogWarning(LogEvents.CustomerWithIdNotFound, msg);
            throw new ResponseException(HttpStatusCode.NotFound, msg);
        }

        [Description("Create a new customer.")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(CustomerDto), Description = "Customer successfully created.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "Failed to create customer.")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "Failed to create customer.")]
        [HttpPost]
        public IActionResult Post([Required] [FromBody] EditCustomerDto customerDto)
        {
            return UpdateConditional(() =>
            {
                var customer = new CustomerModel();
                _customerConverter.DtoToModel(customerDto, customer, _customers.Any() ? _customers.Keys.Max() + 1 : 1);
                if (_customers.Values.Select(v => v.Name).Contains(customer.Name))
                {
                    return BadRequest($"{customer.Name} already exists");
                }
                if (_customers.TryAdd(customer.Id, customer))
                {

                    var result = _customerConverter.ModelToDto(customer);
                    _logger.LogInformation(LogEvents.CustomerCreated,
                        string.Format(ResTaskListAPI.Created_new_customer_with_id__0__, result.Id));
                    return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
                }

                return Conflict("Failed to create customer.");
            }, _logger);
        }

        [Description("Delete an existing customer by id.")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "Customer successfully removed.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "Failed to delete customer.")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "Failed to delete customer.")]
        [HttpDelete("{id:int}")]
        public IActionResult Delete([Required] [Description("The id of the customer")] int id)
        {
            return UpdateConditional(() =>
            {
                if (_customers.TryRemove(id, out var value))
                {
                    _logger.LogInformation(LogEvents.CustomerWithIdRemoved,
                        string.Format(ResTaskListAPI.Removed_customer_with_id__0__, id));
                    return new OkResult();
                }
                return NotFound("Failed to delete customer.");
            }, _logger);
        }
    }
}