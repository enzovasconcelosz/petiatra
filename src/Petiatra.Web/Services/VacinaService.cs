using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Repositories.Interfaces;
using Petiatra.Web.Services.Interfaces;

namespace Petiatra.Web.Services;

public class VacinaService(
    IVacinaRepositorio vacinas,
    IPetRepositorio pets,
    IVeterinarioRepositorio veterinarios,
    TimeProvider relogio) : IVacinaService
{
    private static readonly string[] NomesComuns =
    [
        "Antirrábica", "V8 (Polivalente)", "V10 (Polivalente)", "Gripe canina", "Giárdia",
        "Leishmaniose", "V3 (Tríplice felina)", "V4 (Quádrupla felina)", "V5 (Quíntupla felina)"
    ];

    public Task<Pagina<ItemVacina>> ListarAsync(FiltroVacinas filtro) =>
        vacinas.BuscarPaginadoAsync(filtro, relogio.Hoje());

    public Task<IReadOnlyList<Vacina>> ListarAtrasadasAsync() => vacinas.ListarAtrasadasAsync(relogio.Hoje());

    public async Task<IReadOnlyList<string>> ListarNomesSugeridosAsync()
    {
        var usados = await vacinas.ListarNomesUsadosAsync();
        return NomesComuns.Concat(usados).Distinct(StringComparer.OrdinalIgnoreCase).Order().ToList();
    }

    public Task<Vacina?> ObterAsync(int id) => vacinas.ObterCompletaAsync(id);

    public async Task<Resultado<Vacina>> RegistrarAsync(Vacina vacina)
    {
        var validacao = await ValidarAsync(vacina);
        if (!validacao.Sucesso)
            return Resultado<Vacina>.De(validacao);

        vacina.CriadoEm = relogio.Agora();
        await vacinas.AdicionarAsync(vacina);
        return Resultado<Vacina>.Ok(vacina);
    }

    public async Task<Resultado<Vacina>> AtualizarAsync(Vacina vacina)
    {
        var validacao = await ValidarAsync(vacina);
        if (!validacao.Sucesso)
            return Resultado<Vacina>.De(validacao);

        await vacinas.AtualizarAsync(vacina);
        return Resultado<Vacina>.Ok(vacina);
    }

    public async Task<Resultado> ExcluirAsync(int id)
    {
        var vacina = await vacinas.ObterPorIdAsync(id);
        if (vacina is null)
            return Resultado.Falha("Vacina não encontrada.");

        await vacinas.RemoverAsync(vacina);
        return Resultado.Ok();
    }

    private async Task<Resultado> ValidarAsync(Vacina vacina)
    {
        vacina.Nome = vacina.Nome.Trim();
        vacina.Lote = Documento.Limpar(vacina.Lote);

        if (await pets.ObterPorIdAsync(vacina.PetId) is null)
            return Resultado.Falha("Selecione um pet cadastrado.", nameof(Vacina.PetId));

        if (await veterinarios.ObterPorIdAsync(vacina.VeterinarioId) is null)
            return Resultado.Falha("Selecione um veterinário cadastrado.", nameof(Vacina.VeterinarioId));

        // RN06: aplicação não pode ser futura e o reforço vem depois da aplicação
        if (vacina.DataAplicacao > relogio.Hoje())
            return Resultado.Falha("A data de aplicação não pode ser futura.", nameof(Vacina.DataAplicacao));

        if (vacina.DataReforco is { } reforco && reforco <= vacina.DataAplicacao)
            return Resultado.Falha("A data de reforço deve ser posterior à data de aplicação.", nameof(Vacina.DataReforco));

        return Resultado.Ok();
    }
}
