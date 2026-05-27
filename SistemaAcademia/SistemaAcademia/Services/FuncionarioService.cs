using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class FuncionarioService
{
    private readonly UsuarioRepository _repositorio = new UsuarioRepository();

    public IEnumerable<FuncionarioViewModel> Listar(long idAcademia)
        => _repositorio.ListarPorAcademia(idAcademia);

    public void CadastrarFuncionario(CadastroFuncionarioViewModel dados, long idAcademia)
        => _repositorio.CadastrarFuncionario(dados, idAcademia);

    public void Editar(EditarFuncionarioViewModel dados, long idAcademia)
        => _repositorio.Atualizar(dados, idAcademia);

    public void AlterarStatus(long id, string ativo, long idAcademia, long idLogado)
        => _repositorio.AlterarStatus(id, ativo, idAcademia, idLogado);
}
