using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Petiatra.Web.Models;
using Petiatra.Web.Services.Interfaces;
using Petiatra.Web.ViewModels;

namespace Petiatra.Web.Controllers;

/// <summary>UC01: autenticar-se e cadastrar veterinário.</summary>
public class ContaController(IAuthService auth) : AppController
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Painel");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var resultado = await auth.AutenticarAsync(model.Email, model.Senha);
        if (!resultado.Sucesso)
        {
            AdicionarErro(resultado);
            return View(model);
        }

        await EntrarAsync(resultado.Valor!, model.LembrarDeMim);

        return Url.IsLocalUrl(model.ReturnUrl)
            ? LocalRedirect(model.ReturnUrl)
            : RedirectToAction("Index", "Painel");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Cadastrar() => View(new CadastroVeterinarioViewModel());

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Cadastrar(CadastroVeterinarioViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var veterinario = new Veterinario { Nome = model.Nome, Email = model.Email, Crmv = model.Crmv };
        var resultado = await auth.CadastrarAsync(veterinario, model.Senha);
        if (!resultado.Sucesso)
        {
            AdicionarErro(resultado);
            return View(model);
        }

        await EntrarAsync(resultado.Valor!, lembrar: false);
        MensagemSucesso($"Bem-vindo(a) ao Petiatra, {resultado.Valor!.Nome}!");
        return RedirectToAction("Index", "Painel");
    }

    [HttpPost]
    public async Task<IActionResult> Sair()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    private Task EntrarAsync(Veterinario veterinario, bool lembrar)
    {
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, veterinario.Id.ToString()),
            new(ClaimTypes.Name, veterinario.Nome),
            new(ClaimTypes.Email, veterinario.Email),
            new("crmv", veterinario.Crmv)
        ];

        var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidade),
            new AuthenticationProperties { IsPersistent = lembrar });
    }
}
