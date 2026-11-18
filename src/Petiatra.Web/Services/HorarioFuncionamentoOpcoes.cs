namespace Petiatra.Web.Services;

/// <summary>Configuração do expediente da clínica (seção "Clinica" do appsettings.json).</summary>
public class HorarioFuncionamentoOpcoes
{
    public const string Secao = "Clinica";

    public TimeOnly Abertura { get; set; } = new(8, 0);
    public TimeOnly Fechamento { get; set; } = new(18, 0);
    public int IntervaloEntreConsultasMinutos { get; set; } = 30;

    public TimeSpan IntervaloEntreConsultas => TimeSpan.FromMinutes(IntervaloEntreConsultasMinutos);
}
