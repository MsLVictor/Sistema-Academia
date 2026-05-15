
using Microsoft.AspNetCore.Mvc;
using SistemaAcademia.Services;

namespace SistemaAcademia.Controllers;


public class LoginController : Controller
{
    private readonly LoginService _loginService = new LoginService();

    public IActionResult Index() => View();
    

    [HttpPost]
    public IActionResult Logar(string email, string senha)
    {
        var usuario = _loginService.Autenticar(email, senha);

        if(usuario != null)
        {
            HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetString("UsuarioCargo", usuario.Cargo.Nome);

            return usuario.Cargo.Nome switch
            {
                "Administrador" => RedirectToAction("Administrador", "Home"),
                "Recepcionista" => RedirectToAction("Recepcionista", "Home"),
                "Instrutor"     => RedirectToAction("Instrutor", "Home"),
                "Cliente"       => RedirectToAction("Cliente", "Home"),
                _               => RedirectToAction("Administrador", "Home")
            };
        }

        ViewBag.Erro = "Usuário ou senha inválidos!";
        
        return View("Index");
    }

    public IActionResult Sair()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}
