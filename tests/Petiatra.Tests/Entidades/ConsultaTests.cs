using Petiatra.Web.Models;
using Petiatra.Web.Models.Enums;

namespace Petiatra.Tests.Entidades;

public class ConsultaTests
{
    private static readonly TimeSpan TrintaMinutos = TimeSpan.FromMinutes(30);

    private static Consulta Nova(int id, int veterinarioId, int hora, int minuto) => new()
    {
        Id = id,
        VeterinarioId = veterinarioId,
        DataHora = new DateTime(2026, 11, 18, hora, minuto, 0),
        Motivo = "Check-up"
    };

    [Theory]
    [InlineData(10, 0, true)]
    [InlineData(10, 29, true)]
    [InlineData(9, 31, true)]
    [InlineData(10, 30, false)]
    [InlineData(9, 30, false)]
    public void ConflitaCom_respeita_intervalo_de_30_minutos_RN01(int hora, int minuto, bool conflita)
    {
        var existente = Nova(1, veterinarioId: 1, 10, 0);
        var nova = Nova(0, veterinarioId: 1, hora, minuto);

        Assert.Equal(conflita, nova.ConflitaCom(existente, TrintaMinutos));
    }

    [Fact]
    public void ConflitaCom_ignora_outro_veterinario()
    {
        Assert.False(Nova(0, 2, 10, 0).ConflitaCom(Nova(1, 1, 10, 0), TrintaMinutos));
    }

    [Fact]
    public void ConflitaCom_ignora_consulta_cancelada()
    {
        var cancelada = Nova(1, 1, 10, 0);
        cancelada.Cancelar();

        Assert.False(Nova(0, 1, 10, 0).ConflitaCom(cancelada, TrintaMinutos));
    }

    [Fact]
    public void ConflitaCom_ignora_a_propria_consulta_ao_reagendar()
    {
        var consulta = Nova(5, 1, 10, 0);

        Assert.False(consulta.ConflitaCom(Nova(5, 1, 10, 10), TrintaMinutos));
    }

    [Fact]
    public void RegistrarAtendimento_marca_como_realizada()
    {
        var consulta = Nova(1, 1, 10, 0);

        consulta.RegistrarAtendimento(" Otite ", "Limpeza", 120m);

        Assert.Equal(StatusConsulta.Realizada, consulta.Status);
        Assert.Equal("Otite", consulta.Diagnostico);
        Assert.Equal(120m, consulta.Valor);
    }

    [Fact]
    public void Consulta_cancelada_nao_recebe_atendimento()
    {
        var consulta = Nova(1, 1, 10, 0);
        consulta.Cancelar();

        Assert.Throws<InvalidOperationException>(() => consulta.RegistrarAtendimento("x", "y", null));
    }

    [Fact]
    public void Consulta_realizada_nao_pode_ser_cancelada()
    {
        var consulta = Nova(1, 1, 10, 0);
        consulta.RegistrarAtendimento("x", "y", null);

        Assert.False(consulta.PodeSerCancelada);
        Assert.Throws<InvalidOperationException>(consulta.Cancelar);
    }
}
