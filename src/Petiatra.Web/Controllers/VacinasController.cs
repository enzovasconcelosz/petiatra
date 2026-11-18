using Microsoft.AspNetCore.Mvc;
using Petiatra.Web.Infraestrutura;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Services;
using Petiatra.Web.Services.Interfaces;
using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Controllers;

/// <summary>UC05: manter vacinas.</summary>
public class VacinasController(
    IVacinaService vacinas,
    IPetService pets,
    IAuthService veterinarios,
    TimeProvider relogio) : AppController
{
    public async Task<IActionResult> Index(FiltroSituacaoVacina situacao, string? busca, int pagina = 1)
    {
        var filtro = new FiltroVacinas(situacao, busca, pagina);
        return View(new VacinasIndexViewModel(await vacinas.ListarAsync(filtro), filtro, relogio.Hoje()));
    }

    [HttpGet]
    public async Task<IActionResult> Criar(int? petId)
    {
        var model = new VacinaFormViewModel
        {
            VeterinarioId = User.ObterVeterinarioId(),
            DataAplicacao = relogio.Hoje()
        };

        if (petId is { } id && await pets.ObterAsync(id) is { } pet)
        {
            model.PetId = pet.Id;
            model.PetDescricao = ConsultaFormViewModel.DescreverPet(pet);
        }

        return await FormularioAsync(model);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(VacinaFormViewModel model, string? acao)
    {
        if (!ModelState.IsValid)
            return await FormularioAsync(model);

        var resultado = await vacinas.RegistrarAsync(model.ParaEntidade());
        if (!resultado.Sucesso)
        {
            AdicionarErro(resultado);
            return await FormularioAsync(model);
        }

        MensagemSucesso($"Vacina {resultado.Valor!.Nome} registrada.");
        return CadastrarOutro(acao)
            ? RedirectToAction(nameof(Criar), new { petId = model.PetId })
            : RedirectToAction("Detalhes", "Pets", new { id = model.PetId });
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var vacina = await vacinas.ObterAsync(id);
        return vacina is null ? NotFound() : await FormularioAsync(VacinaFormViewModel.De(vacina));
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, VacinaFormViewModel model)
    {
        var vacina = await vacinas.ObterAsync(id);
        if (vacina is null)
            return NotFound();

        model.Id = id;
        if (!ModelState.IsValid)
            return await FormularioAsync(model);

        model.CopiarPara(vacina);
        var resultado = await vacinas.AtualizarAsync(vacina);
        if (!resultado.Sucesso)
        {
            AdicionarErro(resultado);
            return await FormularioAsync(model);
        }

        MensagemSucesso($"Vacina {vacina.Nome} atualizada.");
        return RedirectToAction("Detalhes", "Pets", new { id = vacina.PetId });
    }

    [HttpGet]
    public async Task<IActionResult> Excluir(int id)
    {
        var vacina = await vacinas.ObterAsync(id);
        if (vacina is null)
            return NotFound();

        return View("ConfirmarExclusao", new ConfirmarExclusaoViewModel(
            "Excluir vacina",
            $"{vacina.Nome} · {vacina.Pet?.Nome} · aplicada em {Formatos.Data(vacina.DataAplicacao)}",
            ["A dose sai da carteira de vacinação do pet.", "Esta ação não pode ser desfeita."],
            Url.Action("Detalhes", "Pets", new { id = vacina.PetId })!));
    }

    [HttpPost, ActionName(nameof(Excluir))]
    public async Task<IActionResult> ExcluirConfirmado(int id)
    {
        var vacina = await vacinas.ObterAsync(id);
        var resultado = await vacinas.ExcluirAsync(id);
        var destino = vacina is null
            ? RedirectToAction(nameof(Index))
            : RedirectToAction("Detalhes", "Pets", new { id = vacina.PetId });
        return MensagemEVoltar(resultado, "Vacina excluída.", destino);
    }

    private async Task<IActionResult> FormularioAsync(VacinaFormViewModel model)
    {
        model.Veterinarios = await veterinarios.ListarVeterinariosAsync();
        model.NomesSugeridos = await vacinas.ListarNomesSugeridosAsync();
        if (model.PetId > 0 && string.IsNullOrEmpty(model.PetDescricao) && await pets.ObterAsync(model.PetId) is { } pet)
            model.PetDescricao = ConsultaFormViewModel.DescreverPet(pet);

        return View("Form", model);
    }
}
