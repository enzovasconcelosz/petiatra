using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;

namespace Petiatra.Web.Services.Interfaces;

public interface IConsultaService
{
    Task<Pagina<Consulta>> ListarAsync(FiltroConsultas filtro);
    Task<Consulta?> ObterAsync(int id);
    Task<IReadOnlyList<Consulta>> ListarOcupadosAsync(int veterinarioId, DateOnly dia);
    Task<Resultado<Consulta>> AgendarAsync(Consulta consulta);
    Task<Resultado<Consulta>> ReagendarAsync(int id, DateTime dataHora, int veterinarioId, int petId, string motivo);
    Task<Resultado> RegistrarAtendimentoAsync(int id, string diagnostico, string tratamento, decimal? valor);
    Task<Resultado> CancelarAsync(int id);
    Task<Resultado> ExcluirAsync(int id);
}
