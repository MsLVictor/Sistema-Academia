using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class MatriculaService
{
    private readonly MatriculaRepository _repositorio = new MatriculaRepository();

    public ClienteMatriculaViewModel BuscarCliente(int idCliente, int idAcademia) =>
        _repositorio.BuscarCliente(idCliente, idAcademia);

    public bool TemMatriculaAtiva(int idCliente) =>
        _repositorio.TemMatriculaAtiva(idCliente);

    public IEnumerable<MatriculaListaViewModel> ListarPorCliente(int idCliente, int idAcademia) =>
        _repositorio.ListarPorCliente(idCliente, idAcademia);

    public (bool Sucesso, string Erro) Matricular(MatriculaViewModel dados, int idAcademia)
    {
        if (_repositorio.TemMatriculaAtiva(dados.IdCliente))
            return (false, "Aluno já possui matrícula ativa.");

        var dadosCalculo = _repositorio.BuscarDadosParaMatricula(dados.IdModalidade, dados.IdPlano, idAcademia);
        if (dadosCalculo is null)
            return (false, "Modalidade ou plano inválido.");

        var (valorModalidade, percentualDesconto, meses) = dadosCalculo.Value;
        _repositorio.Matricular(dados, valorModalidade, percentualDesconto, meses);
        return (true, string.Empty);
    }

    public void Cancelar(int idMatricula, int idAcademia) =>
        _repositorio.Cancelar(idMatricula, idAcademia);
}