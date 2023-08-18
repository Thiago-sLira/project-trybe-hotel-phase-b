namespace TrybeHotel.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Security.Claims;
using Newtonsoft.Json;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Text;
using System.Net.Http.Headers;
using TrybeHotel.Dto;
using TrybeHotel.Services;

public class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientTest;

    public IntegrationTest(WebApplicationFactory<Program> factory)
    {
        //_factory = factory;
        _clientTest = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TrybeHotelContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                builder.ConfigureServices(services =>
                {
                    // ... (seus serviços)

                    // Adicione a política de autorização "admin" ao serviço de autorização
                    services.AddAuthorization(options =>
                    {
                        options.AddPolicy("admin", policy =>
                        {
                            policy.RequireClaim(ClaimTypes.Role, "admin");
                            policy.RequireClaim(ClaimTypes.Email);
                        });
                    });
                });

                services.AddDbContext<ContextTest>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDatabase");
                });
                services.AddScoped<ITrybeHotelContext, ContextTest>();
                services.AddScoped<ICityRepository, CityRepository>();
                services.AddScoped<IHotelRepository, HotelRepository>();
                services.AddScoped<IRoomRepository, RoomRepository>();
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ContextTest>())
                {
                    appContext.Database.EnsureCreated();
                    appContext.Database.EnsureDeleted();
                    appContext.Database.EnsureCreated();
                    appContext.Cities.Add(new City { CityId = 1, Name = "Manaus" });
                    appContext.Cities.Add(new City { CityId = 2, Name = "Palmas" });
                    appContext.SaveChanges();
                    appContext.Hotels.Add(new Hotel { HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1 });
                    appContext.Hotels.Add(new Hotel { HotelId = 2, Name = "Trybe Hotel Palmas", Address = "Address 2", CityId = 2 });
                    appContext.Hotels.Add(new Hotel { HotelId = 3, Name = "Trybe Hotel Ponta Negra", Address = "Addres 3", CityId = 1 });
                    appContext.SaveChanges();
                    appContext.Rooms.Add(new Room { RoomId = 1, Name = "Room 1", Capacity = 2, Image = "Image 1", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 2, Name = "Room 2", Capacity = 3, Image = "Image 2", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 3, Name = "Room 3", Capacity = 4, Image = "Image 3", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 4, Name = "Room 4", Capacity = 2, Image = "Image 4", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 5, Name = "Room 5", Capacity = 3, Image = "Image 5", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 6, Name = "Room 6", Capacity = 4, Image = "Image 6", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 7, Name = "Room 7", Capacity = 2, Image = "Image 7", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 8, Name = "Room 8", Capacity = 3, Image = "Image 8", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 9, Name = "Room 9", Capacity = 4, Image = "Image 9", HotelId = 3 });
                    appContext.SaveChanges();
                    appContext.Users.Add(new User
                    {
                        UserId = 1,
                        Name = "Ana",
                        Email = "ana@trybehotel.com",
                        Password = "Senha1",
                        UserType = "admin"
                    });
                    appContext.Users.Add(new User
                    {
                        UserId = 2,
                        Name = "Beatriz",
                        Email = "beatriz@trybehotel.com",
                        Password = "Senha2",
                        UserType = "client"
                    });
                    appContext.Users.Add(new User
                    {
                        UserId = 3,
                        Name = "Laura",
                        Email = "laura@trybehotel.com",
                        Password = "Senha3",
                        UserType = "client"
                    });
                    appContext.SaveChanges();
                    appContext.Bookings.Add(new Booking
                    {
                        BookingId = 1,
                        CheckIn = new DateTime(2023, 07, 02),
                        CheckOut = new DateTime(2023, 07, 03),
                        GuestQuant = 1,
                        UserId = 2,
                        RoomId = 1
                    });
                    appContext.Bookings.Add(new Booking
                    {
                        BookingId = 2,
                        CheckIn = new DateTime(2023, 07, 02),
                        CheckOut = new DateTime(2023, 07, 03),
                        GuestQuant = 1,
                        UserId = 3,
                        RoomId = 4
                    });
                    appContext.SaveChanges();
                }
            });
        }).CreateClient();
    }

    // Testes da rota /city
    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Executando meus testes")]
    [InlineData("/city")]
    public async Task TestGet(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Teste de Post de City")]
    [InlineData("/city")]
    public async Task TestPost(string url)
    {
        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new City
        {
            CityId = 3,
            Name = "São Paulo"
        }), Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }

    // Testes da rota /hotel
    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Teste de Post de Hotel")]
    [InlineData("/hotel")]
    public async Task TestPostHotel(string url)
    {
        var token = new TokenGenerator().Generate(new UserDto { UserId = 1, Name = "Ana", Email = "ana@trybehotel.com", UserType = "admin" });

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new Hotel
        {
            HotelId = 4,
            Name = "Trybe Hotel São Paulo",
            Address = "Address 4",
            CityId = 1
        }), Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }

    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Teste de Get de Hotel")]
    [InlineData("/hotel")]
    public async Task TestGetHotel(string url)
    {
        var token = new TokenGenerator().Generate(new UserDto
        {
            UserId = 1,
            Name = "Ana",
            Email = "ana@trybehotel.com",
            UserType = "admin"
        });

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    // Testes da rota /room
    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Teste de Post de Room")]
    [InlineData("/room")]
    public async Task TestPostRoom(string url)
    {
        var token = new TokenGenerator().Generate(new UserDto
        {
            UserId = 1,
            Name = "Ana",
            Email = "ana@trybehotel.com",
            UserType = "admin"
        });

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new Room
        {
            RoomId = 10,
            Name = "Room 10",
            Capacity = 2,
            Image = "Image 10",
            HotelId = 1
        }), Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }

    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Teste de Get de Room com Id")]
    [InlineData("/room/1")]
    public async Task TestGetRoomId(string url)
    {
        var token = new TokenGenerator().Generate(new UserDto
        {
            UserId = 1,
            Name = "Ana",
            Email = "ana@trybehotel.com",
            UserType = "admin"
        });

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Teste de Delete de Room com Id")]
    [InlineData("/room/1")]
    public async Task TestDeleteRoomId(string url)
    {
        var token = new TokenGenerator().Generate(new UserDto
        {
            UserId = 1,
            Name = "Ana",
            Email = "ana@trybehotel.com",
            UserType = "admin"
        });

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.DeleteAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response?.StatusCode);
    }

    // Testes da rota /booking
    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Teste de Post de Booking")]
    [InlineData("/booking")]
    public async Task TestPostBooking(string url)
    {
        var token = new TokenGenerator().Generate(new UserDto
        {
            UserId = 2,
            Name = "Beatriz",
            Email = "beatriz@trybehotel.com",
            UserType = "client"
        });

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new BookingDtoInsert
        {
            CheckIn = "2030-08-27",
            CheckOut = "2030-08-28",
            GuestQuant = 1,
            RoomId = 1
        }), Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }

    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Teste de Get de Booking com Id")]
    [InlineData("/booking/1")]
    public async Task TestGetBookingId(string url)
    {
        var token = new TokenGenerator().Generate(new UserDto
        {
            UserId = 2,
            Name = "Beatriz",
            Email = "beatriz@trybehotel.com",
            UserType = "client"
        });

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.GetAsync(url);

        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    // Testes da rota /user
    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Teste de Get de Users")]
    [InlineData("/user")]
    public async Task TestGetUsers(string url)
    {
        var token = new TokenGenerator().Generate(new UserDto
        {
            UserId = 1,
            Name = "Ana",
            Email = "ana@trybehotel.com",
            UserType = "admin"
        });

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.GetAsync(url);

        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    [Trait("Category", "Meus testes")]
    [Theory(DisplayName = "Teste de Post de User")]
    [InlineData("/user")]
    public async Task TestPostUser(string url)
    {
        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new UserDtoInsert
        {
            Name = "João",
            Email = "joao@trybehotel.com",
            Password = "Senha4"
        }), Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
    }
}