using Petiatra.Tests.Apoio;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;

namespace Petiatra.Tests.Servicos;

public class VacinaServiceTests : IDisposable
{
    private readonly Cenario _cenario = new();
    private DateOnly Hoje => _cenario.Relogio.Hoje;

    private async Task<(Pet Pet, Veterinario Veterinario)> PrepararAsync()
    {
        var veterinario = await _cenario.CriarVeterinarioAsync();
        var pet = await _cenario.CriarPetAsync(await _cenario.CriarTutorAsync());
        return (pet, veterinario);
    }

    private static Vacina Dose(Pet pet, Veterinario veterinario, DateOnly aplicacao, DateOnly? reforco, string nome = "Antirrábica") =>
        new() { PetId = pet.Id, VeterinarioId = veterinario.Id, Nome = nome, DataAplicacao = aplicacao, DataReforco = reforco };

    [Fact]
    public async Task Recusa_aplicacao_no_futuro_RN06()
    {
        var (pet, veterinario) = await PrepararAsync();

        var resultado = await _cenario.VacinaService.RegistrarAsync(Dose(pet, veterinario, Hoje.AddDays(1), null));

        Assert.False(resultado.Sucesso);
        Assert.Equal(nameof(Vacina.DataAplicacao), resultado.Campo);
    }

    [Fact]
    public async Task Recusa_reforco_antes_ou_no_dia_da_aplicacao_RN06()
    {
        var (pet, veterinario) = await PrepararAsync();

        var resultado = await _cenario.VacinaService.RegistrarAsync(Dose(pet, veterinario, Hoje, Hoje));

        Assert.False(resultado.Sucesso);
        Assert.Equal(nameof(Vacina.DataReforco), resultado.Campo);
    }

    [Fact]
    public async Task Lista_de_atrasadas_mostra_reforco_vencido_sem_dose_posterior_RN07()
    {
        var (pet, veterinario) = await PrepararAsync();
        await _cenario.VacinaService.RegistrarAsync(Dose(pet, veterinario, Hoje.AddYears(-1), Hoje.AddDays(-10)));
        await _cenario.VacinaService.RegistrarAsync(Dose(pet, veterinario, Hoje.AddMonths(-2), Hoje.AddMonths(10), "V10"));

        var atrasadas = await _cenario.VacinaService.ListarAtrasadasAsync();

        Assert.Equal("Antirrábica", Assert.Single(atrasadas).Nome);
    }

    [Fact]
    public async Task Dose_posterior_tira_a_vacina_da_lista_de_atrasadas_RN07()
    {
        var (pet, veterinario) = await PrepararAsync();
        await _cenario.VacinaService.RegistrarAsync(Dose(pet, veterinario, Hoje.AddYears(-1), Hoje.AddDays(-10)));
        await _cenario.VacinaService.RegistrarAsync(Dose(pet, veterinario, Hoje, Hoje.AddYears(1)));

        Assert.Empty(await _cenario.VacinaService.ListarAtrasadasAsync());

        var todas = await _cenario.VacinaService.ListarAsync(new FiltroVacinas(FiltroSituacaoVacina.Todas, null));
        Assert.Contains(todas.Itens, item => item.ReforcoAplicado);
    }

    [Fact]
    public async Task Proximos_reforcos_lista_os_30_dias_seguintes()
    {
        var (pet, veterinario) = await PrepararAsync();
        await _cenario.VacinaService.RegistrarAsync(Dose(pet, veterinario, Hoje.AddYears(-1), Hoje.AddDays(20)));
        await _cenario.VacinaService.RegistrarAsync(Dose(pet, veterinario, Hoje.AddMonths(-1), Hoje.AddDays(60), "V10"));

        var proximos = await _cenario.VacinaService.ListarAsync(new FiltroVacinas(FiltroSituacaoVacina.ProximosReforcos, null));

        Assert.Equal("Antirrábica", Assert.Single(proximos.Itens).Vacina.Nome);
    }

    public void Dispose() => _cenario.Dispose();
}
