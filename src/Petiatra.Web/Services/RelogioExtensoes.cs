namespace Petiatra.Web.Services;

public static class RelogioExtensoes
{
    public static DateTime Agora(this TimeProvider relogio) => relogio.GetLocalNow().DateTime;

    public static DateOnly Hoje(this TimeProvider relogio) => DateOnly.FromDateTime(relogio.Agora());
}
