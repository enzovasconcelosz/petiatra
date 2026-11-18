using System.ComponentModel.DataAnnotations;

namespace Petiatra.Web.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;

    [Display(Name = "Manter conectado")]
    public bool LembrarDeMim { get; set; }

    public string? ReturnUrl { get; set; }
}

public class CadastroVeterinarioViewModel
{
    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(100, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "Nome completo")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(120, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o CRMV.")]
    [StringLength(20, ErrorMessage = "Use até {1} caracteres.")]
    [Display(Name = "CRMV")]
    public string Crmv { get; set; } = string.Empty;

    [Required(ErrorMessage = "Crie uma senha.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha precisa ter pelo menos {2} caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Repita a senha.")]
    [Compare(nameof(Senha), ErrorMessage = "As senhas não conferem.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirme a senha")]
    public string ConfirmarSenha { get; set; } = string.Empty;
}
