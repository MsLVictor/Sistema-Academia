using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class ModalidadeService
{
    private readonly ModalidadeRepository _repositorio = new ModalidadeRepository();

    public IEnumerable<ModalidadeViewModel> Listar(int idAcademia) =>
        _repositorio.Listar(idAcademia);

    public void Cadastrar(ModalidadeViewModel dados, int idAcademia) =>
        _repositorio.Cadastrar(dados, idAcademia);

    public void Editar(ModalidadeViewModel dados, int idAcademia) =>
        _repositorio.Editar(dados, idAcademia);

    public bool Excluir(int id, int idAcademia) =>
        _repositorio.Excluir(id, idAcademia);
}