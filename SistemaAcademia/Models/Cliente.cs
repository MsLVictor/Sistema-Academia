namespace SistemaAcademia.Models;

public class Cliente : Pessoa
{
    public Cliente(int idUsuario, string nome, string cpf, DateTime dataNascimento, string email, string senha) : base(nome, dataNascimento, cpf, email, senha)
    {
        IdUsuario = idUsuario;
    }

    public DateTime DataInicio = DateTime.Now;
    public int IdUsuario {get; private set;}

    
}
