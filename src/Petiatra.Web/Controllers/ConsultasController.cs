using Microsoft.AspNetCore.Mvc;
using Petiatra.Web.Infraestrutura;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Models.Enums;
using Petiatra.Web.Services;
using Petiatra.Web.Services.Interfaces;
using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Controllers;

/// <summary>UC04: manter consultas.</summary>
public class ConsultasController(
    IConsultaService consultas,
    IPetService pets,
    IAuthService veterinarios,
    TimeProvider relogio) : AppController
{
    /// <summary>O erro de horário (RN01/RN02) aparece no campo "Hora" do formulário.</summary>
    private static readonly Dictionary<string, string> CamposDaTela = new() { [nameof(Consulta.DataHora)] = "Hora" };

    public async Task<IActionResult> Index(DateOnly? data, int? veterinarioId, StatusConsulta? status, string? busca, int pagina = 1)
    {
        // sem nenhum filtro na URL, abre a agenda de hoje
        if (Request.Query.Count == 0)
            return RedirectToAction(nameof(Index), new { data = relogio.Hoje().ToString("yyyy-MM-dd") });

        var filtro = new FiltroConsultas(data, veterinarioId, status, busca, pagina);
        return View(new ConsultasIndexViewModel(
            await consultas.ListarAsync(filtro),
            filtro,
            await veterinarios.ListarVeterinariosAsync(),
            relogio.Hoje()));
    }

    public async Task<IActionResult> Detalhes(int id)
    {
        var consulta = await consultas.ObterAsync(id);
        return consulta is null ? NotFound() : View(consulta);
    }

    [HttpGet]
    public async Task<IActionResult> Criar(int? petId, DateOnly? data)
    {
        var model = new ConsultaFormViewModel
        {
            VeterinarioId = User.ObterVeterinarioId(),
            Data = data ?? relogio.Hoje()
        };

        if (petId is { } id && await pets.ObterAsync(id) is { } pet)
        {
            model.PetId = pet.Id;
            model.PetDescricao = ConsultaFormViewModel.DescreverPet(pet);
        }

        return await FormularioAsync(model);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(ConsultaFormViewModel model, string? acao)
    {
        if (!ModelState.IsValid)
            return await FormularioAsync(model);

        var resultado = await consultas.AgendarAsync(model.ParaEntidade());
        if (!resultado.Sucesso)
        {
            AdicionarErro(resultado, CamposDaTela);
            return await FormularioAsync(model);
        }

        var consulta = resultado.Valor!;
        MensagemSucesso($"Consulta agendada para {Formatos.DataHora(consulta.DataHora)}.");
        return CadastrarOutro(acao)
            ? RedirectToAction(nameof(Criar), new { data = model.Data!.Value.ToString("yyyy-MM-dd") })
            : RedirectToAction(nameof(Index), new { data = model.Data!.Value.ToString("yyyy-MM-dd") });
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var consulta = await consultas.ObterAsync(id);
        if (consulta is null)
            return NotFound();

        if (!consulta.PodeSerReagendada)
        {
            MensagemErro("Somente consultas agendadas podem ser alteradas.");
            return RedirectToAction(nameof(Detalhes), new { id });
        }

        return await FormularioAsync(ConsultaFormViewModel.De(consulta));
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, ConsultaFormViewModel model)
    {
        model.Id = id;
        if (!ModelState.IsValid)
            return await FormularioAsync(model);

        var resultado = await consultas.ReagendarAsync(id, model.DataHora, model.VeterinarioId, model.PetId, model.Motivo);
        if (!resultado.Sucesso)
        {
            AdicionarErro(resultado, CamposDaTela);
            return await FormularioAsync(model);
        }

        MensagemSucesso($"Consulta alterada para {Formatos.DataHora(resultado.Valor!.DataHora)}.");
        return RedirectToAction(nameof(Detalhes), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Atender(int id)
    {
        var consulta = await consultas.ObterAsync(id);
        if (consulta is null)
            return NotFound();

        if (!consulta.PodeRegistrarAtendimento)
        {
            MensagemErro("Uma consulta cancelada não pode receber atendimento.");
            return RedirectToAction(nameof(Detalhes), new { id });
        }

        return View(AtendimentoFormViewModel.De(consulta));
    }

    [HttpPost]
    public async Task<IActionResult> Atender(int id, AtendimentoFormViewModel model)
    {
        if (ModelState.IsValid)
        {
            var resultado = await consultas.RegistrarAtendimentoAsync(id, model.Diagnostico, model.Tratamento, model.Valor);
            if (resultado.Sucesso)
            {
                MensagemSucesso("Atendimento registrado.");
                return RedirectToAction(nameof(Detalhes), new { id });
            }

            AdicionarErro(resultado);
        }

        model.Consulta = await consultas.ObterAsync(id);
        return model.Consulta is null ? NotFound() : View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Cancelar(int id)
    {
        var resultado = await consultas.CancelarAsync(id);
        return MensagemEVoltar(resultado, "Consulta cancelada. O horário ficou livre.", RedirectToAction(nameof(Detalhes), new { id }));
    }

    [HttpGet]
    public async Task<IActionResult> Excluir(int id)
    {
        var consulta = await consultas.ObterAsync(id);
        if (consulta is null)
            return NotFound();

        return View("ConfirmarExclusao", new ConfirmarExclusaoViewModel(
            "Excluir consulta",
            $"{consulta.Pet?.Nome} · {Formatos.DataHora(consulta.DataHora)} · {consulta.Motivo}",
            [
                "A consulta sai do histórico do pet. Para apenas liberar o horário, prefira cancelar.",
                "Esta ação não pode ser desfeita."
            ],
            Url.Action(nameof(Detalhes), new { id })!));
    }

    [HttpPost, ActionName(nameof(Excluir))]
    public async Task<IActionResult> ExcluirConfirmado(int id)
    {
        var resultado = await consultas.ExcluirAsync(id);
        return MensagemEVoltar(resultado, "Consulta excluída.", RedirectToAction(nameof(Index)));
    }

    /// <summary>RF08: horários já ocupados do veterinário no dia, exibidos enquanto o usuário preenche o formulário.</summary>
    [HttpGet]
    public async Task<IActionResult> Ocupados(int veterinarioId, DateOnly data, int? ignorar)
    {
        var doDia = await consultas.ListarOcupadosAsync(veterinarioId, data);
        var horarios = doDia
            .Where(c => c.Id != ignorar)
            .OrderBy(c => c.DataHora)
            .Select(c => Formatos.Hora(c.DataHora));
        return Json(horarios);
    }

    private async Task<IActionResult> FormularioAsync(ConsultaFormViewModel model)
    {
        model.Veterinarios = await veterinarios.ListarVeterinariosAsync();
        if (model.PetId > 0 && string.IsNullOrEmpty(model.PetDescricao) && await pets.ObterAsync(model.PetId) is { } pet)
            model.PetDescricao = ConsultaFormViewModel.DescreverPet(pet);

        return View("Form", model);
    }
}
