using Microsoft.AspNetCore.Mvc;
using Petiatra.Web.Infraestrutura;
using Petiatra.Web.Models;
using Petiatra.Web.Services;
using Petiatra.Web.Services.Interfaces;
using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Controllers;

/// <summary>RF13: assistente "Novo atendimento" (tutor, pet e consulta em 3 passos).</summary>
public class AtendimentoController(
    IAtendimentoService atendimento,
    ITutorService tutores,
    IPetService pets,
    IAuthService veterinarios,
    TimeProvider relogio) : AppController
{
    private static readonly Dictionary<string, string> CamposDaTela = new()
    {
        [AtendimentoService.PrefixoConsulta + nameof(Consulta.DataHora)] = nameof(NovoAtendimentoViewModel.Hora),
        [AtendimentoService.PrefixoConsulta + nameof(Consulta.VeterinarioId)] = nameof(NovoAtendimentoViewModel.VeterinarioId)
    };

    [HttpGet]
    public async Task<IActionResult> Novo(int? tutorId)
    {
        var model = new NovoAtendimentoViewModel
        {
            VeterinarioId = User.ObterVeterinarioId(),
            Data = relogio.Hoje()
        };

        if (tutorId is { } id)
            model.TutorExistenteId = id;

        return await FormularioAsync(model);
    }

    [HttpPost]
    public async Task<IActionResult> Novo(NovoAtendimentoViewModel model)
    {
        IgnorarValidacaoDoQueNaoSeraCadastrado(model);
        if (!ModelState.IsValid)
            return await FormularioAsync(model);

        var tutor = model.UsaTutorExistente ? new Tutor { Id = model.TutorExistenteId!.Value } : model.Tutor.ParaEntidade();
        var pet = model.UsaPetExistente ? new Pet { Id = model.PetExistenteId!.Value } : model.Pet.ParaEntidade();
        var consulta = new Consulta
        {
            VeterinarioId = model.VeterinarioId,
            DataHora = model.Data!.Value.ToDateTime(model.Hora!.Value),
            Motivo = model.Motivo
        };

        if (model.UsaPetExistente && !await PetPertenceAoTutorAsync(model.PetExistenteId!.Value, model.TutorExistenteId))
        {
            ModelState.AddModelError(string.Empty, "O pet escolhido não pertence a este tutor.");
            return await FormularioAsync(model);
        }

        var resultado = await atendimento.RegistrarAsync(tutor, pet, consulta);
        if (!resultado.Sucesso)
        {
            AdicionarErro(resultado, CamposDaTela);
            return await FormularioAsync(model);
        }

        MensagemSucesso($"Atendimento registrado: consulta agendada para {Formatos.DataHora(resultado.Valor!.DataHora)}.");
        return RedirectToAction("Detalhes", "Pets", new { id = resultado.Valor.PetId });
    }

    /// <summary>Quem escolhe um tutor ou pet já cadastrado não precisa preencher o formulário de cadastro dele.</summary>
    private void IgnorarValidacaoDoQueNaoSeraCadastrado(NovoAtendimentoViewModel model)
    {
        if (model.UsaTutorExistente)
            RemoverValidacao(nameof(NovoAtendimentoViewModel.Tutor));

        if (model.UsaPetExistente)
            RemoverValidacao(nameof(NovoAtendimentoViewModel.Pet));
        else
            ModelState.Remove($"{nameof(NovoAtendimentoViewModel.Pet)}.{nameof(PetFormViewModel.TutorId)}");
    }

    private void RemoverValidacao(string prefixo)
    {
        foreach (var chave in ModelState.Keys.Where(k => k.StartsWith(prefixo + ".")).ToList())
            ModelState.Remove(chave);
    }

    private async Task<bool> PetPertenceAoTutorAsync(int petId, int? tutorId) =>
        tutorId is { } id && await pets.ObterAsync(petId) is { } pet && pet.TutorId == id;

    private async Task<IActionResult> FormularioAsync(NovoAtendimentoViewModel model)
    {
        model.Veterinarios = await veterinarios.ListarVeterinariosAsync();

        if (model.UsaTutorExistente && await tutores.ObterAsync(model.TutorExistenteId!.Value) is { } tutor)
        {
            model.TutorExistenteDescricao = PetFormViewModel.Descrever(tutor);
            model.PetsDoTutor = await pets.ListarDoTutorAsync(tutor.Id);
        }

        return View(model);
    }
}
