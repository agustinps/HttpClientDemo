using Comun;
using Microsoft.EntityFrameworkCore;

namespace HttpClientDemo.API
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Persona> Personas { get; set; }
    }
}
