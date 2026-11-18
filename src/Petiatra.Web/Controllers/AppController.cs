using Microsoft.AspNetCore.Mvc;
using Petiatra.Web.Services;

namespace Petiatra.Web.Controllers;

/// <summary>Comportamento comum dos controllers: mensagens para o usuário e erros do serviço.</summary>
public abstract class AppController : Controller
{
    public const string ChaveSucesso = "Sucesso";
    public const string ChaveErro = "Erro";
    protected const string AcaoSalvarECadastrarOutro = "outro";

    protected void MensagemSucesso(string mensagem) => TempData[ChaveSucesso] = mensagem;

    protected void MensagemErro(string mensagem) => TempData[ChaveErro] = mensagem;

    /// <summary>Mostra o erro do serviço no campo correspondente do formulário (ou no topo, se não houver campo).</summary>
    protected void AdicionarErro(Resultado resultado, IReadOnlyDictionary<string, string>? camposDaTela = null)
    {
        var campo = resultado.Campo ?? string.Empty;
        if (camposDaTela is not null && camposDaTela.TryGetValue(campo, out var campoDaTela))
            campo = campoDaTela;

        ModelState.AddModelError(campo, resultado.Erro ?? "Não foi possível concluir a operação.");
    }

    protected IActionResult MensagemEVoltar(Resultado resultado, string sucesso, IActionResult destino)
    {
        if (resultado.Sucesso)
            MensagemSucesso(sucesso);
        else
            MensagemErro(resultado.Erro!);

        return destino;
    }

    protected static bool CadastrarOutro(string? acao) => acao == AcaoSalvarECadastrarOutro;
}
