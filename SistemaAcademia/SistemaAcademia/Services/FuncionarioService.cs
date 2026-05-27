using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class FuncionarioService
{
    private readonly UsuarioRepository _repositorio = new UsuarioRepository();

    public IEnumerable<FuncionarioViewModel> Listar(int idAcademia)
        => _repositorio.ListarPorAcademia(idAcademia);

    public void CadastrarFuncionario(CadastroFuncionarioViewModel dados, int idAcademia)
        => _repositorio.CadastrarFuncionario(dados, idAcademia);

    public void Editar(EditarFuncionarioViewModel dados, int idAcademia)
        => _repositorio.Atualizar(dados, idAcademia);

    public void AlterarStatus(int id, string ativo, int idAcademia, int idLogado)
        => _repositorio.AlterarStatus(id, ativo, idAcademia, idLogado);
}