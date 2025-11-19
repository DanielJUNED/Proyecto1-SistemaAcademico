using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaAcademico.Data.Entities;

namespace SistemaAcademico.Data.Context
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Canton> Canton { get; set; }
        public DbSet<Cuatrimestre> Cuatrimestre { get; set; }
        public DbSet<Curso> Curso { get; set; }
        public DbSet<CursoCuatrimestre> CursoCuatrimestre { get; set; }
        public DbSet<CursoCuatrimestreDocente> CursoCuatrimestreDocente { get; set; }
        public DbSet<Distrito> Distrito { get; set; }
        public DbSet<Docente> Docente { get; set; }
        public DbSet<Estudiante> Estudiante { get; set; }
        public DbSet<EstudianteCurso> EstudianteCurso { get; set; }
        public DbSet<Evaluacion> Evaluacion { get; set; }
        public DbSet<Provincia> Provincia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------
            // CANTON
            // -------------------------
            modelBuilder.Entity<Canton>()
                .HasMany(e => e.Distrito)
                .WithOne(e => e.Canton)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // CUATRIMESTRE
            // -------------------------
            modelBuilder.Entity<Cuatrimestre>()
                .HasMany(e => e.CursoCuatrimestre)
                .WithOne(e => e.Cuatrimestre)
                .HasForeignKey(e => e.CuatrimestreId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // CURSO
            // -------------------------
            modelBuilder.Entity<Curso>()
                .HasMany(e => e.CursoCuatrimestre)
                .WithOne(e => e.Curso)
                .HasForeignKey(e => e.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // CURSO CUATRIMESTRE
            // -------------------------
            modelBuilder.Entity<CursoCuatrimestre>()
                .HasMany(e => e.EstudianteCurso)
                .WithOne(e => e.CursoCuatrimestre)
                .HasForeignKey(e => e.CursoCuatrimestreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CursoCuatrimestre>()
                .HasMany(e => e.CursoCuatrimestreDocente)
                .WithOne(cd => cd.CursoCuatrimestre)
                .HasForeignKey(cd => cd.CursoCuatrimestreId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // DOCENTE
            // -------------------------
            modelBuilder.Entity<Docente>()
                .HasMany(d => d.CursoCuatrimestreDocente)
                .WithOne(cd => cd.Docente)
                .HasForeignKey(cd => cd.DocenteId)   // ✔ correcto
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Docente>()
                .HasMany(d => d.Evaluacion)
                .WithOne(e => e.Docente)
                .HasForeignKey(e => e.DocenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Docente>()
                .HasOne(d => d.User)
                .WithMany(u => u.Docente)
                .HasForeignKey(d => d.UserId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // ESTUDIANTE
            // -------------------------
            modelBuilder.Entity<Estudiante>()
                .HasMany(e => e.EstudianteCurso)
                .WithOne(ec => ec.Estudiante)
                .HasForeignKey(ec => ec.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // ESTUDIANTE CURSO
            // -------------------------
            modelBuilder.Entity<EstudianteCurso>()
                .HasMany(ec => ec.Evaluacion)
                .WithOne(ev => ev.EstudianteCurso)
                .HasForeignKey(ev => ev.EstudianteCursoId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // EVALUACION
            // -------------------------
            modelBuilder.Entity<Evaluacion>()
                .Property(e => e.Nota)
                .HasPrecision(5, 2);

            // -------------------------
            // PROVINCIA
            // -------------------------
            modelBuilder.Entity<Provincia>()
                .HasMany(p => p.Canton)
                .WithOne(c => c.Provincia)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // CURSO_CUATRIMESTRE_DOCENTE (tabla intermedia)
            // -------------------------
            modelBuilder.Entity<CursoCuatrimestreDocente>()
                .HasKey(cd => cd.CursoCuatriDocenteId);

            modelBuilder.Entity<CursoCuatrimestreDocente>()
                .HasIndex(cd => new { cd.CursoCuatrimestreId, cd.DocenteId })
                .IsUnique();

            // -------------------------
            // Identity: renombrar tablas
            // -------------------------
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
