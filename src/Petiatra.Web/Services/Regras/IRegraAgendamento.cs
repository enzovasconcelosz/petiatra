using Petiatra.Web.Models;

namespace Petiatra.Web.Services.Regras;

/// <summary>
/// Uma regra verificada antes de gravar uma consulta. Para criar uma regra nova basta
/// escrever outra classe e registrá-la no Program.cs, sem alterar o ConsultaService (SOLID: aberto/fechado).
/// </summary>
public interface IRegraAgendamento
{
    Task<Resultado> ValidarAsync(Consulta consulta);
}
