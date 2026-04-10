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
        public DbSet<Diccionario> Diccionarios { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Sector> Sectores { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<Departamento>().HasIndex(x => x.CodDepto).IsUnique();
        //    modelBuilder.Entity<Departamento>().HasIndex(x => x.Nombre).IsUnique();
        //    modelBuilder.Entity<Diccionario>().HasIndex(x => new { x.Catalogo, x.Nombre }).IsUnique();
        //    modelBuilder.Entity<Municipio>().HasIndex(x => x.CodMuni).IsUnique();
        //    modelBuilder.Entity<BarrioComarca>().HasIndex(x => x.CodBarrioComarca).IsUnique();
        //    modelBuilder.Entity<Caserio>().HasIndex(x => x.CodCaserio).IsUnique();
        //    modelBuilder.Entity<Sector>().HasIndex(x => new { x.MunicipioId, x.NumeroSector }).IsUnique();

        //    var entity = modelBuilder.Entity<Persona>();

        //    // Forzamos el acceso vía Propiedad para disparar la lógica de los Setters
        //    entity.Property(e => e.PrimerNombre)
        //          .UsePropertyAccessMode(PropertyAccessMode.Property);

        //    entity.Property(e => e.SegundoNombre)
        //          .UsePropertyAccessMode(PropertyAccessMode.Property);

        //    entity.Property(e => e.PrimerApellido)
        //          .UsePropertyAccessMode(PropertyAccessMode.Property);

        //    entity.Property(e => e.SegundoApellido)
        //          .UsePropertyAccessMode(PropertyAccessMode.Property);

        //    entity.Property(e => e.NombreCompleto)
        //          .UsePropertyAccessMode(PropertyAccessMode.Property);

        //    DisableCascadingDelete(modelBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CONFIGURACIÓN DE ÍNDICES (GEOGRAFÍA Y DICCIONARIOS) ---
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

            modelBuilder.Entity<Sector>(e =>
            {
                e.HasIndex(x => new { x.MunicipioId, x.NumeroSector }).IsUnique();
            });

            modelBuilder.Entity<Diccionario>(e =>
            {
                e.HasIndex(x => new { x.Catalogo, x.Nombre }).IsUnique();
            });

            // --- CONFIGURACIÓN DE PERSONA ---
            modelBuilder.Entity<Persona>(entity =>
            {
                entity.UsePropertyAccessMode(PropertyAccessMode.Property);

                // Si necesitas índices para Persona (ej. Cédula/DNI), agrégalos aquí
                // entity.HasIndex(x => x.Identificacion).IsUnique();
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