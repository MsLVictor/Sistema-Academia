using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class CheckInRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

    public (int Id, string Nome)? BuscarClientePorCpf(string cpf, int idAcademia)
    {
        string sql = @"
            SELECT Id, Nome FROM Cliente
            WHERE CPF = @cpf AND IdAcademia = @idAcademia AND Ativo = 'A'";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@cpf",        cpf);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();
        if (!reader.Read()) return null;
        return ((int)reader["Id"], reader["Nome"].ToString());
    }

    public bool TemMatriculaAtiva(int idCliente)
    {
        string sql = @"
            SELECT COUNT(1) FROM MatriculaCliente
            WHERE IdCliente = @idCliente AND StatusSituacao = 'A'";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idCliente", idCliente);

        conexao.Open();
        return (int)comando.ExecuteScalar() > 0;
    }

    public bool TemEntradaAberta(int idCliente, int idAcademia)
    {
        string sql = @"
            SELECT COUNT(1) FROM CheckIn
            WHERE IdCliente = @idCliente AND IdAcademia = @idAcademia
              AND DataHoraSaida IS NULL
              AND CAST(DataHoraEntrada AS DATE) = CAST(GETDATE() AS DATE)";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idCliente",  idCliente);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        return (int)comando.ExecuteScalar() > 0;
    }

    public int? BuscarEntradaAbertaId(int idCliente, int idAcademia)
    {
        string sql = @"
            SELECT TOP 1 Id FROM CheckIn
            WHERE IdCliente = @idCliente AND IdAcademia = @idAcademia
              AND DataHoraSaida IS NULL
              AND CAST(DataHoraEntrada AS DATE) = CAST(GETDATE() AS DATE)
            ORDER BY DataHoraEntrada DESC";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idCliente",  idCliente);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        var resultado = comando.ExecuteScalar();
        return resultado == DBNull.Value || resultado is null ? null : (int?)Convert.ToInt32(resultado);
    }

    public void RegistrarEntrada(int idCliente, int idAcademia, int idUsuario)
    {
        string sql = @"
            INSERT INTO CheckIn (IdCliente, IdAcademia, IdUsuarioRegistro, DataHoraEntrada)
            VALUES (@idCliente, @idAcademia, @idUsuario, GETDATE())";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idCliente",  idCliente);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);
        comando.Parameters.AddWithValue("@idUsuario",  idUsuario);

        conexao.Open();
        comando.ExecuteNonQuery();
    }

    public bool RegistrarSaida(int idCheckIn, int idAcademia)
    {
        string sql = @"
            UPDATE CheckIn SET DataHoraSaida = GETDATE()
            WHERE Id = @id AND IdAcademia = @idAcademia AND DataHoraSaida IS NULL";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@id",        idCheckIn);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        return comando.ExecuteNonQuery() > 0;
    }

    public IEnumerable<CheckInPresencaViewModel> ListarPresentes(int idAcademia)
    {
        string sql = @"
            SELECT ci.Id, cl.Nome AS NomeCliente, ci.DataHoraEntrada,
                   us.Nome AS NomeRecepcionista
            FROM CheckIn ci
                INNER JOIN Cliente cl  ON cl.Id = ci.IdCliente
                INNER JOIN Usuario us  ON us.Id = ci.IdUsuarioRegistro
            WHERE ci.IdAcademia = @idAcademia
              AND ci.DataHoraSaida IS NULL
              AND CAST(ci.DataHoraEntrada AS DATE) = CAST(GETDATE() AS DATE)
            ORDER BY ci.DataHoraEntrada";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();

        var lista = new List<CheckInPresencaViewModel>();
        while (reader.Read())
        {
            lista.Add(new CheckInPresencaViewModel
            {
                IdCheckIn         = (int)reader["Id"],
                NomeCliente       = reader["NomeCliente"].ToString(),
                DataHoraEntrada   = (DateTime)reader["DataHoraEntrada"],
                NomeRecepcionista = reader["NomeRecepcionista"].ToString()
            });
        }
        return lista;
    }

    public IEnumerable<CheckInHistoricoViewModel> HistoricoPorCliente(int idCliente, int idAcademia)
    {
        string sql = @"
            SELECT ci.Id, ci.DataHoraEntrada, ci.DataHoraSaida,
                   us.Nome AS NomeRecepcionista
            FROM CheckIn ci
                INNER JOIN Usuario us ON us.Id = ci.IdUsuarioRegistro
                INNER JOIN Cliente cl ON cl.Id = ci.IdCliente
            WHERE ci.IdCliente = @idCliente AND ci.IdAcademia = @idAcademia
            ORDER BY ci.DataHoraEntrada DESC";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idCliente",  idCliente);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();

        var lista = new List<CheckInHistoricoViewModel>();
        while (reader.Read())
        {
            lista.Add(new CheckInHistoricoViewModel
            {
                Id                = (int)reader["Id"],
                DataHoraEntrada   = (DateTime)reader["DataHoraEntrada"],
                DataHoraSaida     = reader["DataHoraSaida"] == DBNull.Value ? null : (DateTime?)reader["DataHoraSaida"],
                NomeRecepcionista = reader["NomeRecepcionista"].ToString()
            });
        }
        return lista;
    }
}
