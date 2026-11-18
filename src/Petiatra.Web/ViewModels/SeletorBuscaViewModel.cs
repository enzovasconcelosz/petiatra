namespace Petiatra.Web.ViewModels;

/// <summary>Configuração do campo de seleção com busca (partial _SeletorBusca).</summary>
public sealed record SeletorBuscaViewModel(
    string Campo,
    string Rotulo,
    string Url,
    int Valor,
    string? Texto,
    string Dica,
    string Ajuda,
    string MensagemObrigatorio,
    bool Obrigatorio = true);
