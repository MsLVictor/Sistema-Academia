using SistemaAcademia.Enum;

namespace SistemaAcademia.Models;

public class Usuario : Pessoa
{
    public Usuario(string nome, string cpf, DateTime dataNascimento, string email, string senha, CargosEnum cargosEnum) : base(nome, dataNascimento, cpf, email, senha)
    {
        CargosEnum = cargosEnum;
    }
    public CargosEnum CargosEnum { get; private set; }

}
