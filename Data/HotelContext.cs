using HotelBookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApi.Data
{
    public class HotelContext : DbContext
    {
        public HotelContext(DbContextOptions<HotelContext> options) : base(options)
        {
        }

        public DbSet<RoomType> RoomTypes => Set<RoomType>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Guest> Guests => Set<Guest>();
        public DbSet<Booking> Bookings => Set<Booking>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.Property(rt => rt.Name).IsRequired().HasMaxLength(100);
                entity.Property(rt => rt.Capacity).IsRequired();
                entity.Property(rt => rt.PricePerNight).HasPrecision(18, 2);
                entity.HasMany(rt => rt.Rooms).WithOne(r => r.RoomType).HasForeignKey(r => r.RoomTypeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(r => r.Number).IsRequired().HasMaxLength(20);
                entity.HasIndex(r => r.Number).IsUnique();
                entity.Property(r => r.Floor).IsRequired();
                entity.Property(r => r.IsAvailable).HasDefaultValue(true);
                entity.Property(r => r.RoomTypeId).IsRequired();
                entity.HasMany(r => r.Bookings).WithOne(b => b.Room).HasForeignKey(b => b.RoomId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Guest>(entity =>
            {
                entity.Property(g => g.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(g => g.LastName).IsRequired().HasMaxLength(100);
                entity.Property(g => g.Email).IsRequired().HasMaxLength(200);
                entity.HasIndex(g => g.Email).IsUnique();
                entity.Property(g => g.Phone).IsRequired().HasMaxLength(50);
                entity.HasMany(g => g.Bookings).WithOne(b => b.Guest).HasForeignKey(b => b.GuestId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(b => b.CheckIn).IsRequired();
                entity.Property(b => b.CheckOut).IsRequired();
                entity.Property(b => b.TotalPrice).HasPrecision(18, 2);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
