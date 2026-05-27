using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class LoginService
{
    private readonly UsuarioRepository _usuarioRepository = new();
    private readonly ClienteRepository _clienteRepository = new();

    public Usuario Autenticar(string email, string senha) =>
        _usuarioRepository.ValidarLogin(email, senha);

    public Cliente AutenticarCliente(string email, string senha) =>
        _clienteRepository.ValidarLogin(email, senha);
}