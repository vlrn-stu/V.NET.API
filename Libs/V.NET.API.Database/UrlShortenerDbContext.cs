using Microsoft.EntityFrameworkCore;
using V.NET.API.Models;

namespace V.NET.API.Database
{
    public class UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) : DbContext(options)
    {
        public DbSet<UrlMapping> UrlMappings { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UrlMapping>()
                .HasIndex(u => u.ShortCode)
                .IsUnique();

            modelBuilder.Entity<RequestLog>()
                .HasOne(rl => rl.UrlMapping)
                .WithMany(um => um.RequestLogs)
                .HasForeignKey(rl => rl.UrlMappingId)
                .OnDelete(DeleteBehavior.Cascade);  // Set cascade delete

            base.OnModelCreating(modelBuilder);
        }

    }
}
