using System.Text;
using Microsoft.EntityFrameworkCore;
using Petiatra.Web.Models;

namespace Petiatra.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Veterinario> Veterinarios => Set<Veterinario>();
    public DbSet<Tutor> Tutores => Set<Tutor>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Consulta> Consultas => Set<Consulta>();
    public DbSet<Vacina> Vacinas => Set<Vacina>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        UsarNomesEmSnakeCase(modelBuilder);
    }

    /// <summary>Tabelas e colunas em snake_case, como no DER (ex.: DataHora -> data_hora).</summary>
    private static void UsarNomesEmSnakeCase(ModelBuilder modelBuilder)
    {
        foreach (var entidade in modelBuilder.Model.GetEntityTypes())
        {
            entidade.SetTableName(ParaSnakeCase(entidade.ClrType.Name));

            foreach (var propriedade in entidade.GetProperties())
                propriedade.SetColumnName(ParaSnakeCase(propriedade.Name));

            foreach (var chave in entidade.GetKeys())
                chave.SetName($"pk_{entidade.GetTableName()}");

            foreach (var estrangeira in entidade.GetForeignKeys())
                estrangeira.SetConstraintName($"fk_{entidade.GetTableName()}_{ParaSnakeCase(estrangeira.PrincipalEntityType.ClrType.Name)}");

            foreach (var indice in entidade.GetIndexes())
                indice.SetDatabaseName($"ix_{entidade.GetTableName()}_{string.Join("_", indice.Properties.Select(p => ParaSnakeCase(p.Name)))}");
        }
    }

    internal static string ParaSnakeCase(string nome)
    {
        var texto = new StringBuilder(nome.Length + 8);
        for (var i = 0; i < nome.Length; i++)
        {
            var letra = nome[i];
            if (char.IsUpper(letra) && i > 0)
                texto.Append('_');
            texto.Append(char.ToLowerInvariant(letra));
        }
        return texto.ToString();
    }
}
