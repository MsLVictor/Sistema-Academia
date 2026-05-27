using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class ModalidadeService
{
    private readonly ModalidadeRepository _repositorio = new ModalidadeRepository();

    public IEnumerable<ModalidadeViewModel> Listar(long idAcademia) =>
        _repositorio.Listar(idAcademia);

    public void Cadastrar(ModalidadeViewModel dados, long idAcademia) =>
        _repositorio.Cadastrar(dados, idAcademia);

    public void Editar(ModalidadeViewModel dados, long idAcademia) =>
        _repositorio.Editar(dados, idAcademia);

    public bool Excluir(long id, long idAcademia) =>
        _repositorio.Excluir(id, idAcademia);
}
