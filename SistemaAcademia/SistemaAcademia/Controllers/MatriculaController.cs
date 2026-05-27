using Microsoft.AspNetCore.Mvc;
using SistemaAcademia.Filters;
using SistemaAcademia.Models;
using SistemaAcademia.Services;

namespace SistemaAcademia.Controllers;

[SessaoAutorizada]
public class MatriculaController : Controller
{
    private readonly MatriculaService   _matriculaService   = new MatriculaService();
    private readonly PlanoService       _planoService       = new PlanoService();
    private readonly ModalidadeService  _modalidadeService  = new ModalidadeService();
    private readonly PagamentoService   _pagamentoService   = new PagamentoService();

    private int GetIdAcademia() =>
        int.Parse(HttpContext.Session.GetString("UsuarioIdAcademia") ?? "0");

    private bool CargoPermitidoMatricular()
    {
        var cargo = HttpContext.Session.GetString("UsuarioCargo");
        return cargo == "Administrador" || cargo == "Recepcionista";
    }

    private bool CargoPermitidoVisualizar()
    {
        var cargo = HttpContext.Session.GetString("UsuarioCargo");
        return cargo == "Administrador" || cargo == "Gerente" || cargo == "Recepcionista";
    }

    private string PainelDoUsuario() =>
        HttpContext.Session.GetString("UsuarioCargo") switch
        {
            "Gerente"       => "Gerente",
            "Recepcionista" => "Recepcionista",
            _               => "Administrador"
        };

    [HttpGet]
    public IActionResult Cadastrar(int idCliente)
    {
        if (!CargoPermitidoMatricular()) return RedirectToAction("Index", "Login");

        int idAcademia = GetIdAcademia();
        var cliente = _matriculaService.BuscarCliente(idCliente, idAcademia);
        if (cliente is null) return RedirectToAction("Listar", "Cliente");

        ViewData["Nome"]        = HttpContext.Session.GetString("UsuarioNome");
        ViewData["PainelAction"] = PainelDoUsuario();
        ViewBag.Cliente          = cliente;
        ViewBag.Planos           = _planoService.Listar(idAcademia);
        ViewBag.Modalidades      = _modalidadeService.Listar(idAcademia);
        ViewBag.Metodos          = _pagamentoService.ListarMetodos();
        return View();
    }

    [HttpPost]
    public IActionResult Cadastrar(MatriculaViewModel dados)
    {
        if (!CargoPermitidoMatricular()) return RedirectToAction("Index", "Login");

        int idAcademia = GetIdAcademia();
        var (sucesso, erro) = _matriculaService.Matricular(dados, idAcademia);

        if (!sucesso)
        {
            TempData["ErroMatricula"] = erro;
            return RedirectToAction("Cadastrar", new { idCliente = dados.IdCliente });
        }

        TempData["SucessoMatricula"] = "Matrícula realizada com sucesso!";
        return RedirectToAction("Listar", new { idCliente = dados.IdCliente });
    }

    [HttpGet]
    public IActionResult Listar(int idCliente)
    {
        if (!CargoPermitidoVisualizar()) return RedirectToAction("Index", "Login");

        int idAcademia = GetIdAcademia();
        var cliente = _matriculaService.BuscarCliente(idCliente, idAcademia);
        if (cliente is null) return RedirectToAction("Listar", "Cliente");

        ViewData["Nome"]         = HttpContext.Session.GetString("UsuarioNome");
        ViewData["PainelAction"] = PainelDoUsuario();
        ViewBag.Cliente          = cliente;
        ViewBag.Matriculas       = _matriculaService.ListarPorCliente(idCliente, idAcademia);
        return View();
    }

    [HttpPost]
    public IActionResult Cancelar(int id, int idCliente)
    {
        if (!CargoPermitidoMatricular()) return RedirectToAction("Index", "Login");

        try
        {
            _matriculaService.Cancelar(id, GetIdAcademia());
            TempData["SucessoMatricula"] = "Matrícula cancelada e parcelas futuras canceladas.";
        }
        catch
        {
            TempData["ErroMatricula"] = "Erro ao cancelar matrícula.";
        }

        return RedirectToAction("Listar", new { idCliente });
    }
}