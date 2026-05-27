namespace SistemaAcademia.Models;

public class Cargo
{
    public Cargo(string nome)
    {
        Nome = nome;
    }

    public Cargo(long id, string nome)
    {
        Id = id;
        Nome = nome;
    }

    public long Id { get; }
    public string Nome { get; }
}
