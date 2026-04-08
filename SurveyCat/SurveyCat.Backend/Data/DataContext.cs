using Microsoft.EntityFrameworkCore;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<BarrioComarca> BarriosComarcas { get; set; }
        public DbSet<Caserio> Caserios { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Sector> Sectores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Departamento>().HasIndex(x => x.CodDepto).IsUnique();
            modelBuilder.Entity<Departamento>().HasIndex(x => x.Nombre).IsUnique();
            modelBuilder.Entity<Municipio>().HasIndex(x => x.CodMuni).IsUnique();
            modelBuilder.Entity<BarrioComarca>().HasIndex(x => x.CodBarrioComarca).IsUnique();
            modelBuilder.Entity<Caserio>().HasIndex(x => x.CodCaserio).IsUnique();
            modelBuilder.Entity<Sector>().HasIndex(x => new { x.MunicipioId, x.NumeroSector }).IsUnique();

            DisableCascadingDelete(modelBuilder);
        }

        private void DisableCascadingDelete(ModelBuilder modelBuilder)
        {
            var relationships = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var relationship in relationships)
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}