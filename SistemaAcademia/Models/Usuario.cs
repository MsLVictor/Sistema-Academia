using SistemaAcademia.Enum;
namespace SistemaAcademia.Models;

public class Usuario
{
    public Usuario(string nome, string sobrenome, string cpf, string email, string senha, CargosEnum cargo)
    {
        Nome = nome;
        Sobrenome = sobrenome;
        CPF = cpf;
        Email = email;
        Senha = senha;
        CargoEnum = cargo;
    }

    public string Nome { get; set; }
    public string Sobrenome { get; set; }
    public string CPF { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public CargosEnum CargoEnum { get; set; }

}
