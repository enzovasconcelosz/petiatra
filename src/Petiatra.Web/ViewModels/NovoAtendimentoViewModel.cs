using System.ComponentModel.DataAnnotations;
using Petiatra.Web.Models;

namespace Petiatra.Web.ViewModels;

/// <summary>RF13: dados dos 3 passos do assistente "Novo atendimento".</summary>
public class NovoAtendimentoViewModel
{
    /// <summary>Preenchido quando o tutor já é cadastrado; senão, os dados de <see cref="Tutor"/> são usados.</summary>
    public int? TutorExistenteId { get; set; }
    public string? TutorExistenteDescricao { get; set; }
    public TutorFormViewModel Tutor { get; set; } = new();

    /// <summary>Preenchido quando o pet já é cadastrado; senão, os dados de <see cref="Pet"/> são usados.</summary>
    public int? PetExistenteId { get; set; }
    public PetFormViewModel Pet { get; set; } = new();

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o veterinário.")]
    [Display(Name = "Veterinário")]
    public int VeterinarioId { get; set; }

    [Required(ErrorMessage = "Informe a data.")]
    [Display(Name = "Data")]
    public DateOnly? Data { get; set; }

    [Required(ErrorMessage = "Informe a hora.")]
    [Display(Name = "Hora")]
    public TimeOnly? Hora { get; set; }

    [Required(ErrorMessage = "Informe o motivo da consulta.")]
    [StringLength(200, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "Motivo")]
    public string Motivo { get; set; } = string.Empty;

    public IReadOnlyList<Veterinario> Veterinarios { get; set; } = [];
    public IReadOnlyList<Pet> PetsDoTutor { get; set; } = [];

    public bool UsaTutorExistente => TutorExistenteId is > 0;
    public bool UsaPetExistente => PetExistenteId is > 0;
}
