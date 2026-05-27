namespace SistemaAcademia.Models;

public class DashboardViewModel
{
    public int     TotalAlunos       { get; set; }
    public decimal ReceitaMes        { get; set; }
    public int     VencimentosSemana { get; set; }
    public int     ParcelasAtrasadas { get; set; }
    public int     PresentesHoje     { get; set; }

    public List<ReceitaMensalItem> TendenciaReceita { get; set; } = [];
    public List<AlertaAtrasoItem>  AlertasAtraso    { get; set; } = [];
}

public record ReceitaMensalItem(string Mes, decimal Valor);
public record AlertaAtrasoItem(string NomeCliente, int DiasAtraso, decimal ValorParcela);
