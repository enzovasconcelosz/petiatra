using Petiatra.Web.Models;
using Petiatra.Web.Models.Enums;

namespace Petiatra.Tests.Entidades;

public class VacinaTests
{
    private static readonly DateOnly Hoje = new(2026, 11, 18);

    [Fact]
    public void Sem_data_de_reforco_e_dose_unica()
    {
        var vacina = new Vacina { DataAplicacao = Hoje.AddMonths(-1) };

        Assert.Equal(SituacaoVacina.DoseUnica, vacina.ObterSituacao(Hoje, reforcoAplicado: false));
    }

    [Fact]
    public void Reforco_futuro_esta_em_dia()
    {
        var vacina = new Vacina { DataAplicacao = Hoje.AddMonths(-1), DataReforco = Hoje.AddDays(10) };

        Assert.Equal(SituacaoVacina.EmDia, vacina.ObterSituacao(Hoje, reforcoAplicado: false));
    }

    [Fact]
    public void Reforco_vencido_sem_dose_posterior_esta_atrasada_RN07()
    {
        var vacina = new Vacina { DataAplicacao = Hoje.AddYears(-1), DataReforco = Hoje.AddDays(-1) };

        Assert.Equal(SituacaoVacina.Atrasada, vacina.ObterSituacao(Hoje, reforcoAplicado: false));
    }

    [Fact]
    public void Reforco_vencido_com_dose_posterior_nao_esta_atrasada()
    {
        var vacina = new Vacina { DataAplicacao = Hoje.AddYears(-1), DataReforco = Hoje.AddDays(-1) };

        Assert.Equal(SituacaoVacina.ReforcoAplicado, vacina.ObterSituacao(Hoje, reforcoAplicado: true));
    }

    [Fact]
    public void TemDosePosterior_considera_mesmo_pet_e_mesmo_nome()
    {
        var primeira = new Vacina { Id = 1, PetId = 1, Nome = "Antirrábica", DataAplicacao = Hoje.AddYears(-1) };
        var outras = new[]
        {
            primeira,
            new Vacina { Id = 2, PetId = 2, Nome = "Antirrábica", DataAplicacao = Hoje },
            new Vacina { Id = 3, PetId = 1, Nome = "V10", DataAplicacao = Hoje }
        };

        Assert.False(primeira.TemDosePosterior(outras));
        Assert.True(primeira.TemDosePosterior([.. outras, new Vacina { Id = 4, PetId = 1, Nome = "antirrábica", DataAplicacao = Hoje }]));
    }
}
