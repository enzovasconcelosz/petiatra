using Petiatra.Web.Infraestrutura;
using Petiatra.Web.Models.Comum;

namespace Petiatra.Tests.Entidades;

public class DocumentoTests
{
    [Theory]
    [InlineData("529.982.247-25", true)]
    [InlineData("52998224725", true)]
    [InlineData("529.982.247-24", false)]
    [InlineData("111.111.111-11", false)]
    [InlineData("123", false)]
    [InlineData("", false)]
    public void CpfValido_confere_digitos_verificadores(string cpf, bool valido)
    {
        Assert.Equal(valido, Documento.CpfValido(cpf));
    }

    [Fact]
    public void FormatarCpf_coloca_pontos_e_traco()
    {
        Assert.Equal("529.982.247-25", Documento.FormatarCpf("52998224725"));
    }

    [Theory]
    [InlineData("1.234,56", 1234.56)]
    [InlineData("7,5", 7.5)]
    [InlineData("7.5", 7.5)]
    [InlineData("150", 150)]
    public void Valores_em_reais_aceitam_virgula_e_ponto(string texto, double esperado)
    {
        Assert.True(DecimalModelBinder.TentarConverter(texto, out var numero));
        Assert.Equal((decimal)esperado, numero);
    }
}
