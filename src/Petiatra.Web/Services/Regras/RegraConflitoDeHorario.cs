using Microsoft.Extensions.Options;
using Petiatra.Web.Models;
using Petiatra.Web.Repositories.Interfaces;

namespace Petiatra.Web.Services.Regras;

/// <summary>RN01: o veterinário não pode ter outra consulta a menos de 30 minutos.</summary>
public class RegraConflitoDeHorario(
    IConsultaRepositorio consultas,
    IOptions<HorarioFuncionamentoOpcoes> opcoes) : IRegraAgendamento
{
    private readonly TimeSpan _intervalo = opcoes.Value.IntervaloEntreConsultas;

    public async Task<Resultado> ValidarAsync(Consulta consulta)
    {
        var doDia = await consultas.ListarDoVeterinarioNoDiaAsync(
            consulta.VeterinarioId, DateOnly.FromDateTime(consulta.DataHora));

        var conflito = doDia
            .Where(outra => consulta.ConflitaCom(outra, _intervalo))
            .OrderBy(outra => outra.DataHora)
            .FirstOrDefault();

        return conflito is null
            ? Resultado.Ok()
            : Resultado.Falha(
                $"Horário ocupado: o veterinário já tem uma consulta às {conflito.DataHora:HH\\:mm}. " +
                $"Deixe pelo menos {_intervalo.TotalMinutes:0} minutos entre as consultas.",
                nameof(Consulta.DataHora));
    }
}
