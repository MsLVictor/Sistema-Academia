using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class PagamentoRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

    public IEnumerable<PagamentoListaViewModel> ListarPorCliente(long idCliente, long idAcademia)
    {
        string sql = @"
            SELECT pg.Id, mc.Id AS IdMatricula,
                   mo.Nome AS NomeModalidade, pl.Nome AS NomePlano,
                   pg.ValorPago AS ValorEsperado,
                   ISNULL(pg.ValorPago, 0) AS ValorPago,
                   pg.DataVencimento, pg.DataPagamento,
                   sp.Nome AS Status,
                   ISNULL(mp.Nome, '') AS MetodoPagamento
            FROM Pagamento pg
                INNER JOIN MatriculaCliente mc ON mc.Id = pg.IdMatriculaCliente
                INNER JOIN Modalidade mo       ON mo.Id = mc.IdModalidade
                INNER JOIN Plano pl            ON pl.Id = mc.IdPlano
                INNER JOIN StatusPagamento sp  ON sp.Id = pg.IdStatusPagamento
                LEFT JOIN  MetodoPagamento mp  ON mp.Id = pg.IdMetodoPagamento
                INNER JOIN Cliente cl          ON cl.Id = mc.IdCliente
            WHERE mc.IdCliente = @idCliente AND cl.IdAcademia = @idAcademia
            ORDER BY pg.DataVencimento DESC";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idCliente",  idCliente);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();

        var lista = new List<PagamentoListaViewModel>();
        while (reader.Read())
        {
            lista.Add(new PagamentoListaViewModel
            {
                Id              = (long)reader["Id"],
                IdMatricula     = (long)reader["IdMatricula"],
                NomeModalidade  = reader["NomeModalidade"].ToString(),
                NomePlano       = reader["NomePlano"].ToString(),
                ValorEsperado   = (decimal)reader["ValorEsperado"],
                ValorPago       = (decimal)reader["ValorPago"],
                DataVencimento  = (DateTime)reader["DataVencimento"],
                DataPagamento   = reader["DataPagamento"] == DBNull.Value ? null : (DateTime?)reader["DataPagamento"],
                Status          = reader["Status"].ToString(),
                MetodoPagamento = reader["MetodoPagamento"].ToString()
            });
        }
        return lista;
    }

    public IEnumerable<MetodoPagamentoViewModel> ListarMetodos()
    {
        string sql = "SELECT Id, Nome FROM MetodoPagamento ORDER BY Nome";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();

        var lista = new List<MetodoPagamentoViewModel>();
        while (reader.Read())
            lista.Add(new MetodoPagamentoViewModel { Id = (long)reader["Id"], Nome = reader["Nome"].ToString() });

        return lista;
    }

    public bool Registrar(RegistrarPagamentoViewModel dados, long idAcademia)
    {
        string sql = @"
            UPDATE pg
            SET pg.IdMetodoPagamento = @idMetodo,
                pg.ValorPago         = @valorPago,
                pg.DataPagamento     = CAST(GETDATE() AS DATE),
                pg.IdStatusPagamento = (SELECT Id FROM StatusPagamento WHERE Nome = 'Pago')
            FROM Pagamento pg
                INNER JOIN MatriculaCliente mc ON mc.Id = pg.IdMatriculaCliente
                INNER JOIN Cliente cl          ON cl.Id = mc.IdCliente
            WHERE pg.Id = @id
              AND cl.IdAcademia = @idAcademia
              AND pg.IdStatusPagamento IN (
                  SELECT Id FROM StatusPagamento WHERE Nome IN ('Pendente', 'Atrasado')
              )";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@id",        dados.IdPagamento);
        comando.Parameters.AddWithValue("@idMetodo",  dados.IdMetodoPagamento);
        comando.Parameters.AddWithValue("@valorPago", dados.ValorPago);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        return comando.ExecuteNonQuery() > 0;
    }

    public int AtualizarVencidos(long idAcademia)
    {
        string sql = @"
            UPDATE pg
            SET pg.IdStatusPagamento = (SELECT Id FROM StatusPagamento WHERE Nome = 'Atrasado')
            FROM Pagamento pg
                INNER JOIN MatriculaCliente mc ON mc.Id = pg.IdMatriculaCliente
                INNER JOIN Cliente cl          ON cl.Id = mc.IdCliente
            WHERE cl.IdAcademia = @idAcademia
              AND pg.DataVencimento < CAST(GETDATE() AS DATE)
              AND pg.IdStatusPagamento = (SELECT Id FROM StatusPagamento WHERE Nome = 'Pendente')";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        return comando.ExecuteNonQuery();
    }

    // hook para integração futura com Asaas
    public string GerarCobrancaAsaas(long idPagamento, long idAcademia)
    {
        // integração com Asaas pendente — retornar URL do QR PIX quando implementado
        return null;
    }
}
