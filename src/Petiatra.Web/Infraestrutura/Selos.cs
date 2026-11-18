using Petiatra.Web.Models.Enums;

namespace Petiatra.Web.Infraestrutura;

/// <summary>Classe CSS do selo colorido de cada situação.</summary>
public static class Selos
{
    public static string Classe(StatusConsulta status) => status switch
    {
        StatusConsulta.Agendada => "selo selo-info",
        StatusConsulta.Realizada => "selo selo-ok",
        _ => "selo selo-neutro"
    };

    public static string Classe(SituacaoVacina situacao) => situacao switch
    {
        SituacaoVacina.Atrasada => "selo selo-alerta",
        SituacaoVacina.EmDia => "selo selo-ok",
        SituacaoVacina.ReforcoAplicado => "selo selo-info",
        _ => "selo selo-neutro"
    };
}
