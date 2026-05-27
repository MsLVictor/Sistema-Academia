using SistemaAcademia.Enum;

namespace SistemaAcademia.Models;

public class Usuario : Pessoa
{
    public Usuario(string nome, string cpf, string email, CargosEnum cargosEnum) : base(nome, cpf, email, cargosEnum)
    {
        
    }

        public CargosEnum CargosEnum { get; private set; }

}
