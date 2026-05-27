using SistemaAcademia.Enum;

namespace SistemaAcademia.Models;

public class Pagamento(long idMatricula, decimal valorPrevisto, DateTime dataVencimento)
{
    public long Id { get; init; }
    public long IdMatricula { get; } = idMatricula;
    public decimal ValorPrevisto { get; } = valorPrevisto;
    public decimal? ValorPago { get; set; }
    public DateTime DataVencimento { get; } = dataVencimento;
    public DateTime? DataPagamento { get; private set; }
    public StatusPagamentoEnum Status { get; private set; } = StatusPagamentoEnum.Pendente;

    public void RegistrarPagamento()
    {
        Status = StatusPagamentoEnum.Pago;
        DataPagamento = DateTime.Now;
    }
}