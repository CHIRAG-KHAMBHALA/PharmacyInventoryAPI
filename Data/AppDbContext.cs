using Microsoft.EntityFrameworkCore;
using PharmacyInventoryAPI.Models;

namespace PharmacyInventoryAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One Supplier → Many Medicines
            modelBuilder.Entity<Medicine>()
                .HasOne(m => m.Supplier)
                .WithMany(s => s.Medicines)
                .HasForeignKey(m => m.SupplierId);

            // Index on Name and Category for faster queries
            modelBuilder.Entity<Medicine>()
                .HasIndex(m => m.Name);

            modelBuilder.Entity<Medicine>()
                .HasIndex(m => m.Category);
        }
    }
}