using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petiatra.Web.Models;

namespace Petiatra.Web.Data.Configuracoes;

public class ConsultaConfiguracao : IEntityTypeConfiguration<Consulta>
{
    public void Configure(EntityTypeBuilder<Consulta> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Motivo).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Diagnostico).HasMaxLength(2000);
        builder.Property(c => c.Tratamento).HasMaxLength(2000);
        builder.Property(c => c.Valor).HasPrecision(10, 2);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(15).IsRequired();

        builder.HasOne(c => c.Veterinario)
               .WithMany()
               .HasForeignKey(c => c.VeterinarioId)
               .OnDelete(DeleteBehavior.Restrict);

        // acelera a verificação de conflito de horário (RN01)
        builder.HasIndex(c => new { c.VeterinarioId, c.DataHora });
        builder.HasIndex(c => c.DataHora);
    }
}
