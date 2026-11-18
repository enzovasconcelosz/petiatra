using Microsoft.EntityFrameworkCore;
using Petiatra.Tests.Apoio;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Enums;

namespace Petiatra.Tests.Servicos;

public class TutorEPetServiceTests : IDisposable
{
    private readonly Cenario _cenario = new();

    [Fact]
    public async Task Salvar_tutor_guarda_cpf_somente_com_digitos()
    {
        var resultado = await _cenario.TutorService.SalvarAsync(new Tutor { Nome = " Mariana ", Cpf = "529.982.247-25", Telefone = "43 9999" });

        Assert.True(resultado.Sucesso);
        Assert.Equal("52998224725", resultado.Valor!.Cpf);
        Assert.Equal("Mariana", resultado.Valor.Nome);
    }

    [Fact]
    public async Task Recusa_cpf_repetido_RN03()
    {
        await _cenario.CriarTutorAsync("52998224725");

        var resultado = await _cenario.TutorService.SalvarAsync(new Tutor { Nome = "Outro", Cpf = "529.982.247-25", Telefone = "1" });

        Assert.False(resultado.Sucesso);
        Assert.Equal(nameof(Tutor.Cpf), resultado.Campo);
    }

    [Fact]
    public async Task Editar_tutor_mantendo_o_proprio_cpf_e_permitido()
    {
        var tutor = await _cenario.CriarTutorAsync();
        tutor.Telefone = "(43) 3333-3333";

        var resultado = await _cenario.TutorService.SalvarAsync(tutor);

        Assert.True(resultado.Sucesso);
    }

    [Fact]
    public async Task Recusa_cpf_invalido()
    {
        var resultado = await _cenario.TutorService.SalvarAsync(new Tutor { Nome = "X", Cpf = "12345678900", Telefone = "1" });

        Assert.False(resultado.Sucesso);
    }

    [Fact]
    public async Task Tutor_com_pets_nao_pode_ser_excluido_RN04()
    {
        var tutor = await _cenario.CriarTutorAsync();
        await _cenario.CriarPetAsync(tutor);

        var resultado = await _cenario.TutorService.ExcluirAsync(tutor.Id);

        Assert.False(resultado.Sucesso);
        await using var conferencia = _cenario.NovoContexto();
        Assert.True(await conferencia.Tutores.AnyAsync(t => t.Id == tutor.Id));
    }

    [Fact]
    public async Task Tutor_sem_pets_pode_ser_excluido()
    {
        var tutor = await _cenario.CriarTutorAsync();

        var resultado = await _cenario.TutorService.ExcluirAsync(tutor.Id);

        Assert.True(resultado.Sucesso);
        await using var conferencia = _cenario.NovoContexto();
        Assert.False(await conferencia.Tutores.AnyAsync());
    }

    [Fact]
    public async Task Excluir_pet_exclui_consultas_e_vacinas_RN05()
    {
        var veterinario = await _cenario.CriarVeterinarioAsync();
        var pet = await _cenario.CriarPetAsync(await _cenario.CriarTutorAsync());
        await _cenario.ConsultaService.AgendarAsync(new Consulta { PetId = pet.Id, VeterinarioId = veterinario.Id, DataHora = _cenario.Hoje(10), Motivo = "x" });
        await _cenario.VacinaService.RegistrarAsync(new Vacina { PetId = pet.Id, VeterinarioId = veterinario.Id, Nome = "V10", DataAplicacao = _cenario.Relogio.Hoje });

        var resultado = await _cenario.PetService.ExcluirAsync(pet.Id);

        Assert.True(resultado.Sucesso);
        await using var conferencia = _cenario.NovoContexto();
        Assert.False(await conferencia.Consultas.AnyAsync());
        Assert.False(await conferencia.Vacinas.AnyAsync());
    }

    [Fact]
    public async Task Recusa_nascimento_no_futuro()
    {
        var tutor = await _cenario.CriarTutorAsync();
        var pet = new Pet { Nome = "Luna", Especie = Especie.Gato, TutorId = tutor.Id, DataNascimento = _cenario.Relogio.Hoje.AddDays(1) };

        var resultado = await _cenario.PetService.SalvarAsync(pet);

        Assert.False(resultado.Sucesso);
        Assert.Equal(nameof(Pet.DataNascimento), resultado.Campo);
    }

    [Fact]
    public async Task Recusa_pet_sem_tutor_cadastrado()
    {
        var resultado = await _cenario.PetService.SalvarAsync(new Pet { Nome = "Luna", TutorId = 999 });

        Assert.False(resultado.Sucesso);
    }

    [Fact]
    public async Task Ficha_do_pet_traz_idade_e_situacao_das_vacinas()
    {
        var veterinario = await _cenario.CriarVeterinarioAsync();
        var tutor = await _cenario.CriarTutorAsync();
        var pet = new Pet { Nome = "Thor", Especie = Especie.Cachorro, TutorId = tutor.Id, DataNascimento = _cenario.Relogio.Hoje.AddYears(-3) };
        await _cenario.PetService.SalvarAsync(pet);
        var hoje = _cenario.Relogio.Hoje;
        await _cenario.VacinaService.RegistrarAsync(new Vacina { PetId = pet.Id, VeterinarioId = veterinario.Id, Nome = "Antirrábica", DataAplicacao = hoje.AddYears(-1), DataReforco = hoje.AddDays(-5) });

        var ficha = await _cenario.PetService.ObterFichaAsync(pet.Id);

        Assert.Equal("3 anos", ficha!.Idade);
        Assert.Equal(SituacaoVacina.Atrasada, Assert.Single(ficha.Vacinas).Situacao);
    }

    public void Dispose() => _cenario.Dispose();
}
