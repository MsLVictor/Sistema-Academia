namespace SistemaAcademia.Models;

public class Modalidade(string nome, decimal valorModalidade, long idAcademia)
{
    public long    Id              { get; init; }
    public string  Nome            { get; } = nome;
    public decimal ValorModalidade { get; } = valorModalidade;
    public long    IdAcademia      { get; } = idAcademia;
}