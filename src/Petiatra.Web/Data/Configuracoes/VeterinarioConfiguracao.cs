using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petiatra.Web.Models;

namespace Petiatra.Web.Data.Configuracoes;

public class VeterinarioConfiguracao : IEntityTypeConfiguration<Veterinario>
{
    public void Configure(EntityTypeBuilder<Veterinario> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Nome).HasMaxLength(100).IsRequired();
        builder.Property(v => v.Email).HasMaxLength(120).IsRequired();
        builder.Property(v => v.Crmv).HasMaxLength(20).IsRequired();
        builder.Property(v => v.SenhaHash).HasMaxLength(255).IsRequired();

        // RN03: e-mail e CRMV únicos
        builder.HasIndex(v => v.Email).IsUnique();
        builder.HasIndex(v => v.Crmv).IsUnique();
    }
}
