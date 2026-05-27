using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

namespace SistemaAcademia.Services;

public class CadastroAcademiaService
{
    private readonly AcademiaRepository _repositorio = new AcademiaRepository();

    public void CadastrarAcademia(CadastroAcademiaViewModel dados)
    {
        string cnpjSomenteNumeros = new string(dados.CNPJAcademia.Where(char.IsDigit).ToArray());
        string cpfSomenteNumeros  = new string(dados.CPFAdministrador.Where(char.IsDigit).ToArray());
        string cepSomenteNumeros  = new string(dados.CEP.Where(char.IsDigit).ToArray());

        Endereco endereco = new Endereco(
            dados.Logradouro,
            dados.Numero,
            dados.Complemento,
            dados.Bairro,
            cepSomenteNumeros,
            dados.Cidade,
            dados.Estado
        );

        Academia academia = new Academia(
            dados.NomeAcademia,
            cnpjSomenteNumeros,
            dados.EmailAcademia,
            idEndereco: 0
        );

        Cargo cargoAdministrador = new Cargo("Administrador");

        Usuario administrador = new Usuario(
            dados.NomeAdministrador,
            "",
            DateTime.MinValue,
            cpfSomenteNumeros,
            dados.EmailAdministrador,
            dados.SenhaAdministrador,
            cargoAdministrador
        );

        _repositorio.CadastrarAcademiaComAdministrador(endereco, academia, administrador);
    }
}
