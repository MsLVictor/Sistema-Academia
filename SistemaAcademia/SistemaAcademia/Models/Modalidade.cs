namespace SistemaAcademia.Models;

public class Modalidade(string nome, decimal valorModalidade, int idAcademia)
{
    public int     Id              { get; init; }
    public string  Nome            { get; } = nome;
    public decimal ValorModalidade { get; } = valorModalidade;
    public int     IdAcademia      { get; } = idAcademia;
}