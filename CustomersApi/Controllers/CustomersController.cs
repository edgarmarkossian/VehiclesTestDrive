using CustomersApi.Interfaces;
using CustomersApi.Models;

using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomersApi.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private ICustomer _customerService;

        public CustomersController(ICustomer customerService)
        {
            _customerService = customerService;
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody]Customer customer)
        {
            await _customerService.AddCustomer(customer);
        }
    }
}