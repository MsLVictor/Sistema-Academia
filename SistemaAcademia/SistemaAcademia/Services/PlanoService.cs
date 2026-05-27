using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class PlanoService
{
    private readonly PlanoRepository _repositorio = new PlanoRepository();

    public IEnumerable<PlanoViewModel> Listar(int idAcademia) =>
        _repositorio.Listar(idAcademia);

    public void Cadastrar(PlanoViewModel dados, int idAcademia) =>
        _repositorio.Cadastrar(dados, idAcademia);

    public void Editar(PlanoViewModel dados, int idAcademia) =>
        _repositorio.Editar(dados, idAcademia);

    public bool Excluir(int id, int idAcademia) =>
        _repositorio.Excluir(id, idAcademia);
}