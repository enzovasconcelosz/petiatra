namespace Petiatra.Web.Models;

public class Tutor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public DateTime CriadoEm { get; set; }
    public List<Pet> Pets { get; set; } = [];
}
