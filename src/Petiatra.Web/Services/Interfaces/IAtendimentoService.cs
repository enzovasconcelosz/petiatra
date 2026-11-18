using Petiatra.Web.Models;

namespace Petiatra.Web.Services.Interfaces;

/// <summary>RF13: o assistente "Novo atendimento" grava tutor, pet e consulta de uma só vez.</summary>
public interface IAtendimentoService
{
    Task<Resultado<Consulta>> RegistrarAsync(Tutor tutor, Pet pet, Consulta consulta);
}
