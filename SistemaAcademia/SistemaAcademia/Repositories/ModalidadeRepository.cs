using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class ModalidadeRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

    public IEnumerable<ModalidadeViewModel> Listar(int idAcademia)
    {
        string sql = @"
            SELECT Id, Nome, ValorModalidade
            FROM Modalidade
            WHERE IdAcademia = @idAcademia
            ORDER BY Nome";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();

        var lista = new List<ModalidadeViewModel>();
        while (reader.Read())
        {
            lista.Add(new ModalidadeViewModel
            {
                Id              = (int)reader["Id"],
                Nome            = reader["Nome"].ToString(),
                ValorModalidade = (decimal)reader["ValorModalidade"]
            });
        }
        return lista;
    }

    public void Cadastrar(ModalidadeViewModel dados, int idAcademia)
    {
        string sql = @"
            INSERT INTO Modalidade (IdAcademia, Nome, ValorModalidade)
            VALUES (@idAcademia, @nome, @valorModalidade)";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idAcademia",      idAcademia);
        comando.Parameters.AddWithValue("@nome",            dados.Nome);
        comando.Parameters.AddWithValue("@valorModalidade", dados.ValorModalidade);

        conexao.Open();
        comando.ExecuteNonQuery();
    }

    public void Editar(ModalidadeViewModel dados, int idAcademia)
    {
        string sql = @"
            UPDATE Modalidade
            SET Nome = @nome, ValorModalidade = @valorModalidade
            WHERE Id = @id AND IdAcademia = @idAcademia";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@id",              dados.Id);
        comando.Parameters.AddWithValue("@idAcademia",      idAcademia);
        comando.Parameters.AddWithValue("@nome",            dados.Nome);
        comando.Parameters.AddWithValue("@valorModalidade", dados.ValorModalidade);

        conexao.Open();
        comando.ExecuteNonQuery();
    }

    public bool Excluir(int id, int idAcademia)
    {
        string sql = @"
            IF NOT EXISTS (SELECT 1 FROM MatriculaCliente WHERE IdModalidade = @id)
            BEGIN
                DELETE FROM Modalidade WHERE Id = @id AND IdAcademia = @idAcademia
                SELECT 1
            END
            ELSE SELECT 0";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@id",        id);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        return (int)comando.ExecuteScalar() == 1;
    }
}