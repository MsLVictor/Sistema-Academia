namespace SistemaAcademia.Models;

public class Usuario : Pessoa
{
    public Usuario(string nome, string sobrenome, DateTime dataNascimento, string cpf, string email, string senha, Cargo cargo) : base(nome, cpf, email, senha, dataNascimento)
    {
        Cargo = cargo;
    }

    public long  IdAcademia { get; init; }
    public Cargo Cargo      { get; private set; }

    public void MudarCargo(Cargo novoCargo) => Cargo = novoCargo;
}