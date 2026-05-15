namespace SistemaAcademia.Models;

public class Cliente : Pessoa
{
    public Cliente(string nome, string cpf, string email, string senha, DateTime dataNascimento, int idUsuario) : base(nome, cpf, email, senha, dataNascimento)
    {
        IdUsuario = idUsuario;
    }

    public int IdUsuario { get; private set; }
}
