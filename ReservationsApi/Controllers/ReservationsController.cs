using Microsoft.AspNetCore.Mvc;
using ReservationsApi.Interfaces;
using ReservationsApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReservationsApi.Controllers
{
    [Route("api/[controller]")]
    public class ReservationsController : Controller
    {
        private IReservation _reservationService;

        public ReservationsController(IReservation reservationService)
        {
            _reservationService = reservationService;
        }

        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<Reservation>> Get()
        {
            var reservations = await _reservationService.GetReservations();
            return reservations;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task Put(int id)
        {  
            await _reservationService.UpdateMailStatus(id);
        }
    }
}