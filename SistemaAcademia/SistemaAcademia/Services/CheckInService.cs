using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class CheckInService
{
    private readonly CheckInRepository _repositorio = new CheckInRepository();

    public CheckInResultado RegistrarMovimento(string cpf, int idAcademia, int idUsuario)
    {
        string cpfLimpo = string.Concat(cpf.Where(char.IsDigit));

        if (cpfLimpo.Length != 11)
            return new CheckInResultado { Sucesso = false, Mensagem = "CPF inválido." };

        var cliente = _repositorio.BuscarClientePorCpf(cpfLimpo, idAcademia);
        if (cliente is null)
            return new CheckInResultado { Sucesso = false, Mensagem = "Aluno não encontrado ou inativo." };

        if (!_repositorio.TemMatriculaAtiva(cliente.Value.Id))
            return new CheckInResultado { Sucesso = false, Mensagem = "Aluno sem matrícula ativa." };

        int? idAberto = _repositorio.BuscarEntradaAbertaId(cliente.Value.Id, idAcademia);
        if (idAberto.HasValue)
        {
            _repositorio.RegistrarSaida(idAberto.Value, idAcademia);
            return new CheckInResultado
            {
                Sucesso     = true,
                Tipo        = "Saida",
                NomeCliente = cliente.Value.Nome,
                Hora        = DateTime.Now.ToString("HH:mm · dd/MM/yyyy")
            };
        }

        _repositorio.RegistrarEntrada(cliente.Value.Id, idAcademia, idUsuario);
        LiberarCatraca(cliente.Value.Id);

        return new CheckInResultado
        {
            Sucesso     = true,
            Tipo        = "Entrada",
            NomeCliente = cliente.Value.Nome,
            Hora        = DateTime.Now.ToString("HH:mm · dd/MM/yyyy")
        };
    }

    public IEnumerable<CheckInPresencaViewModel> ListarPresentes(int idAcademia) =>
        _repositorio.ListarPresentes(idAcademia);

    public IEnumerable<CheckInHistoricoViewModel> HistoricoPorCliente(int idCliente, int idAcademia) =>
        _repositorio.HistoricoPorCliente(idCliente, idAcademia);

    public bool RegistrarSaida(int idCheckIn, int idAcademia) =>
        _repositorio.RegistrarSaida(idCheckIn, idAcademia);

    private void LiberarCatraca(int idCliente)
    {
        // integração com catraca pendente — implementar após identificar modelo/protocolo
    }
}