using Microsoft.EntityFrameworkCore;
using V.NET.API.Models;

namespace V.NET.API.Database
{
    public class UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) : DbContext(options)
    {
        public DbSet<UrlMapping> UrlMappings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UrlMapping>()
                .HasIndex(u => u.ShortCode)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
