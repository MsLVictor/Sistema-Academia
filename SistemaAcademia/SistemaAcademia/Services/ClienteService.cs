using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class ClienteService
{
    private readonly ClienteRepository _repositorio = new ClienteRepository();

    public IEnumerable<ClienteListaViewModel> Listar(long idAcademia, string busca, string status)
        => _repositorio.Listar(idAcademia, busca, status);

    public long Cadastrar(CadastroClienteViewModel dados, long idAcademia, long idUsuario)
        => _repositorio.Cadastrar(dados, idAcademia, idUsuario);

    public EditarClienteViewModel BuscarPorId(long id, long idAcademia)
        => _repositorio.BuscarPorId(id, idAcademia);

    public void Atualizar(EditarClienteViewModel dados, long idAcademia)
        => _repositorio.Atualizar(dados, idAcademia);

    public void AlterarStatus(long id, string ativo, long idAcademia)
        => _repositorio.AlterarStatus(id, ativo, idAcademia);
}
