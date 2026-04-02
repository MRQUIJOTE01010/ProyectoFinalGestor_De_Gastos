using Microsoft.EntityFrameworkCore;
using GestorGastos.Models;

namespace GestorGastos.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Usa SQLite local. Cambia la cadena si prefieres otra base de datos.
            optionsBuilder.UseSqlite("Data Source=gastos.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración opcional
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Evita borrar categorías con gastos

            // Datos iniciales de categorías
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Alimentación" },
                new Category { Id = 2, Name = "Transporte" },
                new Category { Id = 3, Name = "Entretenimiento" },
                new Category { Id = 4, Name = "Servicios" },
                new Category { Id = 5, Name = "Otros" }
            );
        }
    }

}

