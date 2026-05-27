using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class PlanoRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

    public IEnumerable<PlanoViewModel> Listar(int idAcademia)
    {
        string sql = @"
            SELECT Id, Nome, TempoPlano, PercentualDesconto
            FROM Plano
            WHERE IdAcademia = @idAcademia
            ORDER BY Nome";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();

        var lista = new List<PlanoViewModel>();
        while (reader.Read())
        {
            lista.Add(new PlanoViewModel
            {
                Id                 = (int)reader["Id"],
                Nome               = reader["Nome"].ToString(),
                TempoPlano         = reader["TempoPlano"].ToString(),
                PercentualDesconto = (float)(double)reader["PercentualDesconto"]
            });
        }
        return lista;
    }

    public void Cadastrar(PlanoViewModel dados, int idAcademia)
    {
        string sql = @"
            INSERT INTO Plano (IdAcademia, Nome, TempoPlano, PercentualDesconto)
            VALUES (@idAcademia, @nome, @tempoPlano, @percentualDesconto)";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idAcademia",         idAcademia);
        comando.Parameters.AddWithValue("@nome",               dados.Nome);
        comando.Parameters.AddWithValue("@tempoPlano",         dados.TempoPlano.PadLeft(2, '0'));
        comando.Parameters.AddWithValue("@percentualDesconto", dados.PercentualDesconto);

        conexao.Open();
        comando.ExecuteNonQuery();
    }

    public void Editar(PlanoViewModel dados, int idAcademia)
    {
        string sql = @"
            UPDATE Plano
            SET Nome = @nome, TempoPlano = @tempoPlano, PercentualDesconto = @percentualDesconto
            WHERE Id = @id AND IdAcademia = @idAcademia";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@id",                 dados.Id);
        comando.Parameters.AddWithValue("@idAcademia",         idAcademia);
        comando.Parameters.AddWithValue("@nome",               dados.Nome);
        comando.Parameters.AddWithValue("@tempoPlano",         dados.TempoPlano.PadLeft(2, '0'));
        comando.Parameters.AddWithValue("@percentualDesconto", dados.PercentualDesconto);

        conexao.Open();
        comando.ExecuteNonQuery();
    }

    public bool Excluir(int id, int idAcademia)
    {
        string sql = @"
            IF NOT EXISTS (SELECT 1 FROM MatriculaCliente WHERE IdPlano = @id)
            BEGIN
                DELETE FROM Plano WHERE Id = @id AND IdAcademia = @idAcademia
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