using SistemaAcademia.Enum;

namespace SistemaAcademia.Models;

public class Pagamento
{
    public Pagamento(int idMatricula, decimal valorPrevisto, DateTime dataVencimento)
    {
        IdMatricula = idMatricula;
        ValorPrevisto = valorPrevisto;
        DataVencimento = dataVencimento;
        Status = StatusPagamentoEnum.Pendente;
    }

    public int IdMatricula { get; set; }
    public decimal ValorPrevisto { get; set; }
    public decimal? ValorPago { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public StatusPagamentoEnum Status { get; set; }

    public void RegistrarPagamento()
    {
        Status = StatusPagamentoEnum.Pago;
        DataPagamento = DateTime.Now;
    }

}