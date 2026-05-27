namespace SistemaAcademia.Models;

public class CheckInResultado
{
    public bool   Sucesso     { get; init; }
    public string Tipo        { get; init; } = "Entrada"; // "Entrada" | "Saida"
    public string Mensagem    { get; init; } = string.Empty;
    public string NomeCliente { get; init; } = string.Empty;
    public string Hora        { get; init; } = string.Empty;
}

public class CheckInPresencaViewModel
{
    public int      IdCheckIn         { get; set; }
    public string   NomeCliente       { get; set; } = string.Empty;
    public DateTime DataHoraEntrada   { get; set; }
    public string   NomeRecepcionista { get; set; } = string.Empty;
}

public class CheckInHistoricoViewModel
{
    public int       Id                { get; set; }
    public DateTime  DataHoraEntrada   { get; set; }
    public DateTime? DataHoraSaida     { get; set; }
    public string    NomeRecepcionista { get; set; } = string.Empty;
}