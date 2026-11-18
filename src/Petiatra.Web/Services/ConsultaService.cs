using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Repositories.Interfaces;
using Petiatra.Web.Services.Interfaces;
using Petiatra.Web.Services.Regras;

namespace Petiatra.Web.Services;

public class ConsultaService(
    IConsultaRepositorio consultas,
    IPetRepositorio pets,
    IVeterinarioRepositorio veterinarios,
    IEnumerable<IRegraAgendamento> regras,
    TimeProvider relogio) : IConsultaService
{
    public Task<Pagina<Consulta>> ListarAsync(FiltroConsultas filtro) => consultas.BuscarPaginadoAsync(filtro);

    public Task<Consulta?> ObterAsync(int id) => consultas.ObterCompletaAsync(id);

    public Task<IReadOnlyList<Consulta>> ListarOcupadosAsync(int veterinarioId, DateOnly dia) =>
        consultas.ListarDoVeterinarioNoDiaAsync(veterinarioId, dia);

    public async Task<Resultado<Consulta>> AgendarAsync(Consulta consulta)
    {
        consulta.Motivo = consulta.Motivo.Trim();
        consulta.DataHora = SemSegundos(consulta.DataHora);

        var validacao = await ValidarAsync(consulta);
        if (!validacao.Sucesso)
            return Resultado<Consulta>.De(validacao);

        consulta.CriadoEm = relogio.Agora();
        await consultas.AdicionarAsync(consulta);
        return Resultado<Consulta>.Ok(consulta);
    }

    public async Task<Resultado<Consulta>> ReagendarAsync(int id, DateTime dataHora, int veterinarioId, int petId, string motivo)
    {
        var consulta = await consultas.ObterPorIdAsync(id);
        if (consulta is null)
            return Resultado<Consulta>.Falha("Consulta não encontrada.");

        if (!consulta.PodeSerReagendada)
            return Resultado<Consulta>.Falha("Somente consultas agendadas podem ser alteradas.");

        consulta.DataHora = SemSegundos(dataHora);
        consulta.VeterinarioId = veterinarioId;
        consulta.PetId = petId;
        consulta.Motivo = motivo.Trim();

        var validacao = await ValidarAsync(consulta);
        if (!validacao.Sucesso)
            return Resultado<Consulta>.De(validacao);

        await consultas.AtualizarAsync(consulta);
        return Resultado<Consulta>.Ok(consulta);
    }

    public async Task<Resultado> RegistrarAtendimentoAsync(int id, string diagnostico, string tratamento, decimal? valor)
    {
        var consulta = await consultas.ObterPorIdAsync(id);
        if (consulta is null)
            return Resultado.Falha("Consulta não encontrada.");

        if (!consulta.PodeRegistrarAtendimento)
            return Resultado.Falha("Uma consulta cancelada não pode receber atendimento.");

        if (valor < 0)
            return Resultado.Falha("O valor não pode ser negativo.", nameof(Consulta.Valor));

        consulta.RegistrarAtendimento(diagnostico, tratamento, valor);
        await consultas.AtualizarAsync(consulta);
        return Resultado.Ok();
    }

    public async Task<Resultado> CancelarAsync(int id)
    {
        var consulta = await consultas.ObterPorIdAsync(id);
        if (consulta is null)
            return Resultado.Falha("Consulta não encontrada.");

        if (!consulta.PodeSerCancelada)
            return Resultado.Falha("Somente consultas agendadas podem ser canceladas.");

        consulta.Cancelar();
        await consultas.AtualizarAsync(consulta);
        return Resultado.Ok();
    }

    public async Task<Resultado> ExcluirAsync(int id)
    {
        var consulta = await consultas.ObterPorIdAsync(id);
        if (consulta is null)
            return Resultado.Falha("Consulta não encontrada.");

        await consultas.RemoverAsync(consulta);
        return Resultado.Ok();
    }

    /// <summary>RF08: confere pet e veterinário e depois cada regra de agendamento registrada (RN01, RN02...).</summary>
    private async Task<Resultado> ValidarAsync(Consulta consulta)
    {
        if (await pets.ObterPorIdAsync(consulta.PetId) is null)
            return Resultado.Falha("Selecione um pet cadastrado.", nameof(Consulta.PetId));

        if (await veterinarios.ObterPorIdAsync(consulta.VeterinarioId) is null)
            return Resultado.Falha("Selecione um veterinário cadastrado.", nameof(Consulta.VeterinarioId));

        foreach (var regra in regras)
        {
            var resultado = await regra.ValidarAsync(consulta);
            if (!resultado.Sucesso)
                return resultado;
        }

        return Resultado.Ok();
    }

    private static DateTime SemSegundos(DateTime data) =>
        new(data.Year, data.Month, data.Day, data.Hour, data.Minute, 0, data.Kind);
}
