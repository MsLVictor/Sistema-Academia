using Microsoft.AspNetCore.Mvc;
using SistemaAcademia.Filters;
using SistemaAcademia.Models;
using SistemaAcademia.Services;

namespace SistemaAcademia.Controllers;

[SessaoAutorizada]
public class PlanoController : Controller
{
    private readonly PlanoService _planoService = new PlanoService();

    private long GetIdAcademia() =>
        long.Parse(HttpContext.Session.GetString("UsuarioIdAcademia") ?? "0");

    private bool CargoPermitido()
    {
        var cargo = HttpContext.Session.GetString("UsuarioCargo");
        return cargo == "Administrador" || cargo == "Gerente";
    }

    [HttpGet]
    public IActionResult Listar()
    {
        if (!CargoPermitido()) return RedirectToAction("Index", "Login");

        long idAcademia = GetIdAcademia();
        ViewData["Nome"]  = HttpContext.Session.GetString("UsuarioNome");
        ViewBag.Planos    = _planoService.Listar(idAcademia);
        return View();
    }

    [HttpPost]
    public IActionResult Cadastrar(PlanoViewModel dados)
    {
        if (!CargoPermitido()) return RedirectToAction("Index", "Login");

        try
        {
            _planoService.Cadastrar(dados, GetIdAcademia());
            TempData["SucessoPlano"] = "Plano cadastrado com sucesso!";
        }
        catch
        {
            TempData["ErroPlano"] = "Erro ao cadastrar plano.";
        }

        return RedirectToAction("Listar");
    }

    [HttpPost]
    public IActionResult Editar(PlanoViewModel dados)
    {
        if (!CargoPermitido()) return RedirectToAction("Index", "Login");

        try
        {
            _planoService.Editar(dados, GetIdAcademia());
            TempData["SucessoPlano"] = "Plano atualizado com sucesso!";
        }
        catch
        {
            TempData["ErroPlano"] = "Erro ao atualizar plano.";
        }

        return RedirectToAction("Listar");
    }

    [HttpPost]
    public IActionResult Excluir(long id)
    {
        if (!CargoPermitido()) return RedirectToAction("Index", "Login");

        bool excluido = _planoService.Excluir(id, GetIdAcademia());
        TempData[excluido ? "SucessoPlano" : "ErroPlano"] =
            excluido ? "Plano excluído." : "Não é possível excluir: plano possui matrículas vinculadas.";

        return RedirectToAction("Listar");
    }
}
