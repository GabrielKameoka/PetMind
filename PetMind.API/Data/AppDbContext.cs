using Microsoft.EntityFrameworkCore;
using PetMind.API.Models.Entities;

namespace PetMind.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PetShop> PetShops { get; set; }
        public DbSet<Horario> Horarios { get; set; }
        public DbSet<Cachorro> Cachorros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração do Horario
            modelBuilder.Entity<Horario>(entity =>
            {
                // Relacionamento com Cachorro (1:1)
                entity.HasOne(h => h.Cachorro)
                    .WithMany() // Um cachorro pode ter vários horários
                    .HasForeignKey(h => h.CachorroId)
                    .OnDelete(DeleteBehavior.Restrict);
            
                // Relacionamento com PetShop
                entity.HasOne(h => h.PetShop)
                    .WithMany(p => p.Horarios)
                    .HasForeignKey(h => h.PetShopId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração do Cachorro
            modelBuilder.Entity<Cachorro>(entity =>
            {
                // Relacionamento com PetShop
                entity.HasOne(c => c.PetShop)
                    .WithMany(p => p.Cachorros)
                    .HasForeignKey(c => c.PetShopId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}