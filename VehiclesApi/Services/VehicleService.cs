 using Microsoft.EntityFrameworkCore;

using VehiclesApi.Data;
using VehiclesApi.Interfaces;
using VehiclesApi.Models;

namespace VehiclesApi.Services
{
    public class VehicleService : IVehicle
    {
        private ApiDbContext _dbContext;
        private Serilog.ILogger _logger;

        public VehicleService(Serilog.ILogger logger)
        {
            _dbContext = new ApiDbContext();
            _logger = logger;
        }

        public async Task AddVehicle(Vehicle vehicle)
        {
            try
            {
                await _dbContext.Vehicles.AddAsync(vehicle);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "VehicleService AddVehicle error!");
                throw new ArgumentNullException();
            }
        }

        public async Task DeleteVehicle(int id)
        {
            try
            {
                var vehicle = await _dbContext.Vehicles.FindAsync(id);
                _dbContext.Vehicles.Remove(vehicle);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "VehicleService DeleteVehicle error!");
                throw new ArgumentNullException();
            }
        }

        public async Task<List<Vehicle>> GetAllVehicles()
        {
            try
            {
                var vehicles = await _dbContext.Vehicles.ToListAsync();
                return vehicles;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "VehicleService GetAllVehicles error!");
                throw new ArgumentNullException();
            }
        }

        public async Task<Vehicle> GetVehicleById(int id)
        {
            try
            {
                var vehicle = await _dbContext.Vehicles.FindAsync(id);
                return vehicle;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "VehicleService GetVehicleById error!");
                throw new ArgumentNullException();
            }
        }

        public async Task UpdateVehicle(int id, Vehicle vehicle)
        {
            try
            {
                var vehicleObj = await _dbContext.Vehicles.FindAsync(id);

                vehicleObj.Name = vehicle.Name;
                vehicleObj.Price = vehicle.Price;
                vehicleObj.MaxSpeed = vehicle.MaxSpeed;
                vehicleObj.Length = vehicle.Length;
                vehicleObj.ImageUrl = vehicle.ImageUrl;
                vehicleObj.Height = vehicle.Height;
                vehicleObj.Displacement = vehicle.Displacement;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "VehicleService UpdateVehicle error!");
                throw new ArgumentNullException();
            }
        }
    }
}