using System.ComponentModel.DataAnnotations;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Models.Enums;

namespace Petiatra.Web.ViewModels;

public class PetFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do pet.")]
    [StringLength(60, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "Nome do pet")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escolha a espécie.")]
    [Display(Name = "Espécie")]
    public Especie? Especie { get; set; }

    [Display(Name = "Sexo")]
    public Sexo Sexo { get; set; } = Sexo.NaoInformado;

    [StringLength(60, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "Raça")]
    public string? Raca { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Data de nascimento")]
    public DateOnly? DataNascimento { get; set; }

    [Range(0.01, 9999.99, ErrorMessage = "Informe um peso entre 0,01 e 9.999,99 kg.")]
    [Display(Name = "Peso (kg)")]
    public decimal? PesoKg { get; set; }

    [StringLength(500, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "Observações")]
    public string? Observacoes { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o tutor.")]
    [Display(Name = "Tutor")]
    public int TutorId { get; set; }

    /// <summary>Texto exibido no seletor de tutor (nome e CPF).</summary>
    public string? TutorDescricao { get; set; }

    public bool TemDetalhes => !string.IsNullOrWhiteSpace(Raca) || DataNascimento is not null
                               || PesoKg is not null || !string.IsNullOrWhiteSpace(Observacoes);

    public static PetFormViewModel De(Pet pet) => new()
    {
        Id = pet.Id,
        Nome = pet.Nome,
        Especie = pet.Especie,
        Sexo = pet.Sexo,
        Raca = pet.Raca,
        DataNascimento = pet.DataNascimento,
        PesoKg = pet.PesoKg,
        Observacoes = pet.Observacoes,
        TutorId = pet.TutorId,
        TutorDescricao = pet.Tutor is null ? null : Descrever(pet.Tutor)
    };

    public Pet ParaEntidade()
    {
        var pet = new Pet { Id = Id };
        CopiarPara(pet);
        return pet;
    }

    public void CopiarPara(Pet pet)
    {
        pet.Nome = Nome;
        pet.Especie = Especie ?? Models.Enums.Especie.Outro;
        pet.Sexo = Sexo;
        pet.Raca = Raca;
        pet.DataNascimento = DataNascimento;
        pet.PesoKg = PesoKg;
        pet.Observacoes = Observacoes;
        pet.TutorId = TutorId;
    }

    public static string Descrever(Tutor tutor) => $"{tutor.Nome} · CPF {Documento.FormatarCpf(tutor.Cpf)}";
}

public sealed record PetsIndexViewModel(Pagina<Pet> Pagina, string? Busca, Especie? Especie);

public sealed record VacinaNaFicha(Vacina Vacina, SituacaoVacina Situacao);

public sealed record FichaPetViewModel(
    Pet Pet,
    string Idade,
    IReadOnlyList<Consulta> Consultas,
    IReadOnlyList<VacinaNaFicha> Vacinas);
