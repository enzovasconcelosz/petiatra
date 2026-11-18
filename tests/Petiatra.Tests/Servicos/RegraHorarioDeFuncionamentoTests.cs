using Microsoft.Extensions.Options;
using Petiatra.Web.Models;
using Petiatra.Web.Services;
using Petiatra.Web.Services.Regras;

namespace Petiatra.Tests.Servicos;

public class RegraHorarioDeFuncionamentoTests
{
    private readonly RegraHorarioDeFuncionamento _regra = new(Options.Create(new HorarioFuncionamentoOpcoes()));

    [Theory]
    [InlineData(7, 59, false)]
    [InlineData(8, 0, true)]
    [InlineData(12, 15, true)]
    [InlineData(17, 30, true)]
    [InlineData(17, 31, false)]
    [InlineData(19, 0, false)]
    public async Task Aceita_somente_horarios_do_expediente_RN02(int hora, int minuto, bool aceito)
    {
        var consulta = new Consulta { DataHora = new DateTime(2026, 11, 18, hora, minuto, 0) };

        var resultado = await _regra.ValidarAsync(consulta);

        Assert.Equal(aceito, resultado.Sucesso);
    }

    [Fact]
    public async Task Expediente_vem_da_configuracao()
    {
        var regra = new RegraHorarioDeFuncionamento(Options.Create(new HorarioFuncionamentoOpcoes
        {
            Abertura = new TimeOnly(10, 0),
            Fechamento = new TimeOnly(14, 0)
        }));

        var resultado = await regra.ValidarAsync(new Consulta { DataHora = new DateTime(2026, 11, 18, 9, 0, 0) });

        Assert.False(resultado.Sucesso);
        Assert.Contains("10:00", resultado.Erro);
    }
}
