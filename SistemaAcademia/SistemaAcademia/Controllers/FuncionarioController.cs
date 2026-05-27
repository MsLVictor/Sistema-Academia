using Microsoft.AspNetCore.Mvc;
using SistemaAcademia.Filters;
using SistemaAcademia.Models;
using SistemaAcademia.Services;

namespace SistemaAcademia.Controllers;

[SessaoAutorizada]
public class FuncionarioController : Controller
{
    private readonly FuncionarioService _funcionarioService = new FuncionarioService();

    private long? GetIdAcademia() =>
        long.TryParse(HttpContext.Session.GetString("UsuarioIdAcademia"), out var id) ? id : null;

    private long? GetIdLogado() =>
        long.TryParse(HttpContext.Session.GetString("UsuarioId"), out var id) ? id : null;

    [HttpPost]
    public IActionResult Cadastrar(CadastroFuncionarioViewModel dados)
    {
        long? idAcademia = GetIdAcademia();
        if (idAcademia is null) return RedirectToAction("Index", "Login");

        try
        {
            _funcionarioService.CadastrarFuncionario(dados, idAcademia.Value);
            TempData["SucessoFuncionario"] = "Funcionário cadastrado com sucesso!";
        }
        catch (Exception)
        {
            TempData["ErroFuncionario"] = "Erro ao cadastrar. Verifique se o CPF ou e-mail já está em uso.";
        }

        return RedirectToAction("Administrador", "Home");
    }

    [HttpPost]
    public IActionResult Editar(EditarFuncionarioViewModel dados)
    {
        long? idAcademia = GetIdAcademia();
        if (idAcademia is null) return RedirectToAction("Index", "Login");

        try
        {
            _funcionarioService.Editar(dados, idAcademia.Value);
            TempData["SucessoFuncionario"] = "Funcionário atualizado com sucesso!";
        }
        catch (Exception)
        {
            TempData["ErroFuncionario"] = "Erro ao atualizar. Verifique os dados e tente novamente.";
        }

        return RedirectToAction("Administrador", "Home");
    }

    [HttpPost]
    public IActionResult AlterarStatus(long id, string ativo)
    {
        long? idAcademia = GetIdAcademia();
        long? idLogado   = GetIdLogado();
        if (idAcademia is null || idLogado is null) return RedirectToAction("Index", "Login");

        try
        {
            _funcionarioService.AlterarStatus(id, ativo, idAcademia.Value, idLogado.Value);
            TempData["SucessoFuncionario"] = ativo == "A" ? "Funcionário reativado." : "Funcionário inativado.";
        }
        catch (Exception)
        {
            TempData["ErroFuncionario"] = "Erro ao alterar status do funcionário.";
        }

        return RedirectToAction("Administrador", "Home");
    }
}
