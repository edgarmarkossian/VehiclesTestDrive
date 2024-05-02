using System.Text;

using CustomersApi.Data;
using CustomersApi.Interfaces;
using CustomersApi.Models;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using RabbitMQ.Client;

namespace CustomersApi.Services
{
    public class CustomerService : ICustomer
    {
        private ApiDbContext _dbContext;
        private readonly Serilog.ILogger _logger;
        private readonly string _rabbitMQQueue = "rabbitorderqueue";
        private readonly string _rabbitConnection = "localhost";


        public CustomerService(Serilog.ILogger logger)
        {
            _dbContext = new ApiDbContext();
            _logger = logger;
        }

        public async Task AddCustomer(Customer customer)
        {
            try
            {
                var vehicleInDb = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == customer.VehicleId);
                if (vehicleInDb == null)
                {
                    await _dbContext.Vehicles.AddAsync(customer.Vehicle);
                    await _dbContext.SaveChangesAsync();
                }
                customer.Vehicle = null;
                await _dbContext.Customers.AddAsync(customer);
                await _dbContext.SaveChangesAsync();

                await PublishMessageToQueue(customer, _rabbitMQQueue);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "CustomerService AddCustomer error!");
                throw new ArgumentNullException();
            }
        }

        public async Task PublishMessageToQueue(object messageData, string queueName)
        {
            var factory = new ConnectionFactory { HostName = _rabbitConnection };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            var message = JsonConvert.SerializeObject(messageData);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
        }
    }
}

