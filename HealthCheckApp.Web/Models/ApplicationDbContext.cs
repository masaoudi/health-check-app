using Microsoft.EntityFrameworkCore;

namespace HealthCheckApp.Web.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MonitoredSite> MonitoredSites { get; set; } // La table dans la DB

        // On peut ajouter une configuration simple ici si besoin
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Exemple : rendre l'Url obligatoire
            modelBuilder.Entity<MonitoredSite>()
                .Property(m => m.Url)
                .IsRequired();
        }
    }
}
