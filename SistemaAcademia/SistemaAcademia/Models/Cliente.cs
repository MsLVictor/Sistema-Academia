namespace SistemaAcademia.Models;

public class Cliente(string nome, string cpf, string email, string senha, DateTime dataNascimento, int idUsuario)
    : Pessoa(nome, cpf, email, senha, dataNascimento)
{
    public int IdAcademia { get; init; }
    public int IdUsuario { get; } = idUsuario;
}