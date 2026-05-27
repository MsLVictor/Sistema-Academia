namespace SistemaAcademia.Models;

public class Plano(string nome, string tempoPlano, float percentualDesconto, long idAcademia)
{
    public long   Id                 { get; init; }
    public string Nome               { get; } = nome;
    public string TempoPlano         { get; } = tempoPlano;
    public float  PercentualDesconto { get; } = percentualDesconto;
    public long   IdAcademia         { get; } = idAcademia;
}