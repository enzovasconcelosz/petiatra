using Microsoft.Extensions.Options;
using Petiatra.Web.Models;

namespace Petiatra.Web.Services.Regras;

/// <summary>RN02: a consulta precisa começar e terminar dentro do expediente da clínica.</summary>
public class RegraHorarioDeFuncionamento(IOptions<HorarioFuncionamentoOpcoes> opcoes) : IRegraAgendamento
{
    private readonly HorarioFuncionamentoOpcoes _horario = opcoes.Value;

    public Task<Resultado> ValidarAsync(Consulta consulta)
    {
        var inicio = TimeOnly.FromDateTime(consulta.DataHora);
        var ultimoHorario = _horario.Fechamento.Add(-_horario.IntervaloEntreConsultas);

        var dentroDoExpediente = inicio >= _horario.Abertura && inicio <= ultimoHorario;
        var resultado = dentroDoExpediente
            ? Resultado.Ok()
            : Resultado.Falha(
                $"Fora do horário de funcionamento. Escolha um horário entre {_horario.Abertura:HH\\:mm} e {ultimoHorario:HH\\:mm}.",
                nameof(Consulta.DataHora));

        return Task.FromResult(resultado);
    }
}
