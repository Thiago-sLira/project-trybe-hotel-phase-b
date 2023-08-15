using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class CityRepository : ICityRepository
    {
        protected readonly ITrybeHotelContext _context;
        public CityRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public IEnumerable<CityDto> GetCities()
        {
            return _context.Cities.Select(c => new CityDto
            {
                cityId = c.CityId,
                name = c.Name,
            });
        }

        public CityDto AddCity(City city)
        {
            _context.Cities.Add(city);
            _context.SaveChanges();
            return _context.Cities.Select(c => new CityDto
            {
                cityId = c.CityId,
                name = c.Name,
            }).Last();
        }

    }
}