using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SistemaAcademia.Filters;
using SistemaAcademia.Models;
using SistemaAcademia.Repositories;
using SistemaAcademia.Services;

namespace SistemaAcademia.Controllers;

[SessaoAutorizada]
public class ClienteController : Controller
{
    private readonly ClienteService    _clienteService    = new ClienteService();
    private readonly MatriculaService  _matriculaService  = new MatriculaService();
    private readonly PlanoService      _planoService      = new PlanoService();
    private readonly ModalidadeService _modalidadeService = new ModalidadeService();
    private readonly PagamentoService  _pagamentoService  = new PagamentoService();

    private int? GetIdAcademia() =>
        int.TryParse(HttpContext.Session.GetString("UsuarioIdAcademia"), out var id) ? id : null;

    private int? GetIdUsuario() =>
        int.TryParse(HttpContext.Session.GetString("UsuarioId"), out var id) ? id : null;

    private bool CargoPermitidoListar()
    {
        var cargo = HttpContext.Session.GetString("UsuarioCargo");
        return cargo == "Administrador" || cargo == "Gerente" || cargo == "Recepcionista";
    }

    private bool CargoPermitidoEditar()
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
    public IActionResult Listar(string busca, string status)
    {
        if (!CargoPermitidoListar()) return RedirectToAction("Index", "Login");

        int? idAcademia = GetIdAcademia();
        if (idAcademia is null) return RedirectToAction("Index", "Login");

        ViewData["Nome"]           = HttpContext.Session.GetString("UsuarioNome");
        ViewData["PainelAction"]   = PainelDoUsuario();
        ViewBag.Clientes           = _clienteService.Listar(idAcademia.Value, busca, status);
        ViewBag.OrientacoesSexuais = new OrientacaoSexualRepository().BuscarTodos();
        ViewBag.Busca              = busca;
        ViewBag.Status             = status;

        return View();
    }

    [HttpGet]
    public IActionResult Cadastrar()
    {
        if (!CargoPermitidoEditar()) return RedirectToAction("Index", "Login");
        if (GetIdAcademia() is null) return RedirectToAction("Index", "Login");

        int idAcademia = GetIdAcademia().Value;
        ViewData["Nome"]           = HttpContext.Session.GetString("UsuarioNome");
        ViewData["PainelAction"]   = PainelDoUsuario();
        ViewBag.OrientacoesSexuais = new OrientacaoSexualRepository().BuscarTodos();
        ViewBag.Modalidades        = _modalidadeService.Listar(idAcademia);
        ViewBag.Planos             = _planoService.Listar(idAcademia);
        ViewBag.Metodos            = _pagamentoService.ListarMetodos();

        return View();
    }

    [HttpPost]
    public IActionResult Cadastrar(CadastroClienteViewModel dados)
    {
        if (!CargoPermitidoEditar()) return RedirectToAction("Index", "Login");

        int? idAcademia = GetIdAcademia();
        int? idUsuario  = GetIdUsuario();
        if (idAcademia is null || idUsuario is null) return RedirectToAction("Index", "Login");

        int idNovoCliente;
        try
        {
            idNovoCliente = _clienteService.Cadastrar(dados, idAcademia.Value, idUsuario.Value);
        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            TempData["ErroCliente"] = "CPF ou e-mail já cadastrado.";
            return RedirectToAction("Listar");
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            TempData["ErroCliente"] = "Dado inválido: verifique orientação sexual e tente novamente.";
            return RedirectToAction("Listar");
        }
        catch (SqlException ex)
        {
            TempData["ErroCliente"] = $"Erro no banco ao cadastrar: {ex.Message}";
            return RedirectToAction("Listar");
        }
        catch (Exception ex)
        {
            TempData["ErroCliente"] = $"Erro ao cadastrar aluno: {ex.Message}";
            return RedirectToAction("Listar");
        }

        if (dados.IdModalidade.HasValue && dados.IdPlano.HasValue && dados.DataInicioMatricula.HasValue)
        {
            var matricula = new MatriculaViewModel
            {
                IdCliente                 = idNovoCliente,
                IdModalidade              = dados.IdModalidade.Value,
                IdPlano                   = dados.IdPlano.Value,
                DataInicio                = dados.DataInicioMatricula.Value,
                PrimeiroPagoNoAto         = dados.PrimeiroPagoNoAto,
                IdMetodoPagamentoPrimeiro  = dados.IdMetodoPagamentoPrimeiro
            };
            try
            {
                var (sucesso, erro) = _matriculaService.Matricular(matricula, idAcademia.Value);
                TempData["SucessoCliente"] = sucesso
                    ? "Aluno cadastrado e matriculado com sucesso!"
                    : $"Aluno cadastrado, mas a matrícula falhou: {erro}";
            }
            catch (Exception)
            {
                TempData["SucessoCliente"] = "Aluno cadastrado com sucesso!";
                TempData["ErroCliente"]    = "Erro ao criar a matrícula. Acesse o perfil do aluno para matriculá-lo manualmente.";
            }
        }
        else
        {
            TempData["SucessoCliente"] = "Aluno cadastrado com sucesso!";
        }

        return RedirectToAction("Listar");
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        if (!CargoPermitidoEditar()) return RedirectToAction("Index", "Login");

        int? idAcademia = GetIdAcademia();
        if (idAcademia is null) return RedirectToAction("Index", "Login");

        var cliente = _clienteService.BuscarPorId(id, idAcademia.Value);
        if (cliente is null) return RedirectToAction("Listar");

        ViewData["Nome"]           = HttpContext.Session.GetString("UsuarioNome");
        ViewData["PainelAction"]   = PainelDoUsuario();
        ViewBag.OrientacoesSexuais = new OrientacaoSexualRepository().BuscarTodos();

        return View(cliente);
    }

    [HttpPost]
    public IActionResult Editar(EditarClienteViewModel dados)
    {
        if (!CargoPermitidoEditar()) return RedirectToAction("Index", "Login");

        int? idAcademia = GetIdAcademia();
        if (idAcademia is null) return RedirectToAction("Index", "Login");

        try
        {
            _clienteService.Atualizar(dados, idAcademia.Value);
            TempData["SucessoCliente"] = "Aluno atualizado com sucesso!";
        }
        catch (Exception)
        {
            TempData["ErroCliente"] = "Erro ao atualizar. Verifique os dados e tente novamente.";
        }

        return RedirectToAction("Listar");
    }

    [HttpPost]
    public IActionResult AlterarStatus(int id, string ativo)
    {
        if (!CargoPermitidoEditar()) return RedirectToAction("Index", "Login");

        int? idAcademia = GetIdAcademia();
        if (idAcademia is null) return RedirectToAction("Index", "Login");

        try
        {
            _clienteService.AlterarStatus(id, ativo, idAcademia.Value);
            TempData["SucessoCliente"] = ativo == "A" ? "Aluno reativado." : "Aluno inativado.";
        }
        catch (Exception)
        {
            TempData["ErroCliente"] = "Erro ao alterar status.";
        }

        return RedirectToAction("Listar");
    }
}