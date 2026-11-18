using System.Globalization;

namespace Petiatra.Web.Infraestrutura;

/// <summary>RNF11: datas em dd/mm/aaaa e valores em R$.</summary>
public static class Formatos
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public static string Data(DateOnly data) => data.ToString("dd/MM/yyyy", PtBr);
    public static string Data(DateOnly? data) => data is { } d ? Data(d) : "—";
    public static string Data(DateTime data) => data.ToString("dd/MM/yyyy", PtBr);
    public static string DataHora(DateTime data) => data.ToString("dd/MM/yyyy HH:mm", PtBr);
    public static string Hora(DateTime data) => data.ToString("HH:mm", PtBr);
    public static string Moeda(decimal? valor) => valor is { } v ? v.ToString("C", PtBr) : "—";
    public static string Numero(decimal? valor) => valor is { } v ? v.ToString("0.##", PtBr) : "—";
    public static string DiaPorExtenso(DateOnly data) => data.ToString("dddd, d 'de' MMMM", PtBr);
}
