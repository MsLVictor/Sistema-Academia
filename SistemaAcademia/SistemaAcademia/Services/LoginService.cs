using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services
{
    public class LoginService
    {
        private readonly UsuarioRepository _repository = new UsuarioRepository();

        public Usuario Autenticar(string email, string senha) => _repository.ValidarLogin(email, senha);
        
    }
}