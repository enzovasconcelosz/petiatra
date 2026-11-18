using Petiatra.Tests.Apoio;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Enums;

namespace Petiatra.Tests.Servicos;

public class ConsultaServiceTests : IDisposable
{
    private readonly Cenario _cenario = new();

    private async Task<(Pet Pet, Veterinario Veterinario)> PrepararAsync()
    {
        var veterinario = await _cenario.CriarVeterinarioAsync();
        var pet = await _cenario.CriarPetAsync(await _cenario.CriarTutorAsync());
        return (pet, veterinario);
    }

    private Consulta Nova(Pet pet, Veterinario veterinario, int hora, int minuto = 0) => new()
    {
        PetId = pet.Id,
        VeterinarioId = veterinario.Id,
        DataHora = _cenario.Hoje(hora, minuto),
        Motivo = "Check-up"
    };

    [Fact]
    public async Task Agenda_consulta_valida_como_agendada()
    {
        var (pet, veterinario) = await PrepararAsync();

        var resultado = await _cenario.ConsultaService.AgendarAsync(Nova(pet, veterinario, 10));

        Assert.True(resultado.Sucesso);
        Assert.Equal(StatusConsulta.Agendada, resultado.Valor!.Status);
    }

    [Fact]
    public async Task Recusa_horario_a_menos_de_30_minutos_RN01()
    {
        var (pet, veterinario) = await PrepararAsync();
        await _cenario.ConsultaService.AgendarAsync(Nova(pet, veterinario, 10));

        var resultado = await _cenario.ConsultaService.AgendarAsync(Nova(pet, veterinario, 10, 20));

        Assert.False(resultado.Sucesso);
        Assert.Contains("Horário ocupado", resultado.Erro);
        Assert.Equal(nameof(Consulta.DataHora), resultado.Campo);
    }

    [Fact]
    public async Task Aceita_horario_com_30_minutos_de_intervalo()
    {
        var (pet, veterinario) = await PrepararAsync();
        await _cenario.ConsultaService.AgendarAsync(Nova(pet, veterinario, 10));

        var resultado = await _cenario.ConsultaService.AgendarAsync(Nova(pet, veterinario, 10, 30));

        Assert.True(resultado.Sucesso);
    }

    [Fact]
    public async Task Consulta_cancelada_libera_o_horario()
    {
        var (pet, veterinario) = await PrepararAsync();
        var primeira = await _cenario.ConsultaService.AgendarAsync(Nova(pet, veterinario, 10));
        await _cenario.ConsultaService.CancelarAsync(primeira.Valor!.Id);

        var resultado = await _cenario.ConsultaService.AgendarAsync(Nova(pet, veterinario, 10));

        Assert.True(resultado.Sucesso);
    }

    [Fact]
    public async Task Outro_veterinario_pode_atender_no_mesmo_horario()
    {
        var (pet, veterinario) = await PrepararAsync();
        var outro = await _cenario.CriarVeterinarioAsync("bruno@teste.com", "CRMV-PR 2");
        await _cenario.ConsultaService.AgendarAsync(Nova(pet, veterinario, 10));

        var resultado = await _cenario.ConsultaService.AgendarAsync(Nova(pet, outro, 10));

        Assert.True(resultado.Sucesso);
    }

    [Fact]
    public async Task Recusa_horario_fora_do_expediente_RN02()
    {
        var (pet, veterinario) = await PrepararAsync();

        var resultado = await _cenario.ConsultaService.AgendarAsync(Nova(pet, veterinario, 19));

        Assert.False(resultado.Sucesso);
        Assert.Contains("Fora do horário", resultado.Erro);
    }

    [Fact]
    public async Task Reagendar_nao_conflita_com_a_propria_consulta()
    {
        var (pet, veterinario) = await PrepararAsync();
        var agendada = await _cenario.ConsultaService.AgendarAsync(Nova(pet, veterinario, 10));

        var resultado = await _cenario.ConsultaService.ReagendarAsync(
            agendada.Valor!.Id, _cenario.Hoje(10, 15), veterinario.Id, pet.Id, "Check-up");

        Assert.True(resultado.Sucesso);
    }

    [Fact]
    public async Task Consulta_realizada_nao_pode_ser_cancelada()
    {
        var (pet, veterinario) = await PrepararAsync();
        var agendada = await _cenario.ConsultaService.AgendarAsync(Nova(pet, veterinario, 10));
        await _cenario.ConsultaService.RegistrarAtendimentoAsync(agendada.Valor!.Id, "Otite", "Limpeza", 100m);

        var resultado = await _cenario.ConsultaService.CancelarAsync(agendada.Valor.Id);

        Assert.False(resultado.Sucesso);
    }
    public void Dispose() => _cenario.Dispose();
}
