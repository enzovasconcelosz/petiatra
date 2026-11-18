using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Petiatra.Web.Models;

namespace Petiatra.Web.Data.Configuracoes;

public class TutorConfiguracao : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Nome).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Cpf).HasMaxLength(11).IsFixedLength().IsRequired();
        builder.Property(t => t.Telefone).HasMaxLength(20).IsRequired();
        builder.Property(t => t.Email).HasMaxLength(120);
        builder.Property(t => t.Endereco).HasMaxLength(200);

        // RN03: CPF único
        builder.HasIndex(t => t.Cpf).IsUnique();
        builder.HasIndex(t => t.Nome);

        // RN04: o banco também impede excluir um tutor que ainda tem pets
        builder.HasMany(t => t.Pets)
               .WithOne(p => p.Tutor)
               .HasForeignKey(p => p.TutorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
