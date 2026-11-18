using Microsoft.EntityFrameworkCore;
using Petiatra.Tests.Apoio;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Enums;

namespace Petiatra.Tests.Servicos;

public class AuthEAtendimentoServiceTests : IDisposable
{
    private readonly Cenario _cenario = new();

    private static Veterinario NovoVeterinario(string email = "Ana@Teste.com", string crmv = "crmv-pr 1") =>
        new() { Nome = "Dra. Ana", Email = email, Crmv = crmv };

    [Fact]
    public async Task Cadastro_guarda_hash_e_nunca_a_senha()
    {
        var resultado = await _cenario.AuthService.CadastrarAsync(NovoVeterinario(), "senhaSegura1");

        Assert.True(resultado.Sucesso);
        Assert.NotEqual("senhaSegura1", resultado.Valor!.SenhaHash);
        Assert.DoesNotContain("senhaSegura1", resultado.Valor.SenhaHash);
        Assert.Equal("ana@teste.com", resultado.Valor.Email);
    }

    [Fact]
    public async Task Autentica_com_senha_correta_e_recusa_a_errada()
    {
        await _cenario.AuthService.CadastrarAsync(NovoVeterinario(), "senhaSegura1");

        Assert.True((await _cenario.AuthService.AutenticarAsync("ANA@teste.com ", "senhaSegura1")).Sucesso);
        Assert.False((await _cenario.AuthService.AutenticarAsync("ana@teste.com", "errada")).Sucesso);
        Assert.False((await _cenario.AuthService.AutenticarAsync("nao@existe.com", "senhaSegura1")).Sucesso);
    }

    [Fact]
    public async Task Recusa_email_e_crmv_repetidos_RN03()
    {
        await _cenario.AuthService.CadastrarAsync(NovoVeterinario(), "senhaSegura1");

        var emailRepetido = await _cenario.AuthService.CadastrarAsync(NovoVeterinario(crmv: "outro"), "senhaSegura1");
        var crmvRepetido = await _cenario.AuthService.CadastrarAsync(NovoVeterinario(email: "outro@teste.com"), "senhaSegura1");

        Assert.Equal(nameof(Veterinario.Email), emailRepetido.Campo);
        Assert.Equal(nameof(Veterinario.Crmv), crmvRepetido.Campo);
    }

    [Fact]
    public async Task Novo_atendimento_grava_tutor_pet_e_consulta_RF13()
    {
        var veterinario = await _cenario.CriarVeterinarioAsync();
        var tutor = new Tutor { Nome = "Patrícia", Cpf = "93541134780", Telefone = "1" };
        var pet = new Pet { Nome = "Pipoca", Especie = Especie.Gato };
        var consulta = new Consulta { VeterinarioId = veterinario.Id, DataHora = _cenario.Hoje(11), Motivo = "Primeira consulta" };

        var resultado = await _cenario.AtendimentoService.RegistrarAsync(tutor, pet, consulta);

        Assert.True(resultado.Sucesso);
        await using var conferencia = _cenario.NovoContexto();
        var gravada = await conferencia.Consultas.Include(c => c.Pet).ThenInclude(p => p!.Tutor).SingleAsync();
        Assert.Equal("Pipoca", gravada.Pet!.Nome);
        Assert.Equal("Patrícia", gravada.Pet.Tutor!.Nome);
    }

    [Fact]
    public async Task Novo_atendimento_desfaz_tudo_se_o_horario_for_recusado()
    {
        var veterinario = await _cenario.CriarVeterinarioAsync();
        var tutor = new Tutor { Nome = "Patrícia", Cpf = "93541134780", Telefone = "1" };
        var pet = new Pet { Nome = "Pipoca", Especie = Especie.Gato };
        var consulta = new Consulta { VeterinarioId = veterinario.Id, DataHora = _cenario.Hoje(20), Motivo = "Fora do expediente" };

        var resultado = await _cenario.AtendimentoService.RegistrarAsync(tutor, pet, consulta);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Consulta.DataHora", resultado.Campo);
        await using var conferencia = _cenario.NovoContexto();
        Assert.False(await conferencia.Tutores.AnyAsync());
        Assert.False(await conferencia.Pets.AnyAsync());
    }

    public void Dispose() => _cenario.Dispose();
}
