using System.ComponentModel.DataAnnotations;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;

namespace Petiatra.Web.ViewModels;

public class VacinaFormViewModel
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o pet.")]
    [Display(Name = "Pet")]
    public int PetId { get; set; }

    public string? PetDescricao { get; set; }

    [Required(ErrorMessage = "Informe o nome da vacina.")]
    [StringLength(80, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "Vacina")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a data de aplicação.")]
    [Display(Name = "Data de aplicação")]
    public DateOnly? DataAplicacao { get; set; }

    [Display(Name = "Data do reforço")]
    public DateOnly? DataReforco { get; set; }

    [StringLength(40, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "Lote")]
    public string? Lote { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o veterinário.")]
    [Display(Name = "Aplicada por")]
    public int VeterinarioId { get; set; }

    public IReadOnlyList<Veterinario> Veterinarios { get; set; } = [];
    public IReadOnlyList<string> NomesSugeridos { get; set; } = [];

    public static VacinaFormViewModel De(Vacina vacina) => new()
    {
        Id = vacina.Id,
        PetId = vacina.PetId,
        PetDescricao = vacina.Pet is null ? null : ConsultaFormViewModel.DescreverPet(vacina.Pet),
        Nome = vacina.Nome,
        DataAplicacao = vacina.DataAplicacao,
        DataReforco = vacina.DataReforco,
        Lote = vacina.Lote,
        VeterinarioId = vacina.VeterinarioId
    };

    public Vacina ParaEntidade()
    {
        var vacina = new Vacina { Id = Id };
        CopiarPara(vacina);
        return vacina;
    }

    public void CopiarPara(Vacina vacina)
    {
        vacina.PetId = PetId;
        vacina.Nome = Nome;
        vacina.DataAplicacao = DataAplicacao!.Value;
        vacina.DataReforco = DataReforco;
        vacina.Lote = Lote;
        vacina.VeterinarioId = VeterinarioId;
    }
}

public sealed record VacinasIndexViewModel(Pagina<ItemVacina> Pagina, FiltroVacinas Filtro, DateOnly Hoje);
