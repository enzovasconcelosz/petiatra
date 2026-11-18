using Petiatra.Web.Models;

namespace Petiatra.Web.ViewModels;

public sealed record BuscaViewModel(string Termo, IReadOnlyList<Tutor> Tutores, IReadOnlyList<Pet> Pets)
{
    public bool Vazia => Tutores.Count == 0 && Pets.Count == 0;
}
