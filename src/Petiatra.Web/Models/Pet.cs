using Petiatra.Web.Models.Enums;

namespace Petiatra.Web.Models;

public class Pet
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Especie Especie { get; set; }
    public string? Raca { get; set; }
    public Sexo Sexo { get; set; }
    public DateOnly? DataNascimento { get; set; }
    public decimal? PesoKg { get; set; }
    public string? Observacoes { get; set; }
    public int TutorId { get; set; }
    public Tutor? Tutor { get; set; }
    public DateTime CriadoEm { get; set; }
    public List<Consulta> Consultas { get; set; } = [];
    public List<Vacina> Vacinas { get; set; } = [];

    /// <summary>Idade em anos ou meses, por exemplo "3 anos" ou "5 meses".</summary>
    public string CalcularIdade(DateOnly hoje)
    {
        if (DataNascimento is not { } nascimento || nascimento > hoje)
            return "Não informada";

        var meses = (hoje.Year - nascimento.Year) * 12 + hoje.Month - nascimento.Month;
        if (hoje.Day < nascimento.Day)
            meses--;

        if (meses >= 12)
        {
            var anos = meses / 12;
            return anos == 1 ? "1 ano" : $"{anos} anos";
        }

        return meses switch
        {
            0 => "Menos de 1 mês",
            1 => "1 mês",
            _ => $"{meses} meses"
        };
    }
}
