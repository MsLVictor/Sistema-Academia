namespace SistemaAcademia.Models;

public class Cliente(string nome, string cpf, string email, string senha, DateTime dataNascimento, long idUsuario)
    : Pessoa(nome, cpf, email, senha, dataNascimento)
{
    public long IdAcademia { get; init; }
    public long IdUsuario { get; } = idUsuario;
}