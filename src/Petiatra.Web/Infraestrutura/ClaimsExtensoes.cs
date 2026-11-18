using System.Security.Claims;

namespace Petiatra.Web.Infraestrutura;

public static class ClaimsExtensoes
{
    public static int ObterVeterinarioId(this ClaimsPrincipal usuario) =>
        int.TryParse(usuario.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    public static string ObterNome(this ClaimsPrincipal usuario) =>
        usuario.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public static string ObterPrimeiroNome(this ClaimsPrincipal usuario)
    {
        var nome = usuario.ObterNome().Replace("Dra. ", "").Replace("Dr. ", "");
        return nome.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
    }
}
