namespace Petiatra.Web.Models.Enums;

/// <summary>Textos em português para exibir as enumerações nas telas.</summary>
public static class RotulosEnum
{
    public static string Rotulo(this Especie especie) => especie switch
    {
        Especie.Reptil => "Réptil",
        _ => especie.ToString()
    };

    public static string Rotulo(this Sexo sexo) => sexo switch
    {
        Sexo.Macho => "Macho",
        Sexo.Femea => "Fêmea",
        _ => "Não informado"
    };

    public static string Rotulo(this StatusConsulta status) => status.ToString();

    public static string Rotulo(this SituacaoVacina situacao) => situacao switch
    {
        SituacaoVacina.DoseUnica => "Dose única",
        SituacaoVacina.EmDia => "Em dia",
        SituacaoVacina.Atrasada => "Atrasada",
        SituacaoVacina.ReforcoAplicado => "Reforço aplicado",
        _ => situacao.ToString()
    };
}
