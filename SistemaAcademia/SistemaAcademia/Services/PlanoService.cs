using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class PlanoService
{
    private readonly PlanoRepository _repositorio = new PlanoRepository();

    public IEnumerable<PlanoViewModel> Listar(long idAcademia) =>
        _repositorio.Listar(idAcademia);

    public void Cadastrar(PlanoViewModel dados, long idAcademia) =>
        _repositorio.Cadastrar(dados, idAcademia);

    public void Editar(PlanoViewModel dados, long idAcademia) =>
        _repositorio.Editar(dados, idAcademia);

    public bool Excluir(long id, long idAcademia) =>
        _repositorio.Excluir(id, idAcademia);
}
