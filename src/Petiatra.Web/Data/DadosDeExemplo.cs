using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Petiatra.Web.Models;
using Petiatra.Web.Models.Enums;
using Petiatra.Web.Services;

namespace Petiatra.Web.Data;

/// <summary>
/// Dados de demonstração gravados quando o banco está vazio (Banco:DadosDeExemplo = true).
/// As datas são relativas ao dia de hoje, para o painel sempre ter consultas e vacinas para mostrar.
/// </summary>
public class DadosDeExemplo(AppDbContext contexto, IPasswordHasher<Veterinario> hasher, TimeProvider relogio)
{
    public const string EmailDemo = "ana@petiatra.com";
    public const string SenhaDemo = "petiatra123";

    public async Task GravarSeVazioAsync()
    {
        if (await contexto.Veterinarios.AnyAsync())
            return;

        var agora = relogio.Agora();
        var hoje = relogio.Hoje();

        var ana = Veterinario("Dra. Ana Ribeiro", EmailDemo, "CRMV-PR 12345", agora);
        var bruno = Veterinario("Dr. Bruno Matos", "bruno@petiatra.com", "CRMV-PR 23456", agora);

        var mariana = Tutor("Mariana Souza", "529982247", "(43) 99812-3456", "mariana.souza@email.com", "Rua Alagoas, 1200, Centro, Londrina - PR", agora);
        var carlos = Tutor("Carlos Pereira", "111444777", "(43) 99701-2233", null, "Av. Higienópolis, 450, Londrina - PR", agora);
        var juliana = Tutor("Juliana Alves", "390533447", "(43) 98845-1020", "ju.alves@email.com", null, agora);
        var rafael = Tutor("Rafael Lima", "123456789", "(43) 99634-7788", null, null, agora);
        var fernanda = Tutor("Fernanda Costa", "987654321", "(43) 99120-4567", "fernanda.costa@email.com", "Rua Sergipe, 88, Londrina - PR", agora);
        var lucas = Tutor("Lucas Oliveira", "456123789", "(43) 98411-9090", null, null, agora);

        var thor = Pet("Thor", Especie.Cachorro, "Golden Retriever", Sexo.Macho, hoje.AddYears(-3).AddMonths(-2), 32.5m, mariana, agora);
        var mia = Pet("Mia", Especie.Gato, "SRD", Sexo.Femea, hoje.AddYears(-2).AddMonths(-5), 4.2m, mariana, agora);
        var bob = Pet("Bob", Especie.Cachorro, "Shih-tzu", Sexo.Macho, hoje.AddYears(-6), 7.8m, carlos, agora);
        var luna = Pet("Luna", Especie.Gato, "Siamês", Sexo.Femea, hoje.AddMonths(-9), 3.1m, juliana, agora);
        var piu = Pet("Piu", Especie.Ave, "Calopsita", Sexo.NaoInformado, hoje.AddYears(-1).AddMonths(-1), 0.09m, rafael, agora);
        var nina = Pet("Nina", Especie.Cachorro, "Poodle", Sexo.Femea, hoje.AddYears(-8), 5.6m, fernanda, agora);
        var fred = Pet("Fred", Especie.Roedor, "Hamster sírio", Sexo.Macho, hoje.AddMonths(-5), 0.15m, lucas, agora);
        var max = Pet("Max", Especie.Cachorro, "Labrador", Sexo.Macho, hoje.AddYears(-4), 30.2m, lucas, agora);
        thor.Observacoes = "Alérgico a frango. Fica agitado no banho.";

        contexto.AddRange(ana, bruno);
        contexto.AddRange(mariana, carlos, juliana, rafael, fernanda, lucas);
        contexto.AddRange(thor, mia, bob, luna, piu, nina, fred, max);

        contexto.AddRange(
            Realizada(thor, ana, hoje.AddMonths(-2), 10, 0, "Check-up anual", "Saudável, sem alterações", "Manter alimentação e vermifugar em 3 meses", 180m, agora),
            Realizada(mia, bruno, hoje.AddDays(-20), 14, 30, "Vômitos frequentes", "Gastrite leve", "Omeprazol por 7 dias e ração gastrointestinal", 220m, agora),
            Realizada(thor, ana, hoje, 9, 0, "Coceira nas patas", "Dermatite alérgica", "Shampoo terapêutico 2x por semana e antialérgico por 10 dias", 160m, agora),
            Agendada(bob, ana, hoje, 10, 30, "Retorno: avaliação dentária", agora),
            Agendada(luna, bruno, hoje, 14, 0, "Castração: avaliação pré-cirúrgica", agora),
            Agendada(nina, ana, hoje, 15, 30, "Mancando da pata traseira", agora),
            Agendada(max, bruno, hoje.AddDays(1), 9, 30, "Vacinação e check-up", agora),
            Agendada(fred, ana, hoje.AddDays(2), 11, 0, "Pelo caindo", agora),
            Cancelada(piu, bruno, hoje.AddDays(-3), 16, 0, "Corte de unhas e bico", agora));

        contexto.AddRange(
            Vacina(thor, ana, "Antirrábica", hoje.AddMonths(-13), hoje.AddMonths(-1), "RAB2401", agora),
            Vacina(thor, ana, "V10 (Polivalente)", hoje.AddMonths(-10), hoje.AddMonths(2), "V10-778", agora),
            Vacina(mia, bruno, "V4 (Quádrupla felina)", hoje.AddMonths(-14), hoje.AddMonths(-2), "F4-112", agora),
            Vacina(mia, bruno, "V4 (Quádrupla felina)", hoje.AddMonths(-1), hoje.AddMonths(11), "F4-305", agora),
            Vacina(bob, ana, "Antirrábica", hoje.AddMonths(-12).AddDays(10), hoje.AddDays(10), "RAB2312", agora),
            Vacina(nina, ana, "Gripe canina", hoje.AddMonths(-13), hoje.AddDays(-20), "GC-090", agora),
            Vacina(luna, bruno, "V4 (Quádrupla felina)", hoje.AddMonths(-6), hoje.AddMonths(6), "F4-287", agora),
            Vacina(max, bruno, "Giárdia", hoje.AddMonths(-2), null, "GI-555", agora),
            Vacina(max, bruno, "Leishmaniose", hoje.AddMonths(-11).AddDays(-5), hoje.AddDays(25), "LSH-31", agora));

        await contexto.SaveChangesAsync();
    }

    private Veterinario Veterinario(string nome, string email, string crmv, DateTime agora)
    {
        var veterinario = new Veterinario { Nome = nome, Email = email, Crmv = crmv, CriadoEm = agora };
        veterinario.DefinirSenha(hasher.HashPassword(veterinario, SenhaDemo));
        return veterinario;
    }

    private static Tutor Tutor(string nome, string noveDigitos, string telefone, string? email, string? endereco, DateTime agora) =>
        new() { Nome = nome, Cpf = CompletarCpf(noveDigitos), Telefone = telefone, Email = email, Endereco = endereco, CriadoEm = agora };

    private static Pet Pet(string nome, Especie especie, string raca, Sexo sexo, DateOnly nascimento, decimal peso, Tutor tutor, DateTime agora) =>
        new() { Nome = nome, Especie = especie, Raca = raca, Sexo = sexo, DataNascimento = nascimento, PesoKg = peso, Tutor = tutor, CriadoEm = agora };

    private static Consulta Agendada(Pet pet, Veterinario veterinario, DateOnly dia, int hora, int minuto, string motivo, DateTime agora) =>
        new() { Pet = pet, Veterinario = veterinario, DataHora = dia.ToDateTime(new TimeOnly(hora, minuto)), Motivo = motivo, CriadoEm = agora };

    private static Consulta Realizada(Pet pet, Veterinario veterinario, DateOnly dia, int hora, int minuto, string motivo,
        string diagnostico, string tratamento, decimal valor, DateTime agora)
    {
        var consulta = Agendada(pet, veterinario, dia, hora, minuto, motivo, agora);
        consulta.RegistrarAtendimento(diagnostico, tratamento, valor);
        return consulta;
    }

    private static Consulta Cancelada(Pet pet, Veterinario veterinario, DateOnly dia, int hora, int minuto, string motivo, DateTime agora)
    {
        var consulta = Agendada(pet, veterinario, dia, hora, minuto, motivo, agora);
        consulta.Cancelar();
        return consulta;
    }

    private static Vacina Vacina(Pet pet, Veterinario veterinario, string nome, DateOnly aplicacao, DateOnly? reforco, string lote, DateTime agora) =>
        new() { Pet = pet, Veterinario = veterinario, Nome = nome, DataAplicacao = aplicacao, DataReforco = reforco, Lote = lote, CriadoEm = agora };

    /// <summary>Calcula os dois dígitos verificadores para gerar CPFs válidos de exemplo.</summary>
    private static string CompletarCpf(string noveDigitos)
    {
        var cpf = noveDigitos;
        for (var tamanho = 9; tamanho <= 10; tamanho++)
        {
            var soma = 0;
            for (var i = 0; i < tamanho; i++)
                soma += (cpf[i] - '0') * (tamanho + 1 - i);
            var resto = soma * 10 % 11;
            cpf += resto == 10 ? '0' : (char)('0' + resto);
        }
        return cpf;
    }
}
