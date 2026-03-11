using SistemaAcademia.Enum;

namespace SistemaAcademia.Models;

public abstract class Pessoa
{
    public Pessoa(string nome, string cpf, string email, CargosEnum cargosEnum)
    {
        Nome = nome;
        Cpf = cpf;
        Email = email;
        CargoEnum = cargosEnum;
    }

    public int Id {get; set;}
    public string Nome { get; set; }
    public string Cpf { get; private set; }
    public string Email { get; private set; }
    public CargosEnum CargoEnum { get; private set; }
}
