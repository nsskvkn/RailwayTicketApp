using System.Data.Entity;
using RailwayTicketApp.Models;

namespace RailwayTicketApp.Data
{
    public class RailwayDbContext : DbContext
    {
        public DbSet<Train> Trains { get; set; }
        public DbSet<Wagon> Wagons { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public RailwayDbContext() : base("RailwayTicketDbConnection")
        {
            // Скидаємо базу при зміні моделі
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<RailwayDbContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Вимикаємо каскадне видалення для одного зі зв'язків, щоб уникнути конфлікту
            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.Train)
                .WithMany(t => t.Bookings)
                .HasForeignKey(b => b.TrainId)
                .WillCascadeOnDelete(false); // Вимикаємо каскад

            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.Wagon)
                .WithMany(w => w.Bookings)
                .HasForeignKey(b => b.WagonId)
                .WillCascadeOnDelete(true); // Залишаємо каскад для Wagon -> Booking

            modelBuilder.Entity<Wagon>()
                .HasRequired(w => w.Train)
                .WithMany(t => t.Wagons)
                .HasForeignKey(w => w.TrainId)
                .WillCascadeOnDelete(true); // Каскад Train -> Wagon
        }
    }
}