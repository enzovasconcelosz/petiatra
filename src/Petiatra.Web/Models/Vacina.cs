using Petiatra.Web.Models.Enums;

namespace Petiatra.Web.Models;

public class Vacina
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateOnly DataAplicacao { get; set; }
    public DateOnly? DataReforco { get; set; }
    public string? Lote { get; set; }
    public int PetId { get; set; }
    public Pet? Pet { get; set; }
    public int VeterinarioId { get; set; }
    public Veterinario? Veterinario { get; set; }
    public DateTime CriadoEm { get; set; }

    /// <summary>
    /// RN07: a dose está atrasada quando a data de reforço já passou
    /// e não existe dose posterior da mesma vacina para o mesmo pet.
    /// </summary>
    public SituacaoVacina ObterSituacao(DateOnly hoje, bool reforcoAplicado)
    {
        if (reforcoAplicado)
            return SituacaoVacina.ReforcoAplicado;

        if (DataReforco is not { } reforco)
            return SituacaoVacina.DoseUnica;

        return reforco < hoje ? SituacaoVacina.Atrasada : SituacaoVacina.EmDia;
    }

    /// <summary>Verdadeiro quando existe, em <paramref name="outras"/>, uma dose posterior da mesma vacina no mesmo pet.</summary>
    public bool TemDosePosterior(IEnumerable<Vacina> outras) =>
        outras.Any(outra => outra.Id != Id
                            && outra.PetId == PetId
                            && string.Equals(outra.Nome, Nome, StringComparison.OrdinalIgnoreCase)
                            && outra.DataAplicacao > DataAplicacao);
}
