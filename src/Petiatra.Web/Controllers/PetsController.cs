using Microsoft.AspNetCore.Mvc;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Models.Enums;
using Petiatra.Web.Services.Interfaces;
using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Controllers;

/// <summary>UC03: manter pets.</summary>
public class PetsController(IPetService pets, ITutorService tutores) : AppController
{
    public async Task<IActionResult> Index(string? busca, Especie? especie, int pagina = 1)
    {
        var resultado = await pets.ListarAsync(new FiltroPets(busca, especie, pagina));
        return View(new PetsIndexViewModel(resultado, busca, especie));
    }

    public async Task<IActionResult> Detalhes(int id)
    {
        var ficha = await pets.ObterFichaAsync(id);
        return ficha is null ? NotFound() : View(ficha);
    }

    [HttpGet]
    public async Task<IActionResult> Criar(int? tutorId)
    {
        var model = new PetFormViewModel();
        if (tutorId is { } id && await tutores.ObterAsync(id) is { } tutor)
        {
            model.TutorId = tutor.Id;
            model.TutorDescricao = PetFormViewModel.Descrever(tutor);
        }

        return View("Form", model);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(PetFormViewModel model, string? acao)
    {
        if (!ModelState.IsValid)
            return await FormularioAsync(model);

        var resultado = await pets.SalvarAsync(model.ParaEntidade());
        if (!resultado.Sucesso)
        {
            AdicionarErro(resultado);
            return await FormularioAsync(model);
        }

        MensagemSucesso($"{resultado.Valor!.Nome} cadastrado(a).");
        return CadastrarOutro(acao)
            ? RedirectToAction(nameof(Criar), new { tutorId = model.TutorId })
            : RedirectToAction(nameof(Detalhes), new { id = resultado.Valor.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var pet = await pets.ObterAsync(id);
        return pet is null ? NotFound() : View("Form", PetFormViewModel.De(pet));
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, PetFormViewModel model)
    {
        var pet = await pets.ObterAsync(id);
        if (pet is null)
            return NotFound();

        if (!ModelState.IsValid)
            return await FormularioAsync(model);

        model.CopiarPara(pet);
        var resultado = await pets.SalvarAsync(pet);
        if (!resultado.Sucesso)
        {
            AdicionarErro(resultado);
            return await FormularioAsync(model);
        }

        MensagemSucesso($"Dados de {pet.Nome} atualizados.");
        return RedirectToAction(nameof(Detalhes), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Excluir(int id)
    {
        var ficha = await pets.ObterFichaAsync(id);
        if (ficha is null)
            return NotFound();

        return View("ConfirmarExclusao", new ConfirmarExclusaoViewModel(
            "Excluir pet",
            $"{ficha.Pet.Nome} ({ficha.Pet.Especie.Rotulo()}) · tutor {ficha.Pet.Tutor?.Nome}",
            [
                $"Todo o histórico também será excluído: {ficha.Consultas.Count} consulta(s) e {ficha.Vacinas.Count} vacina(s) (RN05).",
                "Esta ação não pode ser desfeita."
            ],
            Url.Action(nameof(Detalhes), new { id })!));
    }

    [HttpPost, ActionName(nameof(Excluir))]
    public async Task<IActionResult> ExcluirConfirmado(int id)
    {
        var resultado = await pets.ExcluirAsync(id);
        return MensagemEVoltar(resultado, "Pet e histórico excluídos.", RedirectToAction(nameof(Index)));
    }

    /// <summary>Ao voltar com erro, recupera o texto do tutor escolhido no seletor.</summary>
    private async Task<IActionResult> FormularioAsync(PetFormViewModel model)
    {
        if (model.TutorId > 0 && await tutores.ObterAsync(model.TutorId) is { } tutor)
            model.TutorDescricao = PetFormViewModel.Descrever(tutor);

        return View("Form", model);
    }
}
