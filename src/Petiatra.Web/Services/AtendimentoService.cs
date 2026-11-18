using Petiatra.Web.Models;
using Petiatra.Web.Repositories.Interfaces;
using Petiatra.Web.Services.Interfaces;

namespace Petiatra.Web.Services;

/// <summary>
/// RF13: encadeia UC02, UC03 e UC04. Reaproveita os serviços de cada caso de uso,
/// então as mesmas regras valem aqui, e grava tudo numa só transação.
/// </summary>
public class AtendimentoService(
    ITutorService tutores,
    IPetService pets,
    IConsultaService consultas,
    IUnidadeDeTrabalho unidadeDeTrabalho) : IAtendimentoService
{
    public const string PrefixoTutor = "Tutor.";
    public const string PrefixoPet = "Pet.";
    public const string PrefixoConsulta = "Consulta.";

    public Task<Resultado<Consulta>> RegistrarAsync(Tutor tutor, Pet pet, Consulta consulta) =>
        unidadeDeTrabalho.ExecutarAsync(() => GravarAsync(tutor, pet, consulta), resultado => resultado.Sucesso);

    private async Task<Resultado<Consulta>> GravarAsync(Tutor tutor, Pet pet, Consulta consulta)
    {
        if (tutor.Id == 0)
        {
            var tutorSalvo = await tutores.SalvarAsync(tutor);
            if (!tutorSalvo.Sucesso)
                return ComPrefixo(tutorSalvo, PrefixoTutor);
        }

        if (pet.Id == 0)
        {
            pet.TutorId = tutor.Id;
            var petSalvo = await pets.SalvarAsync(pet);
            if (!petSalvo.Sucesso)
                return ComPrefixo(petSalvo, PrefixoPet);
        }

        consulta.PetId = pet.Id;
        var agendada = await consultas.AgendarAsync(consulta);
        return agendada.Sucesso ? agendada : ComPrefixo(agendada, PrefixoConsulta);
    }

    private static Resultado<Consulta> ComPrefixo(Resultado resultado, string prefixo) =>
        Resultado<Consulta>.Falha(resultado.Erro!, resultado.Campo is null ? null : prefixo + resultado.Campo);
}
