using Microsoft.AspNetCore.Mvc;
using SistemaAcademia.Filters;
using SistemaAcademia.Models;
using SistemaAcademia.Services;

namespace SistemaAcademia.Controllers;

[SessaoAutorizada]
public class ModalidadeController : Controller
{
    private readonly ModalidadeService _modalidadeService = new ModalidadeService();

    private int GetIdAcademia() =>
        int.Parse(HttpContext.Session.GetString("UsuarioIdAcademia") ?? "0");

    private bool CargoPermitido()
    {
        var cargo = HttpContext.Session.GetString("UsuarioCargo");
        return cargo == "Administrador" || cargo == "Gerente";
    }

    [HttpGet]
    public IActionResult Listar()
    {
        if (!CargoPermitido()) return RedirectToAction("Index", "Login");

        int idAcademia = GetIdAcademia();
        ViewData["Nome"]      = HttpContext.Session.GetString("UsuarioNome");
        ViewBag.Modalidades   = _modalidadeService.Listar(idAcademia);
        return View();
    }

    [HttpPost]
    public IActionResult Cadastrar(ModalidadeViewModel dados)
    {
        if (!CargoPermitido()) return RedirectToAction("Index", "Login");

        try
        {
            _modalidadeService.Cadastrar(dados, GetIdAcademia());
            TempData["SucessoModalidade"] = "Modalidade cadastrada com sucesso!";
        }
        catch
        {
            TempData["ErroModalidade"] = "Erro ao cadastrar modalidade.";
        }

        return RedirectToAction("Listar");
    }

    [HttpPost]
    public IActionResult Editar(ModalidadeViewModel dados)
    {
        if (!CargoPermitido()) return RedirectToAction("Index", "Login");

        try
        {
            _modalidadeService.Editar(dados, GetIdAcademia());
            TempData["SucessoModalidade"] = "Modalidade atualizada com sucesso!";
        }
        catch
        {
            TempData["ErroModalidade"] = "Erro ao atualizar modalidade.";
        }

        return RedirectToAction("Listar");
    }

    [HttpPost]
    public IActionResult Excluir(int id)
    {
        if (!CargoPermitido()) return RedirectToAction("Index", "Login");

        bool excluido = _modalidadeService.Excluir(id, GetIdAcademia());
        TempData[excluido ? "SucessoModalidade" : "ErroModalidade"] =
            excluido ? "Modalidade excluída." : "Não é possível excluir: modalidade possui matrículas vinculadas.";

        return RedirectToAction("Listar");
    }
}