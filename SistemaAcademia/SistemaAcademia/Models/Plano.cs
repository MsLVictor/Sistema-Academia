namespace SistemaAcademia.Models;

public class Plano(string nome, string tempoPlano, float percentualDesconto, int idAcademia)
{
    public int    Id                 { get; init; }
    public string Nome               { get; } = nome;
    public string TempoPlano         { get; } = tempoPlano;
    public float  PercentualDesconto { get; } = percentualDesconto;
    public int    IdAcademia         { get; } = idAcademia;
}