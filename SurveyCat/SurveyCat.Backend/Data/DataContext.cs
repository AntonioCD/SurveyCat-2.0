using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<BarrioComarca> BarriosComarcas { get; set; }
        public DbSet<Caserio> Caserios { get; set; }
        public DbSet<Colindante> Colindantes { get; set; }
        public DbSet<Conflicto> Conflictos { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Diccionario> Diccionarios { get; set; }
        public DbSet<Familia> Familias { get; set; }
        public DbSet<Ficha> Fichas { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<PersonalEncuesta> PersonalEncuestas { get; set; }
        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Sector> Sectores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Departamento>(e =>
            {
                e.HasIndex(x => x.CodDepto).IsUnique();
                e.HasIndex(x => x.Nombre).IsUnique();
            });

            modelBuilder.Entity<Municipio>(e =>
            {
                e.HasIndex(x => x.CodMuni).IsUnique();
            });

            modelBuilder.Entity<BarrioComarca>(e =>
            {
                e.HasIndex(x => x.CodBarrioComarca).IsUnique();
            });

            modelBuilder.Entity<Caserio>(e =>
            {
                e.HasIndex(x => x.CodCaserio).IsUnique();
            });

            modelBuilder.Entity<Colindante>(e =>
            {
                e.HasIndex(x => new { x.FichaId, x.PuntoCardinalId, x.PersonaId }).IsUnique();
            });

            modelBuilder.Entity<Conflicto>(e =>
            {
                e.HasIndex(x => new { x.TipoConflictoId, x.ViaGestionId, x.ConEstado }).IsUnique();
            });

            modelBuilder.Entity<Familia>(e =>
            {
                e.HasIndex(x => new { x.FichaId, x.PersonaId }).IsUnique();
            });

            modelBuilder.Entity<Ficha>(e =>
            {
                e.HasIndex(x => x.CodEncuesta).IsUnique();
            });

            modelBuilder.Entity<PersonalEncuesta>(e =>
            {
                e.HasIndex(x => x.PersonaId).IsUnique();
            });

            modelBuilder.Entity<Propietario>(e =>
            {
                e.HasIndex(x => new { x.FichaId, x.PersonaId }).IsUnique();
            });

            modelBuilder.Entity<Sector>(e =>
            {
                e.HasIndex(x => new { x.MunicipioId, x.NumeroSector }).IsUnique();
            });

            modelBuilder.Entity<Diccionario>(e =>
            {
                e.HasIndex(x => new { x.Catalogo, x.Nombre }).IsUnique();
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.UsePropertyAccessMode(PropertyAccessMode.Property);

                modelBuilder.Entity<Persona>(e =>
                {
                    e.HasIndex(x => x.Identificacion).IsUnique();
                });
            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(x => new { x.UserName }).IsUnique();
            });

            // --- MÉTODOS GLOBALES ---
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