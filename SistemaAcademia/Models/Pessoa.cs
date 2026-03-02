using SistemaAcademia.Enum;

namespace SistemaAcademia.Models;

public abstract class Pessoa
{
    public Pessoa(string nome, DateTime dataNascimento, string cpf, string email, string senha)
    {
        Nome = nome;
        DataNascimento = dataNascimento;
        Cpf = cpf;
        Email = email;
        _senha = senha;
        Id++;
    }
    
    public int Id { get; } = 1;
    public string Nome { get; set; }
    public DateTime DataNascimento {get; private set;}
    public string Cpf { get; private set; }
    public string Email { get; private set; }
    private string _senha { get; set; }
}