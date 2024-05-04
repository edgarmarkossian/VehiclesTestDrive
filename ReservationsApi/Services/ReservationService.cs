using System.Net;
using System.Net.Mail;
using System.Text;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using ReservationsApi.Data;
using ReservationsApi.Interfaces;
using ReservationsApi.Models;

namespace ReservationsApi.Services
{
    public class ReservationService : IReservation
    {
        private ApiDbContext _dbContext;
        private Serilog.ILogger _logger;
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly string _email = "email@gmail.com";
        private readonly string _password = "password";
        private readonly string _rabbitMQQueue = "rabbitorderqueue";
        private readonly string _rabbitConnection = "localhost";

        public ReservationService(Serilog.ILogger logger, ApiDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Reservation>> GetReservations()
        {
            try
            {

                var factory = new ConnectionFactory { HostName = _rabbitConnection };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: _rabbitMQQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                // Event handler for received messages
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var reservation = JsonConvert.DeserializeObject<Reservation>(message);

                        // Process the message (e.g., save to database)
                        AddReservationAsync(reservation);

                        // Acknowledge message delivery
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions, log errors, and reject message (optional)
                        _logger.Error(ex, "Error processing message");

                        // Reject message and do not requeue
                        try
                        {
                            channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: false);
                        }
                        catch (Exception rejectEx)
                        {
                            // Log or handle the rejection error
                            _logger.Error(rejectEx, "Error rejecting message");
                        }
                    }
                };

                // Start consuming messages
                channel.BasicConsume(queue: _rabbitMQQueue, autoAck: false, consumer: consumer);

                var reservations = await _dbContext.Reservations.ToListAsync();
                return reservations;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ReservationService GetReservations error!");
                throw new ArgumentNullException();
            }
        }

        public async Task UpdateMailStatus(int id)
        {
            try
            {
                var reservationResult = await _dbContext.Reservations.FindAsync(id);
                if (reservationResult != null && reservationResult.IsMailSent == false)
                {
                    //MailMessage mail = new(_email, _email);
                    //mail.Subject = "VehicleTestDrive";
                    //mail.Body = "Your test drive is reserved";

                    //var smtpClient = new SmtpClient(_smtpHost)
                    //{
                    //    Port = 587,
                    //    Credentials = new NetworkCredential(_email, _password),
                    //    EnableSsl = true,
                    //    UseDefaultCredentials = false
                    //};

                    //smtpClient.Send(mail);

                    reservationResult.IsMailSent = true;
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ReservationService UpdateMailStatus error!");
                throw new ArgumentNullException();
            }
        }

        public async Task AddReservationAsync(Reservation reservation)
        {
            using var dbContext = new ApiDbContext();
            // Add message to DbContext
            await dbContext.Reservations.AddAsync(reservation);
            await dbContext.SaveChangesAsync();
        }
    }
}