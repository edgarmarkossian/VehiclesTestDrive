using Microsoft.EntityFrameworkCore;

using VehiclesApi.Data;
using VehiclesApi.Interfaces;
using VehiclesApi.Models;

namespace VehiclesApi.Services
{
    public class VehicleService : IVehicle
    {
        private ApiDbContext _dbContext;

        public VehicleService()
        {
            _dbContext = new ApiDbContext();
        }

        public async Task AddVehicle(Vehicle vehicle)
        {
            await _dbContext.Vehicles.AddAsync(vehicle);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteVehicle(int id)
        {
            var vehicle = await _dbContext.Vehicles.FindAsync(id);
            _dbContext.Vehicles.Remove(vehicle);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Vehicle>> GetAllVehicles()
        {
            var vehicles = await _dbContext.Vehicles.ToListAsync();
            return vehicles;
        }

        public async Task<Vehicle> GetVehicleById(int id)
        {
            var vehicle = await _dbContext.Vehicles.FindAsync(id);
            return vehicle;
        }

        public async Task UpdateVehicle(int id, Vehicle vehicle)
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
    }
}