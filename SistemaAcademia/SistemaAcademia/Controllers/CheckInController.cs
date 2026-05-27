using Microsoft.AspNetCore.Mvc;
using SistemaAcademia.Filters;
using SistemaAcademia.Services;

namespace SistemaAcademia.Controllers;

[SessaoAutorizada]
public class CheckInController : Controller
{
    private readonly CheckInService  _checkInService  = new CheckInService();
    private readonly MatriculaService _matriculaService = new MatriculaService();

    private bool CargoPermitido()
    {
        var cargo = HttpContext.Session.GetString("UsuarioCargo");
        return cargo == "Administrador" || cargo == "Recepcionista";
    }

    private bool CargoPermitidoVisualizar()
    {
        var cargo = HttpContext.Session.GetString("UsuarioCargo");
        return cargo == "Administrador" || cargo == "Gerente" || cargo == "Recepcionista" || cargo == "Instrutor";
    }

    private long GetIdAcademia() =>
        long.Parse(HttpContext.Session.GetString("UsuarioIdAcademia") ?? "0");

    private long GetIdUsuario() =>
        long.Parse(HttpContext.Session.GetString("UsuarioId") ?? "0");

    [HttpGet]
    public IActionResult Scanner()
    {
        if (!CargoPermitido()) return RedirectToAction("Index", "Login");
        ViewData["Nome"] = HttpContext.Session.GetString("UsuarioNome");
        return View();
    }

    [HttpPost]
    public IActionResult Entrada(string cpf)
    {
        if (!CargoPermitido())
            return Json(new { sucesso = false, mensagem = "Acesso não autorizado." });

        var resultado = _checkInService.RegistrarMovimento(cpf, GetIdAcademia(), GetIdUsuario());
        return Json(resultado);
    }

    [HttpGet]
    public IActionResult Presentes()
    {
        if (!CargoPermitido()) return RedirectToAction("Index", "Login");

        ViewData["Nome"]    = HttpContext.Session.GetString("UsuarioNome");
        ViewBag.Presentes   = _checkInService.ListarPresentes(GetIdAcademia());
        return View();
    }

    [HttpPost]
    public IActionResult Saida(long id)
    {
        if (!CargoPermitido()) return RedirectToAction("Index", "Login");

        _checkInService.RegistrarSaida(id, GetIdAcademia());
        return RedirectToAction("Presentes");
    }

    [HttpGet]
    public IActionResult Historico(long idCliente)
    {
        if (!CargoPermitidoVisualizar()) return RedirectToAction("Index", "Login");

        long idAcademia = GetIdAcademia();
        var cliente     = _matriculaService.BuscarCliente(idCliente, idAcademia);
        if (cliente is null) return RedirectToAction("Listar", "Cliente");

        ViewData["Nome"]  = HttpContext.Session.GetString("UsuarioNome");
        ViewBag.Cliente   = cliente;
        ViewBag.Historico = _checkInService.HistoricoPorCliente(idCliente, idAcademia);
        return View();
    }
}
