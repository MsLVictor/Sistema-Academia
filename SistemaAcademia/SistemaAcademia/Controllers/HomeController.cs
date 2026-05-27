using Microsoft.AspNetCore.Mvc;
using SistemaAcademia.Filters;
using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Controllers;

[SessaoAutorizada]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("TipoSessao") == "Cliente")
            return RedirectToAction("Cliente");

        return HttpContext.Session.GetString("UsuarioCargo") switch
        {
            "Gerente"       => RedirectToAction("Gerente"),
            "Recepcionista" => RedirectToAction("Recepcionista"),
            "Instrutor"     => RedirectToAction("Instrutor"),
            "Administrador" => RedirectToAction("Administrador"),
            _               => RedirectToAction("Index", "Login")
        };
    }

    public IActionResult Administrador()
    {
        if (HttpContext.Session.GetString("UsuarioCargo") != "Administrador")
            return RedirectToAction("Index", "Login");

        long idAcademia = long.Parse(HttpContext.Session.GetString("UsuarioIdAcademia") ?? "0");

        ViewData["Nome"]     = HttpContext.Session.GetString("UsuarioNome");
        ViewBag.Cargos       = new CargoRepository().BuscarTodos();
        ViewBag.Funcionarios = new UsuarioRepository().ListarPorAcademia(idAcademia);
        ViewBag.Dashboard    = new DashboardRepository().Carregar(idAcademia);
        ViewBag.SecaoInicial = TempData["SucessoFuncionario"] != null || TempData["ErroFuncionario"] != null
            ? "funcionarios"
            : "dashboard";

        return View();
    }

    public IActionResult Gerente()
    {
        if (HttpContext.Session.GetString("UsuarioCargo") != "Gerente")
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
        if (HttpContext.Session.GetString("TipoSessao") != "Cliente")
            return RedirectToAction("Index", "Login");

        ViewData["Nome"] = HttpContext.Session.GetString("ClienteNome");
        return View();
    }
}