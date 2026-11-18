using Petiatra.Web.Models.Enums;

namespace Petiatra.Web.Models;

public class Consulta
{
    public int Id { get; set; }
    public DateTime DataHora { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Diagnostico { get; private set; }
    public string? Tratamento { get; private set; }
    public decimal? Valor { get; private set; }
    public StatusConsulta Status { get; private set; } = StatusConsulta.Agendada;
    public int PetId { get; set; }
    public Pet? Pet { get; set; }
    public int VeterinarioId { get; set; }
    public Veterinario? Veterinario { get; set; }
    public DateTime CriadoEm { get; set; }

    public bool PodeSerCancelada => Status == StatusConsulta.Agendada;
    public bool PodeSerReagendada => Status == StatusConsulta.Agendada;
    public bool PodeRegistrarAtendimento => Status != StatusConsulta.Cancelada;

    /// <summary>
    /// RN01: indica se as duas consultas são do mesmo veterinário, não estão canceladas
    /// e ficam a menos de <paramref name="intervalo"/> uma da outra.
    /// </summary>
    public bool ConflitaCom(Consulta outra, TimeSpan intervalo)
    {
        if (ReferenceEquals(this, outra) || (Id != 0 && Id == outra.Id))
            return false;

        if (VeterinarioId != outra.VeterinarioId)
            return false;

        if (Status == StatusConsulta.Cancelada || outra.Status == StatusConsulta.Cancelada)
            return false;

        return (DataHora - outra.DataHora).Duration() < intervalo;
    }

    public void RegistrarAtendimento(string diagnostico, string tratamento, decimal? valor)
    {
        if (!PodeRegistrarAtendimento)
            throw new InvalidOperationException("Uma consulta cancelada não pode receber atendimento.");

        if (valor < 0)
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor não pode ser negativo.");

        Diagnostico = diagnostico.Trim();
        Tratamento = tratamento.Trim();
        Valor = valor;
        Status = StatusConsulta.Realizada;
    }

    public void Cancelar()
    {
        if (!PodeSerCancelada)
            throw new InvalidOperationException("Somente consultas agendadas podem ser canceladas.");

        Status = StatusConsulta.Cancelada;
    }
}
