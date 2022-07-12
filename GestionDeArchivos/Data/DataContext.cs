using GestionDeArchivos.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionDeArchivos.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Areas> Areas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Document>().HasIndex(d => d.name).IsUnique();
            modelBuilder.Entity<Areas>().HasIndex(a => a.name).IsUnique();
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Correo).IsUnique();
        }
    }
}
