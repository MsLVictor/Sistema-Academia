namespace SistemaAcademia.Models;

public class PlanoViewModel
{
    public int    Id                 { get; set; }
    public string Nome               { get; set; } = string.Empty;
    public string TempoPlano         { get; set; } = string.Empty;
    public float  PercentualDesconto { get; set; }
}

public class ModalidadeViewModel
{
    public int     Id              { get; set; }
    public string  Nome            { get; set; } = string.Empty;
    public decimal ValorModalidade { get; set; }
}

public class MatriculaViewModel
{
    public int      IdCliente                { get; set; }
    public int      IdModalidade             { get; set; }
    public int      IdPlano                  { get; set; }
    public DateTime DataInicio               { get; set; } = DateTime.Today;
    public bool     PrimeiroPagoNoAto        { get; set; }
    public int?     IdMetodoPagamentoPrimeiro { get; set; }
}

public class MatriculaListaViewModel
{
    public int      Id             { get; set; }
    public string   NomeModalidade { get; set; } = string.Empty;
    public string   NomePlano      { get; set; } = string.Empty;
    public int      Meses          { get; set; }
    public decimal  ValorParcela   { get; set; }
    public DateTime DataInicio     { get; set; }
    public string   Status         { get; set; } = string.Empty;
}

public class ClienteMatriculaViewModel
{
    public int    IdCliente   { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string CPF         { get; set; } = string.Empty;
}