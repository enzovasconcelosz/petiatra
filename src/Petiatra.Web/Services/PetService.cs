using Petiatra.Web.Models;
using Petiatra.Web.Models.Comum;
using Petiatra.Web.Repositories.Interfaces;
using Petiatra.Web.Services.Interfaces;
using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Services;

public class PetService(IPetRepositorio pets, ITutorRepositorio tutores, TimeProvider relogio) : IPetService
{
    public Task<Pagina<Pet>> ListarAsync(FiltroPets filtro) => pets.BuscarPaginadoAsync(filtro);

    public Task<IReadOnlyList<Pet>> ListarDoTutorAsync(int tutorId) => pets.ListarDoTutorAsync(tutorId);

    public Task<Pet?> ObterAsync(int id) => pets.ObterComTutorAsync(id);

    public async Task<FichaPetViewModel?> ObterFichaAsync(int id)
    {
        var pet = await pets.ObterComHistoricoAsync(id);
        if (pet is null)
            return null;

        var hoje = relogio.Hoje();
        var vacinas = pet.Vacinas
            .Select(v => new VacinaNaFicha(v, v.ObterSituacao(hoje, v.TemDosePosterior(pet.Vacinas))))
            .ToList();

        return new FichaPetViewModel(pet, pet.CalcularIdade(hoje), pet.Consultas, vacinas);
    }

    public async Task<Resultado<Pet>> SalvarAsync(Pet pet)
    {
        pet.Nome = pet.Nome.Trim();
        pet.Raca = Documento.Limpar(pet.Raca);
        pet.Observacoes = Documento.Limpar(pet.Observacoes);

        if (await tutores.ObterPorIdAsync(pet.TutorId) is null)
            return Resultado<Pet>.Falha("Selecione um tutor cadastrado.", nameof(Pet.TutorId));

        if (pet.DataNascimento > relogio.Hoje())
            return Resultado<Pet>.Falha("A data de nascimento não pode ser futura.", nameof(Pet.DataNascimento));

        if (pet.PesoKg is <= 0)
            return Resultado<Pet>.Falha("O peso deve ser maior que zero.", nameof(Pet.PesoKg));

        if (pet.Id == 0)
        {
            pet.CriadoEm = relogio.Agora();
            await pets.AdicionarAsync(pet);
        }
        else
        {
            await pets.AtualizarAsync(pet);
        }

        return Resultado<Pet>.Ok(pet);
    }

    public async Task<Resultado> ExcluirAsync(int id)
    {
        var pet = await pets.ObterPorIdAsync(id);
        if (pet is null)
            return Resultado.Falha("Pet não encontrado.");

        // RN05: consultas e vacinas são excluídas em cascata pelo banco
        await pets.RemoverAsync(pet);
        return Resultado.Ok();
    }
}
