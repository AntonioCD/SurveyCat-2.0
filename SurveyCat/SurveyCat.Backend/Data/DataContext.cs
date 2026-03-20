using Microsoft.EntityFrameworkCore;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Departamento> Departamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Departamento>().HasIndex(x => x.CodDepto).IsUnique();
            modelBuilder.Entity<Departamento>().HasIndex(x => x.Nombre).IsUnique();
        }
    }
}