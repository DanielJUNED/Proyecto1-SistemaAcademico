using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace SistemaAcademico.Models
{ 
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("name=DefaultConnection", throwIfV1Schema: false)
        {
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<Canton> Canton { get; set; }
        public virtual DbSet<Cuatrimestre> Cuatrimestre { get; set; }
        public virtual DbSet<Curso> Curso { get; set; }
        public virtual DbSet<CursoCuatrimestre> CursoCuatrimestre { get; set; }
        public virtual DbSet<Distrito> Distrito { get; set; }
        public virtual DbSet<Docente> Docente { get; set; }
        public virtual DbSet<Estudiante> Estudiante { get; set; }
        public virtual DbSet<EstudianteCurso> EstudianteCurso { get; set; }
        public virtual DbSet<Evaluacion> Evaluacion { get; set; }
        public virtual DbSet<Provincia> Provincia { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Canton>()
                .HasMany(e => e.Distrito)
                .WithRequired(e => e.Canton)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Cuatrimestre>()
                .HasMany(e => e.CursoCuatrimestre)
                .WithRequired(e => e.Cuatrimestre)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Curso>()
                .HasMany(e => e.CursoCuatrimestre)
                .WithRequired(e => e.Curso)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CursoCuatrimestre>()
                .HasMany(e => e.EstudianteCurso)
                .WithRequired(e => e.CursoCuatrimestre)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Distrito>()
                .HasMany(e => e.Estudiante)
                .WithRequired(e => e.Distrito)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Docente>()
                .HasMany(e => e.Evaluacion)
                .WithRequired(e => e.Docente)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Estudiante>()
                .HasMany(e => e.EstudianteCurso)
                .WithRequired(e => e.Estudiante)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EstudianteCurso>()
                .HasMany(e => e.Evaluacion)
                .WithRequired(e => e.EstudianteCurso)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Evaluacion>()
                .Property(e => e.Nota)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Provincia>()
                .HasMany(e => e.Canton)
                .WithRequired(e => e.Provincia)
                .WillCascadeOnDelete(false);

            // Configurar nombres de tablas de Identity (opcional)
            modelBuilder.Entity<ApplicationUser>().ToTable("Usuarios");
            // Renombrar tablas de Identity (opcional) 
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UsuarioRoles"); 
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UsuarioClaims");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UsuarioLogins");
            //modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            //modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UsuarioTokens");
        }
    }
}
