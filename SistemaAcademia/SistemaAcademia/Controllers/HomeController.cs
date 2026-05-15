using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SistemaAcademia.Models;

namespace SistemaAcademia.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Painel");

    public IActionResult Administrador()
    {
        if (HttpContext.Session.GetString("UsuarioCargo") != "Administrador")
            return RedirectToAction("Index", "Login");

        ViewData["Nome"] = HttpContext.Session.GetString("UsuarioNome");
        return View();
    }

    public IActionResult Recepcionista()
    {
        if (HttpContext.Session.GetString("UsuarioCargo") != "Recepcionista")
            return RedirectToAction("Index", "Login");

        ViewData["Nome"] = HttpContext.Session.GetString("UsuarioNome");
        return View();
    }

    public IActionResult Instrutor()
    {
        if (HttpContext.Session.GetString("UsuarioCargo") != "Instrutor")
            return RedirectToAction("Index", "Login");

        ViewData["Nome"] = HttpContext.Session.GetString("UsuarioNome");
        return View();
    }

    public IActionResult Cliente()
    {
        if (HttpContext.Session.GetString("UsuarioCargo") != "Cliente")
            return RedirectToAction("Index", "Login");

        ViewData["Nome"] = HttpContext.Session.GetString("UsuarioNome");
        return View();
    }
}
