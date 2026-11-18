using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Petiatra.Web.Infraestrutura;

/// <summary>
/// Aceita valores decimais digitados como "1.234,56" (pt-BR) ou "1234.56" (formato do navegador),
/// para que peso e valor sejam lidos corretamente nos dois casos.
/// </summary>
public class DecimalModelBinder : IModelBinder
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valor = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valor == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valor);
        var texto = valor.FirstValue?.Trim().Replace("R$", string.Empty).Trim();

        if (string.IsNullOrEmpty(texto))
        {
            if (bindingContext.ModelMetadata.IsReferenceOrNullableType)
                bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        if (TentarConverter(texto, out var numero))
            bindingContext.Result = ModelBindingResult.Success(numero);
        else
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                $"O campo {bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelName} precisa ser um número.");

        return Task.CompletedTask;
    }

    public static bool TentarConverter(string texto, out decimal numero)
    {
        var cultura = texto.Contains(',') ? PtBr : CultureInfo.InvariantCulture;
        return decimal.TryParse(texto, NumberStyles.Number, cultura, out numero);
    }
}

public class DecimalModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var tipo = Nullable.GetUnderlyingType(context.Metadata.ModelType) ?? context.Metadata.ModelType;
        return tipo == typeof(decimal) ? new DecimalModelBinder() : null;
    }
}
