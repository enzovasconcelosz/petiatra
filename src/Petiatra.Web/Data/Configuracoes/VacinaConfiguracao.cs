using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petiatra.Web.Models;

namespace Petiatra.Web.Data.Configuracoes;

public class VacinaConfiguracao : IEntityTypeConfiguration<Vacina>
{
    public void Configure(EntityTypeBuilder<Vacina> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Nome).HasMaxLength(80).IsRequired();
        builder.Property(v => v.Lote).HasMaxLength(40);

        builder.HasOne(v => v.Veterinario)
               .WithMany()
               .HasForeignKey(v => v.VeterinarioId)
               .OnDelete(DeleteBehavior.Restrict);

        // usado na lista de vacinas atrasadas (RN07)
        builder.HasIndex(v => v.DataReforco);
    }
}
