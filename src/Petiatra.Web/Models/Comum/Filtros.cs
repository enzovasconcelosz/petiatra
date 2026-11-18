using Petiatra.Web.Models.Enums;

namespace Petiatra.Web.Models.Comum;

public sealed record FiltroTutores(string? Busca, int Pagina = 1);

public sealed record FiltroPets(string? Busca, Especie? Especie, int Pagina = 1);

public sealed record FiltroConsultas(DateOnly? Data, int? VeterinarioId, StatusConsulta? Status, string? Busca, int Pagina = 1);

public enum FiltroSituacaoVacina
{
    Todas,
    Atrasadas,
    ProximosReforcos
}

public sealed record FiltroVacinas(FiltroSituacaoVacina Situacao, string? Busca, int Pagina = 1);

/// <summary>Uma dose de vacina e a indicação de que já existe dose posterior (reforço aplicado).</summary>
public sealed record ItemVacina(Vacina Vacina, bool ReforcoAplicado);
