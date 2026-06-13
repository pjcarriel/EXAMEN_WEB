using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MoldeMVC_Core.Models;

public partial class CatalogoMateriaBdCoreContext : DbContext
{
    public CatalogoMateriaBdCoreContext()
    {
    }

    public CatalogoMateriaBdCoreContext(DbContextOptions<CatalogoMateriaBdCoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Carrera> Carreras { get; set; }

    public virtual DbSet<CatalogoMateria> CatalogoMaterias { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Estudiante> Estudiantes { get; set; }

    public virtual DbSet<Matricula> Matriculas { get; set; }

    public virtual DbSet<Nivel> Nivels { get; set; }

    public virtual DbSet<NrcMaterium> NrcMateria { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Profesor> Profesors { get; set; }

    public virtual DbSet<Rolpersona> Rolpersonas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=CatalogoMateriaBD_Core;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=300;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Carrera>(entity =>
        {
            entity.HasKey(e => e.CarreraId).HasName("PK__carrera__3E43B1A1592DBDAC");

            entity.ToTable("carrera");

            entity.Property(e => e.NombreCarrera)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CatalogoMateria>(entity =>
        {
            entity.HasKey(e => e.MateriaId).HasName("PK__CATALOGO__0D019DE188AB14A3");

            entity.ToTable("CATALOGO_MATERIAS");

            entity.Property(e => e.NombreMateria)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.Carrera).WithMany(p => p.CatalogoMateria)
                .HasForeignKey(d => d.CarreraId)
                .HasConstraintName("FK__CATALOGO___Carre__398D8EEE");

            entity.HasOne(d => d.Departamento).WithMany(p => p.CatalogoMateria)
                .HasForeignKey(d => d.DepartamentoId)
                .HasConstraintName("FK__CATALOGO___Depar__38996AB5");

            entity.HasOne(d => d.Nivel).WithMany(p => p.CatalogoMateria)
                .HasForeignKey(d => d.NivelId)
                .HasConstraintName("FK__CATALOGO___Nivel__37A5467C");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.DepartamentoId).HasName("PK__departam__66BB0E3E199EE95C");

            entity.ToTable("departamento");

            entity.Property(e => e.NombreDepartamento)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.SiglasDepartamento)
                .HasMaxLength(8)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.HasKey(e => e.EstudianteId).HasName("PK__estudian__6F7682D852E82B5F");

            entity.ToTable("estudiante");

            entity.HasOne(d => d.Carrera).WithMany(p => p.Estudiantes)
                .HasForeignKey(d => d.CarreraId)
                .HasConstraintName("FK__estudiant__Carre__33D4B598");

            entity.HasOne(d => d.NivelActualEstNavigation).WithMany(p => p.Estudiantes)
                .HasForeignKey(d => d.NivelActualEst)
                .HasConstraintName("FK__estudiant__Nivel__34C8D9D1");

            entity.HasOne(d => d.Personal).WithMany(p => p.Estudiantes)
                .HasForeignKey(d => d.PersonalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__estudiant__Perso__32E0915F");
        });

        modelBuilder.Entity<Matricula>(entity =>
        {
            entity.HasKey(e => e.RegistroId).HasName("PK__MATRICUL__B89731DE76029397");

            entity.ToTable("MATRICULA");

            entity.Property(e => e.Nrc)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("NRC");

            entity.HasOne(d => d.Estudiante).WithMany(p => p.Matriculas)
                .HasForeignKey(d => d.EstudianteId)
                .HasConstraintName("FK__MATRICULA__Estud__412EB0B6");

            entity.HasOne(d => d.NrcNavigation).WithMany(p => p.Matriculas)
                .HasForeignKey(d => d.Nrc)
                .HasConstraintName("FK__MATRICULA__NRC__403A8C7D");
        });

        modelBuilder.Entity<Nivel>(entity =>
        {
            entity.HasKey(e => e.NivelId).HasName("PK__nivel__316FA27798B23045");

            entity.ToTable("nivel");

            entity.Property(e => e.NivelDescrip)
                .HasMaxLength(32)
                .IsUnicode(false);
        });

        modelBuilder.Entity<NrcMaterium>(entity =>
        {
            entity.HasKey(e => e.Nrc).HasName("PK__NRC_Mate__C7DEEA5BEC83B481");

            entity.ToTable("NRC_Materia");

            entity.Property(e => e.Nrc)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("NRC");

            entity.HasOne(d => d.Materia).WithMany(p => p.NrcMateria)
                .HasForeignKey(d => d.MateriaId)
                .HasConstraintName("FK__NRC_Mater__Mater__3C69FB99");

            entity.HasOne(d => d.Profesor).WithMany(p => p.NrcMateria)
                .HasForeignKey(d => d.ProfesorId)
                .HasConstraintName("FK__NRC_Mater__Profe__3D5E1FD2");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.PersonalId).HasName("PK__persona__283437F3E0C2B431");

            entity.ToTable("persona");

            entity.Property(e => e.ApellidoPersona)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.CedulaPersona)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Foto)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.NombrePersona)
                .HasMaxLength(32)
                .IsUnicode(false);

            entity.HasOne(d => d.RolPersona).WithMany(p => p.Personas)
                .HasForeignKey(d => d.RolPersonaId)
                .HasConstraintName("FK__persona__RolPers__267ABA7A");
        });

        modelBuilder.Entity<Profesor>(entity =>
        {
            entity.HasKey(e => e.ProfesorId).HasName("PK__profesor__4DF3F0C8A461246E");

            entity.ToTable("profesor");

            entity.HasOne(d => d.Departamento).WithMany(p => p.Profesors)
                .HasForeignKey(d => d.DepartamentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__profesor__Depart__300424B4");

            entity.HasOne(d => d.Personal).WithMany(p => p.Profesors)
                .HasForeignKey(d => d.PersonalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__profesor__Person__2F10007B");
        });

        modelBuilder.Entity<Rolpersona>(entity =>
        {
            entity.HasKey(e => e.RolPersonaId).HasName("PK__rolperso__64E281D05155197E");

            entity.ToTable("rolpersona");

            entity.Property(e => e.RolNombre)
                .HasMaxLength(16)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
