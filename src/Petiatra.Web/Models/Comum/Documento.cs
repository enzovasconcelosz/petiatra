namespace Petiatra.Web.Models.Comum;

/// <summary>Funções de apoio para CPF e textos digitados pelo usuário.</summary>
public static class Documento
{
    public static string SomenteDigitos(string? texto) =>
        new((texto ?? string.Empty).Where(char.IsDigit).ToArray());

    public static string FormatarCpf(string cpf)
    {
        var digitos = SomenteDigitos(cpf);
        return digitos.Length == 11
            ? $"{digitos[..3]}.{digitos[3..6]}.{digitos[6..9]}-{digitos[9..]}"
            : cpf;
    }

    /// <summary>Valida os dígitos verificadores do CPF.</summary>
    public static bool CpfValido(string? cpf)
    {
        var digitos = SomenteDigitos(cpf);
        if (digitos.Length != 11 || digitos.Distinct().Count() == 1)
            return false;

        return DigitoVerificador(digitos, 9) == digitos[9] - '0'
               && DigitoVerificador(digitos, 10) == digitos[10] - '0';
    }

    private static int DigitoVerificador(string digitos, int quantidade)
    {
        var soma = 0;
        for (var i = 0; i < quantidade; i++)
            soma += (digitos[i] - '0') * (quantidade + 1 - i);

        var resto = soma * 10 % 11;
        return resto == 10 ? 0 : resto;
    }

    public static string? Limpar(string? texto) =>
        string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
}
