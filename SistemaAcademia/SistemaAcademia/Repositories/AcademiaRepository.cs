using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class AcademiaRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

    public void CadastrarAcademiaComAdministrador(Endereco endereco, Academia academia, Usuario administrador)
    {
        using SqlConnection conexao = new SqlConnection(_connectionString);
        conexao.Open();

        using SqlTransaction transacao = conexao.BeginTransaction();

        try
        {
            int identificadorEndereco = InserirEndereco(endereco, conexao, transacao);
            int identificadorAcademia = InserirAcademia(academia, identificadorEndereco, conexao, transacao);

            int identificadorCargoAdministrador = BuscarIdentificadorCargo("Administrador", conexao, transacao);
            InserirUsuarioAdministrador(administrador, identificadorAcademia, identificadorCargoAdministrador, conexao, transacao);

            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();
            throw;
        }
    }

    private int InserirEndereco(Endereco endereco, SqlConnection conexao, SqlTransaction transacao)
    {
        string sql = @"
            INSERT INTO Endereco (Logradouro, Numero, Complemento, Bairro, CEP, Cidade, Estado)
            OUTPUT INSERTED.Id
            VALUES (@logradouro, @numero, @complemento, @bairro, @cep, @cidade, @estado)";

        using SqlCommand comando = new SqlCommand(sql, conexao, transacao);
        comando.Parameters.AddWithValue("@logradouro",  endereco.Logradouro);
        comando.Parameters.AddWithValue("@numero",      endereco.Numero);
        comando.Parameters.AddWithValue("@complemento", string.IsNullOrEmpty(endereco.Complemento) ? DBNull.Value : endereco.Complemento);
        comando.Parameters.AddWithValue("@bairro",      endereco.Bairro);
        comando.Parameters.AddWithValue("@cep",         endereco.CEP);
        comando.Parameters.AddWithValue("@cidade",      endereco.Cidade);
        comando.Parameters.AddWithValue("@estado",      endereco.Estado);

        return (int)comando.ExecuteScalar()!;
    }

    private int InserirAcademia(Academia academia, int identificadorEndereco, SqlConnection conexao, SqlTransaction transacao)
    {
        string sql = @"
            INSERT INTO Academia (IdEndereco, Nome, CNPJ, Email)
            OUTPUT INSERTED.Id
            VALUES (@idEndereco, @nome, @cnpj, @email)";

        using SqlCommand comando = new SqlCommand(sql, conexao, transacao);
        comando.Parameters.AddWithValue("@idEndereco", identificadorEndereco);
        comando.Parameters.AddWithValue("@nome",       academia.Nome);
        comando.Parameters.AddWithValue("@cnpj",       academia.CNPJ);
        comando.Parameters.AddWithValue("@email",      academia.Email);

        return (int)comando.ExecuteScalar()!;
    }

    private int BuscarIdentificadorCargo(string nomeCargo, SqlConnection conexao, SqlTransaction transacao)
    {
        string sql = "SELECT Id FROM Cargo WHERE Nome = @nomeCargo";

        using SqlCommand comando = new SqlCommand(sql, conexao, transacao);
        comando.Parameters.AddWithValue("@nomeCargo", nomeCargo);

        object? resultado = comando.ExecuteScalar();

        if (resultado is null)
            throw new InvalidOperationException($"Cargo '{nomeCargo}' não encontrado. Execute o script de inserts do banco de dados.");

        return (int)resultado;
    }

    private void InserirUsuarioAdministrador(Usuario administrador, int identificadorAcademia, int identificadorCargo, SqlConnection conexao, SqlTransaction transacao)
    {
        byte[] hashSenha = SHA256.HashData(Encoding.UTF8.GetBytes(administrador.Senha.Trim()));

        string sql = @"
            INSERT INTO Usuario (IdAcademia, IdCargo, Nome, CPF, Email, Senha)
            VALUES (@idAcademia, @idCargo, @nome, @cpf, @email, @senha)";

        using SqlCommand comando = new SqlCommand(sql, conexao, transacao);
        comando.Parameters.AddWithValue("@idAcademia", identificadorAcademia);
        comando.Parameters.AddWithValue("@idCargo",    identificadorCargo);
        comando.Parameters.AddWithValue("@nome",       administrador.Nome);
        comando.Parameters.AddWithValue("@cpf",        administrador.CPF);
        comando.Parameters.AddWithValue("@email",      administrador.Email);
        comando.Parameters.AddWithValue("@senha",      hashSenha);

        comando.ExecuteNonQuery();
    }
}
