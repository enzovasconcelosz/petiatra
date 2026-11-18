namespace Petiatra.Web.Models.Comum;

/// <summary>Dados de paginação usados pela barra de páginas, independentemente do tipo de item.</summary>
public interface IPaginacao
{
    int Numero { get; }
    int TotalItens { get; }
    int TotalPaginas { get; }
    bool TemAnterior { get; }
    bool TemProxima { get; }
}

/// <summary>Uma página de resultados de uma listagem (RNF08: 20 itens por página).</summary>
public sealed record Pagina<T>(IReadOnlyList<T> Itens, int Numero, int TamanhoPagina, int TotalItens) : IPaginacao
{
    public const int TamanhoPadrao = 20;

    public int TotalPaginas => Math.Max(1, (int)Math.Ceiling(TotalItens / (double)TamanhoPagina));
    public bool TemAnterior => Numero > 1;
    public bool TemProxima => Numero < TotalPaginas;

    public static int Normalizar(int numero) => Math.Max(1, numero);
}
