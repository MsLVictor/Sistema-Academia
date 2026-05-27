using Microsoft.AspNetCore.Mvc;
using SistemaAcademia.Filters;
using SistemaAcademia.Models;
using SistemaAcademia.Services;

namespace SistemaAcademia.Controllers;

[SessaoAutorizada]
public class PagamentoController : Controller
{
    private readonly PagamentoService  _pagamentoService  = new PagamentoService();
    private readonly MatriculaService  _matriculaService  = new MatriculaService();

    private int GetIdAcademia() =>
        int.Parse(HttpContext.Session.GetString("UsuarioIdAcademia") ?? "0");

    private bool CargoPermitidoVisualizar()
    {
        var cargo = HttpContext.Session.GetString("UsuarioCargo");
        return cargo == "Administrador" || cargo == "Gerente" || cargo == "Recepcionista";
    }

    private bool CargoPermitidoRegistrar()
    {
        var cargo = HttpContext.Session.GetString("UsuarioCargo");
        return cargo == "Administrador" || cargo == "Recepcionista";
    }

    private string PainelDoUsuario() =>
        HttpContext.Session.GetString("UsuarioCargo") switch
        {
            "Gerente"       => "Gerente",
            "Recepcionista" => "Recepcionista",
            _               => "Administrador"
        };

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
        ViewBag.Pagamentos       = _pagamentoService.ListarPorCliente(idCliente, idAcademia);
        ViewBag.Metodos          = _pagamentoService.ListarMetodos();
        ViewBag.PodeRegistrar    = CargoPermitidoRegistrar();
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(RegistrarPagamentoViewModel dados)
    {
        if (!CargoPermitidoRegistrar()) return RedirectToAction("Index", "Login");

        int idAcademia = GetIdAcademia();
        var (sucesso, erro) = _pagamentoService.Registrar(dados, idAcademia);

        TempData[sucesso ? "SucessoPagamento" : "ErroPagamento"] =
            sucesso ? "Pagamento registrado com sucesso!" : erro;

        return RedirectToAction("Listar", new { idCliente = dados.IdCliente });
    }

    [HttpPost]
    public IActionResult AtualizarVencidos()
    {
        if (HttpContext.Session.GetString("UsuarioCargo") != "Administrador")
            return RedirectToAction("Index", "Login");

        int atualizados = _pagamentoService.AtualizarVencidos(GetIdAcademia());
        TempData["SucessoPagamento"] = $"{atualizados} parcela(s) marcada(s) como Atrasado.";
        return RedirectToAction("Listar", "Home", new { area = "" });
    }
}