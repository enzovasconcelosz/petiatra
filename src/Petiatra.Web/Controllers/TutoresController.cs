using Microsoft.AspNetCore.Mvc;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Services.Interfaces;
using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Controllers;

/// <summary>UC02: manter tutores.</summary>
public class TutoresController(ITutorService tutores) : AppController
{
    public async Task<IActionResult> Index(string? busca, int pagina = 1)
    {
        var resultado = await tutores.ListarAsync(new FiltroTutores(busca, pagina));
        return View(new TutoresIndexViewModel(resultado, busca));
    }

    public async Task<IActionResult> Detalhes(int id)
    {
        var tutor = await tutores.ObterComPetsAsync(id);
        return tutor is null ? NotFound() : View(tutor);
    }

    [HttpGet]
    public IActionResult Criar() => View("Form", new TutorFormViewModel());

    [HttpPost]
    public async Task<IActionResult> Criar(TutorFormViewModel model, string? acao)
    {
        if (!ModelState.IsValid)
            return View("Form", model);

        var resultado = await tutores.SalvarAsync(model.ParaEntidade());
        if (!resultado.Sucesso)
        {
            AdicionarErro(resultado);
            return View("Form", model);
        }

        MensagemSucesso($"Tutor {resultado.Valor!.Nome} cadastrado.");
        return CadastrarOutro(acao)
            ? RedirectToAction(nameof(Criar))
            : RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var tutor = await tutores.ObterAsync(id);
        return tutor is null ? NotFound() : View("Form", TutorFormViewModel.De(tutor));
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, TutorFormViewModel model)
    {
        var tutor = await tutores.ObterAsync(id);
        if (tutor is null)
            return NotFound();

        if (!ModelState.IsValid)
            return View("Form", model);

        model.CopiarPara(tutor);
        var resultado = await tutores.SalvarAsync(tutor);
        if (!resultado.Sucesso)
        {
            AdicionarErro(resultado);
            return View("Form", model);
        }

        MensagemSucesso($"Dados de {tutor.Nome} atualizados.");
        return RedirectToAction(nameof(Detalhes), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Excluir(int id)
    {
        var tutor = await tutores.ObterComPetsAsync(id);
        if (tutor is null)
            return NotFound();

        var possuiPets = tutor.Pets.Count > 0;
        var avisos = possuiPets
            ? new[] { $"{tutor.Nome} possui {tutor.Pets.Count} pet(s): {string.Join(", ", tutor.Pets.Select(p => p.Nome))}. Exclua ou transfira os pets antes de excluir o tutor (RN04)." }
            : ["Esta ação não pode ser desfeita."];

        return View("ConfirmarExclusao", new ConfirmarExclusaoViewModel(
            "Excluir tutor",
            $"{tutor.Nome} · CPF {Documento.FormatarCpf(tutor.Cpf)}",
            avisos,
            Url.Action(nameof(Detalhes), new { id })!,
            Bloqueada: possuiPets));
    }

    [HttpPost, ActionName(nameof(Excluir))]
    public async Task<IActionResult> ExcluirConfirmado(int id)
    {
        var resultado = await tutores.ExcluirAsync(id);
        return resultado.Sucesso
            ? MensagemEVoltar(resultado, "Tutor excluído.", RedirectToAction(nameof(Index)))
            : MensagemEVoltar(resultado, string.Empty, RedirectToAction(nameof(Detalhes), new { id }));
    }
}
