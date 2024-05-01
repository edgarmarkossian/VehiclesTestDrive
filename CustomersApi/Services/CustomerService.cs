using CustomersApi.Data;
using CustomersApi.Interfaces;
using CustomersApi.Models;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace CustomersApi.Services
{
    public class CustomerService : ICustomer
	{
        private ApiDbContext _dbContext;
        private readonly Serilog.ILogger _logger;

		public CustomerService(Serilog.ILogger logger)
		{
            _dbContext = new ApiDbContext();
            _logger = logger;
		}

        public async Task AddCustomer(Customer customer)
        {
            try
            {
                var customerJson = JsonConvert.SerializeObject(customer);

                var vehicleInDb = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == customer.VehicleId);
                if (vehicleInDb == null)
                {
                    await _dbContext.Vehicles.AddAsync(customer.Vehicle);
                    await _dbContext.SaveChangesAsync();
                }
                customer.Vehicle = null;
                await _dbContext.Customers.AddAsync(customer);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CustomerService AddCustomer error!");
                throw new ArgumentNullException();
            }
        }
    }
}

