using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class HotelRepository : IHotelRepository
    {
        protected readonly ITrybeHotelContext _context;
        public HotelRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public IEnumerable<HotelDto> GetHotels()
        {
            var content = from h in _context.Hotels
                          join c in _context.Cities on h.CityId equals c.CityId
                          select new HotelDto
                          {
                              hotelId = h.HotelId,
                              name = h.Name,
                              address = h.Address,
                              cityId = c.CityId,
                              cityName = c.Name
                          };
            return content;
        }

        public HotelDto AddHotel(Hotel hotel)
        {
            _context.Hotels.Add(hotel);
            _context.SaveChanges();
            var content = from h in _context.Hotels
                          join c in _context.Cities on h.CityId equals c.CityId
                          select new HotelDto
                          {
                              hotelId = h.HotelId,
                              name = h.Name,
                              address = h.Address,
                              cityId = c.CityId,
                              cityName = c.Name
                          };
            return content.Last();
        }
    }
}