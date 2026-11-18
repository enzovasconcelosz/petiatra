namespace Petiatra.Web.Services;

/// <summary>Resultado de uma operação de negócio: sucesso ou a mensagem do problema.</summary>
public class Resultado
{
    protected Resultado(bool sucesso, string? erro, string? campo)
    {
        Sucesso = sucesso;
        Erro = erro;
        Campo = campo;
    }

    public bool Sucesso { get; }
    public string? Erro { get; }

    /// <summary>Nome do campo do formulário relacionado ao erro, quando houver.</summary>
    public string? Campo { get; }

    public static Resultado Ok() => new(true, null, null);
    public static Resultado Falha(string erro, string? campo = null) => new(false, erro, campo);
}

public sealed class Resultado<T> : Resultado
{
    private Resultado(bool sucesso, T? valor, string? erro, string? campo) : base(sucesso, erro, campo) => Valor = valor;

    public T? Valor { get; }

    public static Resultado<T> Ok(T valor) => new(true, valor, null, null);
    public static new Resultado<T> Falha(string erro, string? campo = null) => new(false, default, erro, campo);
    public static Resultado<T> De(Resultado outro) => new(false, default, outro.Erro, outro.Campo);
}
