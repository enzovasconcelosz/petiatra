namespace Petiatra.Tests.Apoio;

/// <summary>Relógio com data e hora fixas, para os testes não dependerem do dia em que rodam.</summary>
public sealed class RelogioFixo(DateTime agora) : TimeProvider
{
    public static readonly DateTime Padrao = new(2026, 11, 18, 9, 0, 0);

    public RelogioFixo() : this(Padrao) { }

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;

    public override DateTimeOffset GetUtcNow() => new(DateTime.SpecifyKind(agora, DateTimeKind.Utc));

    public DateOnly Hoje => DateOnly.FromDateTime(agora);
}
