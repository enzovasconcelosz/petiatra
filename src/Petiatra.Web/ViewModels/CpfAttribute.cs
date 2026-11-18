using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Petiatra.Web.Models.Comum;

namespace Petiatra.Web.ViewModels;

/// <summary>Valida os dígitos do CPF no servidor e, com o adaptador em site.js, também no navegador.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CpfAttribute : ValidationAttribute, IClientModelValidator
{
    public CpfAttribute() : base("Informe um CPF válido.") { }

    public override bool IsValid(object? value) =>
        value is not string texto || string.IsNullOrWhiteSpace(texto) || Documento.CpfValido(texto);

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-cpf", ErrorMessageString);
    }
}
