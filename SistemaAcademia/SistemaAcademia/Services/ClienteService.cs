using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class ClienteService
{
    private readonly ClienteRepository _repositorio = new ClienteRepository();

    public IEnumerable<ClienteListaViewModel> Listar(int idAcademia, string busca, string status)
        => _repositorio.Listar(idAcademia, busca, status);

    public int Cadastrar(CadastroClienteViewModel dados, int idAcademia, int idUsuario)
        => _repositorio.Cadastrar(dados, idAcademia, idUsuario);

    public EditarClienteViewModel BuscarPorId(int id, int idAcademia)
        => _repositorio.BuscarPorId(id, idAcademia);

    public void Atualizar(EditarClienteViewModel dados, int idAcademia)
        => _repositorio.Atualizar(dados, idAcademia);

    public void AlterarStatus(int id, string ativo, int idAcademia)
        => _repositorio.AlterarStatus(id, ativo, idAcademia);
}