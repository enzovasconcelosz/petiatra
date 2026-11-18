using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Petiatra.Web.Data;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Enums;
using Petiatra.Web.Repositories;
using Petiatra.Web.Services;
using Petiatra.Web.Services.Regras;

namespace Petiatra.Tests.Apoio;

/// <summary>
/// Monta os serviços reais sobre um banco SQLite em memória (SOLID: substituição de Liskov),
/// com um relógio fixo em 18/11/2026 às 09:00.
/// </summary>
public sealed class Cenario : IDisposable
{
    private readonly SqliteConnection _conexao;

    public Cenario()
    {
        _conexao = new SqliteConnection("Data Source=:memory:");
        _conexao.Open();
        Contexto = NovoContexto();
        Contexto.Database.EnsureCreated();

        var opcoes = Options.Create(new HorarioFuncionamentoOpcoes());
        Tutores = new TutorRepositorio(Contexto);
        Pets = new PetRepositorio(Contexto);
        Veterinarios = new VeterinarioRepositorio(Contexto);
        Consultas = new ConsultaRepositorio(Contexto);
        Vacinas = new VacinaRepositorio(Contexto);

        Regras = [new RegraHorarioDeFuncionamento(opcoes), new RegraConflitoDeHorario(Consultas, opcoes)];
        TutorService = new TutorService(Tutores, Relogio);
        PetService = new PetService(Pets, Tutores, Relogio);
        ConsultaService = new ConsultaService(Consultas, Pets, Veterinarios, Regras, Relogio);
        VacinaService = new VacinaService(Vacinas, Pets, Veterinarios, Relogio);
        AuthService = new AuthService(Veterinarios, new PasswordHasher<Veterinario>(), Relogio);
        AtendimentoService = new AtendimentoService(TutorService, PetService, ConsultaService, new UnidadeDeTrabalho(Contexto));
    }

    public RelogioFixo Relogio { get; } = new();
    public AppDbContext Contexto { get; }
    public TutorRepositorio Tutores { get; }
    public PetRepositorio Pets { get; }
    public VeterinarioRepositorio Veterinarios { get; }
    public ConsultaRepositorio Consultas { get; }
    public VacinaRepositorio Vacinas { get; }
    public IRegraAgendamento[] Regras { get; }
    public TutorService TutorService { get; }
    public PetService PetService { get; }
    public ConsultaService ConsultaService { get; }
    public VacinaService VacinaService { get; }
    public AuthService AuthService { get; }
    public AtendimentoService AtendimentoService { get; }

    /// <summary>Outro contexto sobre o mesmo banco, para conferir o que foi realmente gravado.</summary>
    public AppDbContext NovoContexto() =>
        new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_conexao).Options);

    public async Task<Veterinario> CriarVeterinarioAsync(string email = "ana@teste.com", string crmv = "CRMV-PR 1")
    {
        var veterinario = new Veterinario { Nome = "Dra. Ana", Email = email, Crmv = crmv, CriadoEm = Relogio.Agora() };
        veterinario.DefinirSenha("hash");
        await Veterinarios.AdicionarAsync(veterinario);
        return veterinario;
    }

    public async Task<Tutor> CriarTutorAsync(string cpf = "52998224725")
    {
        var tutor = new Tutor { Nome = "Mariana", Cpf = cpf, Telefone = "(43) 99999-0000", CriadoEm = Relogio.Agora() };
        await Tutores.AdicionarAsync(tutor);
        return tutor;
    }

    public async Task<Pet> CriarPetAsync(Tutor tutor, string nome = "Thor")
    {
        var pet = new Pet { Nome = nome, Especie = Especie.Cachorro, TutorId = tutor.Id, CriadoEm = Relogio.Agora() };
        await Pets.AdicionarAsync(pet);
        return pet;
    }

    public DateTime Hoje(int hora, int minuto = 0) => Relogio.Hoje.ToDateTime(new TimeOnly(hora, minuto));

    public void Dispose()
    {
        Contexto.Dispose();
        _conexao.Dispose();
    }
}
