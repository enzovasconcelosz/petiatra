using System.ComponentModel.DataAnnotations;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Models.Enums;

namespace Petiatra.Web.ViewModels;

public class ConsultaFormViewModel
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o pet.")]
    [Display(Name = "Pet")]
    public int PetId { get; set; }

    public string? PetDescricao { get; set; }

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

    public DateTime DataHora => Data!.Value.ToDateTime(Hora!.Value);

    public static ConsultaFormViewModel De(Consulta consulta) => new()
    {
        Id = consulta.Id,
        PetId = consulta.PetId,
        PetDescricao = consulta.Pet is null ? null : DescreverPet(consulta.Pet),
        VeterinarioId = consulta.VeterinarioId,
        Data = DateOnly.FromDateTime(consulta.DataHora),
        Hora = TimeOnly.FromDateTime(consulta.DataHora),
        Motivo = consulta.Motivo
    };

    public Consulta ParaEntidade() => new()
    {
        PetId = PetId,
        VeterinarioId = VeterinarioId,
        DataHora = DataHora,
        Motivo = Motivo
    };

    public static string DescreverPet(Pet pet) =>
        pet.Tutor is null ? pet.Nome : $"{pet.Nome} ({pet.Especie.Rotulo()}) · tutor {pet.Tutor.Nome}";
}

public class AtendimentoFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Descreva o diagnóstico.")]
    [StringLength(2000, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "Diagnóstico")]
    public string Diagnostico { get; set; } = string.Empty;

    [Required(ErrorMessage = "Descreva o tratamento indicado.")]
    [StringLength(2000, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "Tratamento")]
    public string Tratamento { get; set; } = string.Empty;

    [Range(0, 99999999.99, ErrorMessage = "Informe um valor válido, sem números negativos.")]
    [Display(Name = "Valor (R$)")]
    public decimal? Valor { get; set; }

    public Consulta? Consulta { get; set; }

    public static AtendimentoFormViewModel De(Consulta consulta) => new()
    {
        Id = consulta.Id,
        Diagnostico = consulta.Diagnostico ?? string.Empty,
        Tratamento = consulta.Tratamento ?? string.Empty,
        Valor = consulta.Valor,
        Consulta = consulta
    };
}

public sealed record ConsultasIndexViewModel(
    Pagina<Consulta> Pagina,
    FiltroConsultas Filtro,
    IReadOnlyList<Veterinario> Veterinarios,
    DateOnly Hoje);
