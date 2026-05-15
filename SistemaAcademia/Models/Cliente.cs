using SistemaAcademia.Enum;

namespace SistemaAcademia.Models;

public class Cliente : Pessoa
{
    public Cliente(int idUsuario, string nome, string cpf, string email, CargosEnum cargosEnum) : base(nome, cpf, email, cargosEnum)
    {
        IdUsuario = idUsuario;
    }

    public DateTime DataInicio = DateTime.Now;
    public int IdUsuario {get; private set;}
}
