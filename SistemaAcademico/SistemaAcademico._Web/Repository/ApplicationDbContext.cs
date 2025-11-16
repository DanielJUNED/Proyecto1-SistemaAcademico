using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaAcademico.Data.Entities;

namespace SistemaAcademico._Web.Repository
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        /*public static ApplicationDbContext Create()
            {
                return new ApplicationDbContext();
            }*/

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Canton>()
                .HasMany(e => e.Distrito)
                .WithOne(e => e.Canton)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cuatrimestre>()
                .HasMany(e => e.CursoCuatrimestre)
                .WithOne(e => e.Cuatrimestre)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Curso>()
                .HasMany(e => e.CursoCuatrimestre)
                .WithOne(e => e.Curso)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CursoCuatrimestre>()
                .HasMany(e => e.EstudianteCurso)
                .WithOne(e => e.CursoCuatrimestre)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Distrito>()
                .HasMany(e => e.Estudiante)
                .WithOne(e => e.Distrito)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Docente>()
                .HasMany(d => d.Evaluacion)
                .WithOne(e => e.Docente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Docente>()
                .HasMany(d => d.CursoCuatrimestre)
                .WithOne(e => e.Docente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Docente>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Estudiante>()
                .HasMany(e => e.EstudianteCurso)
                .WithOne(e => e.Estudiante)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EstudianteCurso>()
                .HasMany(e => e.Evaluacion)
                .WithOne(e => e.EstudianteCurso)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluacion>()
                .Property(e => e.Nota)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Provincia>()
                .HasMany(e => e.Canton)
                .WithOne(e => e.Provincia)
                .OnDelete(DeleteBehavior.Restrict);

            // Renombrar tablas de Identity (opcional)
            modelBuilder.Entity<ApplicationUser>().ToTable("Usuarios");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UsuarioRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UsuarioClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UsuarioLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UsuarioTokens");
        }
    }

}
