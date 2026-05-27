namespace SistemaAcademia.Models;

public class PagamentoListaViewModel
{
    public int      Id               { get; set; }
    public int      IdMatricula      { get; set; }
    public string   NomeModalidade   { get; set; } = string.Empty;
    public string   NomePlano        { get; set; } = string.Empty;
    public decimal  ValorEsperado    { get; set; }
    public decimal  ValorPago        { get; set; }
    public DateTime DataVencimento   { get; set; }
    public DateTime? DataPagamento   { get; set; }
    public string   Status           { get; set; } = string.Empty;
    public string   MetodoPagamento  { get; set; } = string.Empty;
}

public class RegistrarPagamentoViewModel
{
    public int     IdPagamento       { get; set; }
    public int     IdMetodoPagamento { get; set; }
    public decimal ValorPago         { get; set; }
    public int     IdCliente         { get; set; }
}

public class MetodoPagamentoViewModel
{
    public int    Id   { get; set; }
    public string Nome { get; set; } = string.Empty;
}