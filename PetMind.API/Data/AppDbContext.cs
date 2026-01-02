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
            modelBuilder.Entity<Horario>(entity =>
            {
                // Propriedades normais
                entity.Property(h => h.ServicoBaseSelecionado).IsRequired();
                entity.Property(h => h.Data).IsRequired();
                
                // Relacionamento com PetShop
                entity.HasOne(h => h.PetShop)
                    .WithMany(p => p.Horarios)
                    .HasForeignKey(h => h.PetShopId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configura o relacionamento muitos-para-muitos
            modelBuilder.Entity<Horario>()
                .HasMany(h => h.Cachorros)
                .WithMany()
                .UsingEntity(j => j.ToTable("HorarioCachorros"));

            // Configura decimal
            modelBuilder.Entity<Horario>()
                .Property(h => h.ValorTotal)
                .HasPrecision(18, 2);
        }
    }
}