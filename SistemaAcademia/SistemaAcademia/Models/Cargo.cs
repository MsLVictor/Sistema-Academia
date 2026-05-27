namespace SistemaAcademia.Models;

public class Cargo
{
    public Cargo(string nome)
    {
        Nome = nome;
    }

    public Cargo(int id, string nome)
    {
        Id = id;
        Nome = nome;
    }

    public int Id { get; }
    public string Nome { get; }
}
