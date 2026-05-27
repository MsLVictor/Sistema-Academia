using Microsoft.AspNetCore.Mvc;
using SistemaAcademia.Models;
using SistemaAcademia.Services;

namespace SistemaAcademia.Controllers;

public class LoginController : Controller
{
    private readonly LoginService _loginService = new();
    private readonly CadastroAcademiaService _cadastroAcademiaService = new();

    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult Logar(string email, string senha)
    {
        var usuario = _loginService.Autenticar(email, senha);

        if (usuario != null)
        {
            HttpContext.Session.SetString("TipoSessao",        "Funcionario");
            HttpContext.Session.SetString("UsuarioId",          usuario.Id.ToString());
            HttpContext.Session.SetString("UsuarioNome",        usuario.Nome);
            HttpContext.Session.SetString("UsuarioEmail",       usuario.Email);
            HttpContext.Session.SetString("UsuarioCargo",       usuario.Cargo.Nome);
            HttpContext.Session.SetString("UsuarioIdAcademia",  usuario.IdAcademia.ToString());

            return usuario.Cargo.Nome switch
            {
                "Administrador" => RedirectToAction("Administrador", "Home"),
                "Gerente"       => RedirectToAction("Gerente",       "Home"),
                "Recepcionista" => RedirectToAction("Recepcionista", "Home"),
                "Instrutor"     => RedirectToAction("Instrutor",     "Home"),
                _               => RedirectToAction("Administrador", "Home")
            };
        }

        ViewBag.Erro = "Usuário ou senha inválidos.";
        return View("Index");
    }

    [HttpPost]
    public IActionResult LogarCliente(string email, string senha)
    {
        var cliente = _loginService.AutenticarCliente(email, senha);

        if (cliente != null)
        {
            HttpContext.Session.SetString("TipoSessao",         "Cliente");
            HttpContext.Session.SetString("ClienteId",           cliente.Id.ToString());
            HttpContext.Session.SetString("ClienteNome",         cliente.Nome);
            HttpContext.Session.SetString("ClienteEmail",        cliente.Email);
            HttpContext.Session.SetString("ClienteIdAcademia",   cliente.IdAcademia.ToString());

            return RedirectToAction("Cliente", "Home");
        }

        ViewBag.ErroCliente = "Usuário ou senha inválidos.";
        return View("Index");
    }

    [HttpPost]
    public IActionResult CadastrarAcademia(CadastroAcademiaViewModel dados)
    {
        try
        {
            _cadastroAcademiaService.CadastrarAcademia(dados);
            TempData["SucessoCadastro"] = "Academia cadastrada com sucesso! Faça login para acessar o sistema.";
            return RedirectToAction("Index");
        }
        catch (InvalidOperationException ex)
        {
            ViewBag.ErroCadastro = ex.Message;
            return View("Index");
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2627)
        {
            ViewBag.ErroCadastro = "CNPJ ou CPF já está em uso.";
            return View("Index");
        }
        catch (Exception)
        {
            ViewBag.ErroCadastro = "Erro inesperado ao cadastrar. Tente novamente.";
            return View("Index");
        }
    }

    public IActionResult Sair()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}