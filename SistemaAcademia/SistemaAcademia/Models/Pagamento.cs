using SistemaAcademia.Enum;

namespace SistemaAcademia.Models;

public class Pagamento(int idMatricula, decimal valorPrevisto, DateTime dataVencimento)
{
    public int Id { get; init; }
    public int IdMatricula { get; } = idMatricula;
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