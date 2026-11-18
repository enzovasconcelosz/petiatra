using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Petiatra.Web.Infraestrutura;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPetiatra(builder.Configuration);

var app = builder.Build();

// RNF11: interface em português do Brasil
var portugues = new CultureInfo("pt-BR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(portugues),
    SupportedCultures = [portugues],
    SupportedUICultures = [portugues]
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Erro");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Erro/{0}");
app.UseHttpsRedirection();
app.Use(async (contexto, proximo) =>
{
    contexto.Response.Headers.XContentTypeOptions = "nosniff";
    contexto.Response.Headers.XFrameOptions = "DENY";
    contexto.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await proximo();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Painel}/{action=Index}/{id?}")
    .WithStaticAssets();

await app.InicializarBancoAsync();
app.Run();

/// <summary>Visível para os testes de integração.</summary>
public partial class Program;
