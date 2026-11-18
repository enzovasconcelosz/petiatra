namespace Petiatra.Web.Models;

public class Veterinario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Crmv { get; set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public DateTime CriadoEm { get; set; }

    /// <summary>Guarda o hash gerado pelo PasswordHasher. A senha em texto nunca é armazenada.</summary>
    public void DefinirSenha(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("O hash da senha é obrigatório.", nameof(hash));

        SenhaHash = hash;
    }
}
