using Microsoft.EntityFrameworkCore;
using TrybeHotel.Models;

namespace TrybeHotel.Repository;
public class TrybeHotelContext : DbContext, ITrybeHotelContext
{
    public TrybeHotelContext(DbContextOptions<TrybeHotelContext> options) : base(options)
    {
        Seeder.SeedUserAdmin(this);
    }
    public TrybeHotelContext() { }

    public DbSet<City> Cities { get; set; } = null!;

    public DbSet<Hotel> Hotels { get; set; } = null!;

    public DbSet<Room> Rooms { get; set; } = null!;

    public DbSet<Booking> Bookings { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer
        (
            @"
            Server=localhost;
            Database=TrybeHotel;
            User=SA;
            Password=TrybeHotel12!;
            TrustServerCertificate=True
            "
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Hotel>()
        .HasOne(h => h.City)
        .WithMany(c => c.Hotels)
        .HasForeignKey(h => h.CityId);

        modelBuilder.Entity<Room>()
        .HasOne(r => r.Hotel)
        .WithMany(h => h.Rooms)
        .HasForeignKey(r => r.HotelId);

        modelBuilder.Entity<Booking>()
        .HasOne(b => b.Room)
        .WithMany(r => r.Bookings)
        .HasForeignKey(b => b.RoomId);

        modelBuilder.Entity<Booking>()
        .HasOne(b => b.User)
        .WithMany(u => u.Bookings)
        .HasForeignKey(b => b.UserId);
    }

}