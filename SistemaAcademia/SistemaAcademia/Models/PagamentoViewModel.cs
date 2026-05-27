namespace SistemaAcademia.Models;

public class PagamentoListaViewModel
{
    public long     Id               { get; set; }
    public long     IdMatricula      { get; set; }
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
    public long    IdPagamento       { get; set; }
    public long    IdMetodoPagamento { get; set; }
    public decimal ValorPago         { get; set; }
    public long    IdCliente         { get; set; }
}

public class MetodoPagamentoViewModel
{
    public long   Id   { get; set; }
    public string Nome { get; set; } = string.Empty;
}
