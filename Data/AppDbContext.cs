using Microsoft.EntityFrameworkCore;
using CrudPark.API.Models;

namespace CrudPark.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets - Representan las tablas en la base de datos
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<Operator> Operators { get; set; }
    public DbSet<Rate> Rates { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar DateTime como timestamp sin zona horaria
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("timestamp without time zone");
                }
            }
        }

        // Configuración de relaciones y restricciones

        // Ticket -> Operator (Entrada)
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.EntryOperator)
            .WithMany()
            .HasForeignKey(t => t.EntryOperatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ticket -> Operator (Salida)
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.ExitOperator)
            .WithMany()
            .HasForeignKey(t => t.ExitOperatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ticket -> Membership
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Membership)
            .WithMany()
            .HasForeignKey(t => t.MembershipId)
            .OnDelete(DeleteBehavior.Restrict);

        // Payment -> Ticket
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Ticket)
            .WithMany()
            .HasForeignKey(p => p.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        // Payment -> Operator
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Operator)
            .WithMany()
            .HasForeignKey(p => p.OperatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices únicos
        modelBuilder.Entity<Membership>()
            .HasIndex(m => m.LicensePlate)
            .HasFilter($"\"IsActive\" = TRUE")
            .IsUnique();

        modelBuilder.Entity<Operator>()
            .HasIndex(o => o.Username)
            .IsUnique();

        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.Folio)
            .IsUnique();

        // Configuración de precisión para decimales
        modelBuilder.Entity<Rate>()
            .Property(r => r.HourlyRate)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Rate>()
            .Property(r => r.FractionRate)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Rate>()
            .Property(r => r.DailyCap)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(p => p.AmountCharged)
            .HasPrecision(18, 2);
    }

}