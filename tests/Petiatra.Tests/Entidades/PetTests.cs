using Petiatra.Web.Models;

namespace Petiatra.Tests.Entidades;

public class PetTests
{
    private static readonly DateOnly Hoje = new(2026, 11, 18);

    [Theory]
    [InlineData(2023, 9, 18, "3 anos")]
    [InlineData(2025, 11, 18, "1 ano")]
    [InlineData(2025, 11, 19, "11 meses")]
    [InlineData(2026, 6, 10, "5 meses")]
    [InlineData(2026, 10, 18, "1 mês")]
    [InlineData(2026, 11, 1, "Menos de 1 mês")]
    public void CalcularIdade_devolve_anos_ou_meses(int ano, int mes, int dia, string esperado)
    {
        var pet = new Pet { DataNascimento = new DateOnly(ano, mes, dia) };

        Assert.Equal(esperado, pet.CalcularIdade(Hoje));
    }

    [Fact]
    public void CalcularIdade_sem_data_de_nascimento()
    {
        Assert.Equal("Não informada", new Pet().CalcularIdade(Hoje));
    }
}
