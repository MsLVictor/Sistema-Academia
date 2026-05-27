using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class PagamentoService
{
    private readonly PagamentoRepository _repositorio = new PagamentoRepository();

    public IEnumerable<PagamentoListaViewModel> ListarPorCliente(long idCliente, long idAcademia) =>
        _repositorio.ListarPorCliente(idCliente, idAcademia);

    public IEnumerable<MetodoPagamentoViewModel> ListarMetodos() =>
        _repositorio.ListarMetodos();

    public (bool Sucesso, string Erro) Registrar(RegistrarPagamentoViewModel dados, long idAcademia)
    {
        if (dados.ValorPago <= 0)
            return (false, "Valor pago deve ser maior que zero.");

        bool atualizado = _repositorio.Registrar(dados, idAcademia);
        return atualizado
            ? (true, string.Empty)
            : (false, "Pagamento não encontrado ou já está quitado.");
    }

    public int AtualizarVencidos(long idAcademia) =>
        _repositorio.AtualizarVencidos(idAcademia);

    // hook Asaas — implementar quando credenciais estiverem disponíveis
    public string GerarCobrancaAsaas(long idPagamento, long idAcademia) =>
        _repositorio.GerarCobrancaAsaas(idPagamento, idAcademia);
}
