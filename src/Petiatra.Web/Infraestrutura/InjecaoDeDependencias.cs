using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Petiatra.Web.Data;
using Petiatra.Web.Models;
using Petiatra.Web.Repositories;
using Petiatra.Web.Repositories.Interfaces;
using Petiatra.Web.Services;
using Petiatra.Web.Services.Interfaces;
using Petiatra.Web.Services.Regras;

namespace Petiatra.Web.Infraestrutura;

/// <summary>Liga cada abstração à sua implementação (SOLID: inversão de dependência).</summary>
public static class InjecaoDeDependencias
{
    public static IServiceCollection AddPetiatra(this IServiceCollection services, IConfiguration configuracao)
    {
        services.AddBancoDeDados(configuracao);

        services.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();
        services.AddScoped<IVeterinarioRepositorio, VeterinarioRepositorio>();
        services.AddScoped<ITutorRepositorio, TutorRepositorio>();
        services.AddScoped<IPetRepositorio, PetRepositorio>();
        services.AddScoped<IConsultaRepositorio, ConsultaRepositorio>();
        services.AddScoped<IVacinaRepositorio, VacinaRepositorio>();

        services.Configure<HorarioFuncionamentoOpcoes>(configuracao.GetSection(HorarioFuncionamentoOpcoes.Secao));
        // a ordem segue o diagrama de atividade: primeiro o expediente (RN02), depois o conflito (RN01)
        services.AddScoped<IRegraAgendamento, RegraHorarioDeFuncionamento>();
        services.AddScoped<IRegraAgendamento, RegraConflitoDeHorario>();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IPasswordHasher<Veterinario>, PasswordHasher<Veterinario>>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITutorService, TutorService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IConsultaService, ConsultaService>();
        services.AddScoped<IVacinaService, VacinaService>();
        services.AddScoped<IPainelService, PainelService>();
        services.AddScoped<IBuscaService, BuscaService>();
        services.AddScoped<IAtendimentoService, AtendimentoService>();
        services.AddScoped<DadosDeExemplo>();

        services.AddAutenticacaoPorCookie();
        services.AddControllersWithViews(opcoes =>
        {
            // RNF06: todas as páginas exigem login e todos os formulários são protegidos contra CSRF
            var somenteLogados = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            opcoes.Filters.Add(new AuthorizeFilter(somenteLogados));
            opcoes.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            opcoes.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
        });

        return services;
    }

    /// <summary>RNF02: SQLite por padrão; SQL Server Express trocando apenas o appsettings.json.</summary>
    private static void AddBancoDeDados(this IServiceCollection services, IConfiguration configuracao)
    {
        var provedor = configuracao["Banco:Provedor"] ?? "Sqlite";
        services.AddDbContext<AppDbContext>(opcoes =>
        {
            if (provedor.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
                opcoes.UseSqlServer(configuracao.GetConnectionString("SqlServer"));
            else
                opcoes.UseSqlite(configuracao.GetConnectionString("Sqlite"));
        });
    }

    private static void AddAutenticacaoPorCookie(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.LoginPath = "/Conta/Login";
                opcoes.LogoutPath = "/Conta/Sair";
                opcoes.AccessDeniedPath = "/Conta/Login";
                opcoes.Cookie.Name = "Petiatra.Auth";
                opcoes.Cookie.HttpOnly = true;
                opcoes.Cookie.SameSite = SameSiteMode.Lax;
                opcoes.ExpireTimeSpan = TimeSpan.FromHours(8);
                opcoes.SlidingExpiration = true;
            });
    }

    /// <summary>Cria ou atualiza o banco e, se configurado, grava os dados de exemplo.</summary>
    public static async Task InicializarBancoAsync(this WebApplication app)
    {
        await using var escopo = app.Services.CreateAsyncScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<AppDbContext>();

        if (contexto.Database.IsSqlite())
            await contexto.Database.MigrateAsync();
        else
            await contexto.Database.EnsureCreatedAsync();

        if (app.Configuration.GetValue("Banco:DadosDeExemplo", false))
            await escopo.ServiceProvider.GetRequiredService<DadosDeExemplo>().GravarSeVazioAsync();
    }
}
