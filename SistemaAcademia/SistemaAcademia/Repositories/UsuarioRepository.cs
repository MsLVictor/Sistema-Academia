using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class UsuarioRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

    public Usuario ValidarLogin(string email, string senha)
    {
        byte[] hashSenha = SHA256.HashData(Encoding.UTF8.GetBytes(senha.Trim()));

        string query = @"
            SELECT us.Id, us.Nome, us.Email, us.IdAcademia, ca.Nome AS Cargo
            FROM Usuario us
                INNER JOIN Cargo ca ON us.IdCargo = ca.Id
            WHERE us.Email = @email AND us.Senha = @senha AND us.Ativo = 'A'";

        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(query, connection);

        command.Parameters.AddWithValue("@email", email.Trim());
        command.Parameters.AddWithValue("@senha", hashSenha);

        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        if (!reader.Read()) return null;

        return new Usuario(
            reader["Nome"].ToString(),
            "", DateTime.MinValue, "",
            reader["Email"].ToString(),
            "",
            new Cargo(reader["Cargo"].ToString()))
        {
            Id         = (long)reader["Id"],
            IdAcademia = (long)reader["IdAcademia"]
        };
    }

    public IEnumerable<FuncionarioViewModel> ListarPorAcademia(long idAcademia)
    {
        string sql = @"
            SELECT us.Id, us.Nome, us.CPF, us.Email, us.Ativo,
                   ca.Id AS IdCargo, ca.Nome AS Cargo
            FROM Usuario us
                INNER JOIN Cargo ca ON us.IdCargo = ca.Id
            WHERE us.IdAcademia = @idAcademia
            ORDER BY us.Nome";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando = new(sql, conexao);

        comando.Parameters.AddWithValue("@idAcademia", idAcademia);
        conexao.Open();

        using SqlDataReader reader = comando.ExecuteReader();
        var lista = new List<FuncionarioViewModel>();

        while (reader.Read())
        {
            lista.Add(new FuncionarioViewModel
            {
                Id      = (long)reader["Id"],
                Nome    = reader["Nome"].ToString(),
                CPF     = reader["CPF"].ToString(),
                Email   = reader["Email"].ToString(),
                Cargo   = reader["Cargo"].ToString(),
                IdCargo = (long)reader["IdCargo"],
                Ativo   = reader["Ativo"].ToString()
            });
        }

        return lista;
    }

    public void CadastrarFuncionario(CadastroFuncionarioViewModel dados, long idAcademia)
    {
        string cpf       = string.Concat(dados.CPFFuncionario.Where(char.IsDigit));
        byte[] hashSenha = SHA256.HashData(Encoding.UTF8.GetBytes(dados.SenhaFuncionario.Trim()));

        string sql = @"
            INSERT INTO Usuario (IdAcademia, IdCargo, Nome, CPF, Email, Senha)
            VALUES (@idAcademia, @idCargo, @nome, @cpf, @email, @senha)";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);

        comando.Parameters.AddWithValue("@idAcademia", idAcademia);
        comando.Parameters.AddWithValue("@idCargo",    dados.IdCargo);
        comando.Parameters.AddWithValue("@nome",       dados.NomeFuncionario);
        comando.Parameters.AddWithValue("@cpf",        cpf);
        comando.Parameters.AddWithValue("@email",      dados.EmailFuncionario);
        comando.Parameters.AddWithValue("@senha",      hashSenha);

        conexao.Open();
        comando.ExecuteNonQuery();
    }

    public void Atualizar(EditarFuncionarioViewModel dados, long idAcademia)
    {
        string sql = @"
            UPDATE Usuario
            SET Nome = @nome, Email = @email, IdCargo = @idCargo
            WHERE Id = @id AND IdAcademia = @idAcademia";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);

        comando.Parameters.AddWithValue("@nome",       dados.NomeFuncionario);
        comando.Parameters.AddWithValue("@email",      dados.EmailFuncionario);
        comando.Parameters.AddWithValue("@idCargo",    dados.IdCargo);
        comando.Parameters.AddWithValue("@id",         dados.Id);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        comando.ExecuteNonQuery();
    }

    public void AlterarStatus(long id, string ativo, long idAcademia, long idLogado)
    {
        string sql = @"
            UPDATE Usuario SET Ativo = @ativo
            WHERE Id = @id AND IdAcademia = @idAcademia AND Id != @idLogado";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);

        comando.Parameters.AddWithValue("@ativo",      ativo);
        comando.Parameters.AddWithValue("@id",         id);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);
        comando.Parameters.AddWithValue("@idLogado",   idLogado);

        conexao.Open();
        comando.ExecuteNonQuery();
    }
}
