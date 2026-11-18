using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Repositories.Interfaces;
using Petiatra.Web.Services.Interfaces;

namespace Petiatra.Web.Services;

public class TutorService(ITutorRepositorio tutores, TimeProvider relogio) : ITutorService
{
    public Task<Pagina<Tutor>> ListarAsync(FiltroTutores filtro) => tutores.BuscarPaginadoAsync(filtro);

    public Task<Tutor?> ObterAsync(int id) => tutores.ObterPorIdAsync(id);

    public Task<Tutor?> ObterComPetsAsync(int id) => tutores.ObterComPetsAsync(id);

    public async Task<Resultado<Tutor>> SalvarAsync(Tutor tutor)
    {
        Normalizar(tutor);

        if (!Documento.CpfValido(tutor.Cpf))
            return Resultado<Tutor>.Falha("Informe um CPF válido.", nameof(Tutor.Cpf));

        // RN03: CPF único
        if (await tutores.ExisteCpfAsync(tutor.Cpf, tutor.Id))
            return Resultado<Tutor>.Falha("CPF já cadastrado para outro tutor.", nameof(Tutor.Cpf));

        if (tutor.Id == 0)
        {
            tutor.CriadoEm = relogio.Agora();
            await tutores.AdicionarAsync(tutor);
        }
        else
        {
            await tutores.AtualizarAsync(tutor);
        }

        return Resultado<Tutor>.Ok(tutor);
    }

    public async Task<Resultado> ExcluirAsync(int id)
    {
        var tutor = await tutores.ObterPorIdAsync(id);
        if (tutor is null)
            return Resultado.Falha("Tutor não encontrado.");

        // RN04: tutor com pets não pode ser excluído
        if (await tutores.PossuiPetsAsync(id))
            return Resultado.Falha($"{tutor.Nome} possui pets cadastrados. Exclua ou transfira os pets antes de excluir o tutor.");

        await tutores.RemoverAsync(tutor);
        return Resultado.Ok();
    }

    private static void Normalizar(Tutor tutor)
    {
        tutor.Nome = tutor.Nome.Trim();
        tutor.Cpf = Documento.SomenteDigitos(tutor.Cpf);
        tutor.Telefone = tutor.Telefone.Trim();
        tutor.Email = Documento.Limpar(tutor.Email)?.ToLowerInvariant();
        tutor.Endereco = Documento.Limpar(tutor.Endereco);
    }
}
