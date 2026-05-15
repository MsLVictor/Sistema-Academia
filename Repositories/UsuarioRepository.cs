using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class UsuarioRepository
{
    private readonly string _connectionString = @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

    public Usuario ValidarLogin(string email, string senha)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            byte[] hashSenha = SHA256.HashData(Encoding.UTF8.GetBytes(senha.Trim()));

            string query = @"
                SELECT  us.Nome as Nome,
                        us.Email as Email, 
                        ca.Nome as Cargo
                    FROM Usuario us
                        INNER JOIN Cargo ca
                            ON us.IdCargo = ca.Id
                    WHERE us.Email = @email AND us.Senha = @senha";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@email", email.Trim());

            command.Parameters.AddWithValue("@senha", hashSenha);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                string nome = reader["Nome"].ToString()!;
                string emailRetorno = reader["Email"].ToString()!;
                string cargoNome = reader["Cargo"].ToString()!;

                return new Usuario(nome, "", DateTime.MinValue, "", emailRetorno, "", new Cargo(cargoNome));
            }

            return null;
        }
    }
}

