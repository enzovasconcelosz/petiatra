using Microsoft.AspNetCore.Mvc;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Models.Enums;
using Petiatra.Web.Services.Interfaces;
using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Controllers;

/// <summary>RF14: busca rápida global (atalho "/" ou Ctrl+K) e listas para os campos de seleção.</summary>
public class BuscaController(IBuscaService busca, IPetService pets) : AppController
{
    private const int LimitePagina = 20;
    private const int LimiteSugestoes = 6;

    public async Task<IActionResult> Index(string? termo) => View(await busca.BuscarAsync(termo, LimitePagina));

    [HttpGet]
    public async Task<IActionResult> Sugestoes(string? termo)
    {
        var resultado = await busca.BuscarAsync(termo, LimiteSugestoes);
        return Json(new
        {
            tutores = resultado.Tutores.Select(t => new
            {
                id = t.Id,
                texto = t.Nome,
                detalhe = $"CPF {Documento.FormatarCpf(t.Cpf)} · {t.Telefone}",
                url = Url.Action("Detalhes", "Tutores", new { id = t.Id })
            }),
            pets = resultado.Pets.Select(p => new
            {
                id = p.Id,
                texto = p.Nome,
                detalhe = $"{p.Especie.Rotulo()} · tutor {p.Tutor?.Nome}",
                url = Url.Action("Detalhes", "Pets", new { id = p.Id })
            })
        });
    }

    /// <summary>Opções do seletor de tutor.</summary>
    [HttpGet]
    public async Task<IActionResult> Tutores(string? termo)
    {
        var resultado = await busca.BuscarAsync(termo, LimiteSugestoes);
        return Json(resultado.Tutores.Select(t => new { id = t.Id, texto = PetFormViewModel.Descrever(t) }));
    }

    /// <summary>Opções do seletor de pet.</summary>
    [HttpGet]
    public async Task<IActionResult> Pets(string? termo)
    {
        var resultado = await busca.BuscarAsync(termo, LimiteSugestoes);
        return Json(resultado.Pets.Select(p => new { id = p.Id, texto = ConsultaFormViewModel.DescreverPet(p) }));
    }

    /// <summary>Pets de um tutor, usados no passo 2 do assistente "Novo atendimento".</summary>
    [HttpGet]
    public async Task<IActionResult> PetsDoTutor(int tutorId)
    {
        var lista = await pets.ListarDoTutorAsync(tutorId);
        return Json(lista.Select(p => new { id = p.Id, texto = $"{p.Nome} ({p.Especie.Rotulo()})" }));
    }
}
