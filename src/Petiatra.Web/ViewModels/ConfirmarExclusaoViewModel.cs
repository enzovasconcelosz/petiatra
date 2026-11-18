namespace Petiatra.Web.ViewModels;

/// <summary>Tela única de confirmação usada antes de qualquer exclusão (RNF04).</summary>
public sealed record ConfirmarExclusaoViewModel(
    string Titulo,
    string Descricao,
    IReadOnlyList<string> Avisos,
    string UrlVoltar,
    bool Bloqueada = false);
