using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petiatra.Web.Models;

namespace Petiatra.Web.Data.Configuracoes;

public class PetConfiguracao : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nome).HasMaxLength(60).IsRequired();
        builder.Property(p => p.Especie).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.Raca).HasMaxLength(60);
        builder.Property(p => p.Sexo).HasConversion<string>().HasMaxLength(15).IsRequired();
        builder.Property(p => p.PesoKg).HasPrecision(6, 2);
        builder.Property(p => p.Observacoes).HasMaxLength(500);
        builder.HasIndex(p => p.Nome);

        // RN05: excluir o pet exclui também o histórico
        builder.HasMany(p => p.Consultas)
               .WithOne(c => c.Pet)
               .HasForeignKey(c => c.PetId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Vacinas)
               .WithOne(v => v.Pet)
               .HasForeignKey(v => v.PetId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
