using System;
using demo_app.Models;
using Microsoft.EntityFrameworkCore;

namespace demo_app.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Persona> Personas { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Persona>(entity =>
            {
                entity.ToTable("Personas");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Edad)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Eliminado)
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .IsRequired();

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp without time zone")
                    .IsRequired();
            });
        }

    }

}

