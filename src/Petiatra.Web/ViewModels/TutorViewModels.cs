using System.ComponentModel.DataAnnotations;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;

namespace Petiatra.Web.ViewModels;

public class TutorFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do tutor.")]
    [StringLength(100, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "Nome completo")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o CPF.")]
    [Cpf]
    [Display(Name = "CPF")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe um telefone para contato.")]
    [StringLength(20, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "Telefone / WhatsApp")]
    public string Telefone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(120, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [StringLength(200, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "Endereço")]
    public string? Endereco { get; set; }

    public bool TemDetalhes => !string.IsNullOrWhiteSpace(Email) || !string.IsNullOrWhiteSpace(Endereco);

    public static TutorFormViewModel De(Tutor tutor) => new()
    {
        Id = tutor.Id,
        Nome = tutor.Nome,
        Cpf = Documento.FormatarCpf(tutor.Cpf),
        Telefone = tutor.Telefone,
        Email = tutor.Email,
        Endereco = tutor.Endereco
    };

    public Tutor ParaEntidade() => new()
    {
        Id = Id,
        Nome = Nome,
        Cpf = Cpf,
        Telefone = Telefone,
        Email = Email,
        Endereco = Endereco
    };

    public void CopiarPara(Tutor tutor)
    {
        tutor.Nome = Nome;
        tutor.Cpf = Cpf;
        tutor.Telefone = Telefone;
        tutor.Email = Email;
        tutor.Endereco = Endereco;
    }
}

public sealed record TutoresIndexViewModel(Pagina<Tutor> Pagina, string? Busca);
