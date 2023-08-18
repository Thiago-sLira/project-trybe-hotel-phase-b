using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TrybeHotel.Dto;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("booking")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "client")]

    public class BookingController : Controller
    {
        private readonly IBookingRepository _repository;
        public BookingController(IBookingRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Add([FromBody] BookingDtoInsert bookingInsert)
        {
            var token = HttpContext.User.Identity as ClaimsIdentity;

            var email = token?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var bookAdded = _repository.Add(bookingInsert, email);

            if (bookAdded != null)
            {
                return Created("", bookAdded);
            }
            else
            {
                return BadRequest(new { message = "Guest quantity over room capacity" });
            }
        }

        [HttpGet("{Bookingid}")]
        public IActionResult GetBooking(int Bookingid)
        {
            var token = HttpContext.User.Identity as ClaimsIdentity;

            var email = token?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var booking = _repository.GetBooking(Bookingid, email);

            if (booking != null)
            {
                return Ok(booking);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}