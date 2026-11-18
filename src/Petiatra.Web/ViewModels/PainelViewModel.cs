using Petiatra.Web.Models;
using Petiatra.Web.Models.Enums;

namespace Petiatra.Web.ViewModels;

public class PainelViewModel
{
    public DateOnly Hoje { get; init; }
    public int TotalTutores { get; init; }
    public int TotalPets { get; init; }
    public int TotalVacinasAtrasadas { get; init; }
    public IReadOnlyList<Consulta> ConsultasDoDia { get; init; } = [];
    public IReadOnlyList<Vacina> VacinasAtrasadas { get; init; } = [];
    public IReadOnlyList<Vacina> ProximosReforcos { get; init; } = [];

    public int ConsultasAgendadasHoje => ConsultasDoDia.Count(c => c.Status == StatusConsulta.Agendada);
}
