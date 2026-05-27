using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class MatriculaRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

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

    public ClienteMatriculaViewModel BuscarCliente(int idCliente, int idAcademia)
    {
        string sql = @"
            SELECT Id, Nome, CPF FROM Cliente
            WHERE Id = @idCliente AND IdAcademia = @idAcademia AND Ativo = 'A'";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idCliente",  idCliente);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();
        if (!reader.Read()) return null;

        return new ClienteMatriculaViewModel
        {
            IdCliente   = (int)reader["Id"],
            NomeCliente = reader["Nome"].ToString(),
            CPF         = reader["CPF"].ToString()
        };
    }

    public IEnumerable<MatriculaListaViewModel> ListarPorCliente(int idCliente, int idAcademia)
    {
        string sql = @"
            SELECT mc.Id, mo.Nome AS NomeModalidade, pl.Nome AS NomePlano,
                   CAST(pl.TempoPlano AS INT) AS Meses,
                   mo.ValorModalidade * (1 - pl.PercentualDesconto / 100.0) AS ValorParcela,
                   mc.DataInicio, mc.StatusSituacao
            FROM MatriculaCliente mc
                INNER JOIN Modalidade mo ON mo.Id = mc.IdModalidade
                INNER JOIN Plano      pl ON pl.Id  = mc.IdPlano
                INNER JOIN Cliente    cl ON cl.Id  = mc.IdCliente
            WHERE mc.IdCliente = @idCliente AND cl.IdAcademia = @idAcademia
            ORDER BY mc.DataInicio DESC";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idCliente",  idCliente);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();

        var lista = new List<MatriculaListaViewModel>();
        while (reader.Read())
        {
            lista.Add(new MatriculaListaViewModel
            {
                Id             = (int)reader["Id"],
                NomeModalidade = reader["NomeModalidade"].ToString(),
                NomePlano      = reader["NomePlano"].ToString(),
                Meses          = (int)reader["Meses"],
                ValorParcela   = Convert.ToDecimal(reader["ValorParcela"]),
                DataInicio     = (DateTime)reader["DataInicio"],
                Status         = reader["StatusSituacao"].ToString() == "A" ? "Ativa" : "Inativa"
            });
        }
        return lista;
    }

    public void Matricular(MatriculaViewModel dados, decimal valorModalidade, float percentualDesconto, int meses)
    {
        using SqlConnection conexao = new(_connectionString);
        conexao.Open();
        using SqlTransaction transacao = conexao.BeginTransaction();

        try
        {
            // 1. Insert matrícula
            string sqlMatricula = @"
                INSERT INTO MatriculaCliente (IdCliente, IdModalidade, IdPlano, DataInicio, StatusSituacao)
                VALUES (@idCliente, @idModalidade, @idPlano, @dataInicio, 'A');
                SELECT SCOPE_IDENTITY();";

            using SqlCommand cmdMatricula = new(sqlMatricula, conexao, transacao);
            cmdMatricula.Parameters.AddWithValue("@idCliente",    dados.IdCliente);
            cmdMatricula.Parameters.AddWithValue("@idModalidade", dados.IdModalidade);
            cmdMatricula.Parameters.AddWithValue("@idPlano",      dados.IdPlano);
            cmdMatricula.Parameters.AddWithValue("@dataInicio",   dados.DataInicio);

            int idMatricula = Convert.ToInt32(cmdMatricula.ExecuteScalar());

            // 2. Buscar ids de status
            string sqlStatus = "SELECT Id, Nome FROM StatusPagamento WHERE Nome IN ('Pendente','Pago')";
            using SqlCommand cmdStatus = new(sqlStatus, conexao, transacao);
            int idStatusPendente = 0, idStatusPago = 0;
            using (var rStatus = cmdStatus.ExecuteReader())
            {
                while (rStatus.Read())
                {
                    if (rStatus["Nome"].ToString() == "Pendente") idStatusPendente = (int)rStatus["Id"];
                    else if (rStatus["Nome"].ToString() == "Pago") idStatusPago     = (int)rStatus["Id"];
                }
            }

            // 3. Gerar mensalidades
            decimal valorParcela = valorModalidade * (1 - (decimal)percentualDesconto / 100);

            string sqlPagamentoPendente = @"
                INSERT INTO Pagamento (IdMatriculaCliente, IdStatusPagamento, ValorPago, DataVencimento)
                VALUES (@idMatricula, @idStatus, @valor, @vencimento)";

            string sqlPagamentoPago = @"
                INSERT INTO Pagamento (IdMatriculaCliente, IdMetodoPagamento, IdStatusPagamento, ValorPago, DataVencimento, DataPagamento)
                VALUES (@idMatricula, @idMetodo, @idStatus, @valor, @vencimento, CAST(GETDATE() AS DATE))";

            for (int i = 0; i < meses; i++)
            {
                bool primeiroPago = i == 0 && dados.PrimeiroPagoNoAto;
                string sql = primeiroPago ? sqlPagamentoPago : sqlPagamentoPendente;

                using SqlCommand cmdPagamento = new(sql, conexao, transacao);
                cmdPagamento.Parameters.AddWithValue("@idMatricula", idMatricula);
                cmdPagamento.Parameters.AddWithValue("@idStatus",    primeiroPago ? idStatusPago : idStatusPendente);
                cmdPagamento.Parameters.AddWithValue("@valor",       valorParcela);
                cmdPagamento.Parameters.AddWithValue("@vencimento",  dados.DataInicio.AddMonths(i));
                if (primeiroPago)
                    cmdPagamento.Parameters.AddWithValue("@idMetodo", (object?)dados.IdMetodoPagamentoPrimeiro ?? DBNull.Value);
                cmdPagamento.ExecuteNonQuery();
            }

            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();
            throw;
        }
    }

    public void Cancelar(int idMatricula, int idAcademia)
    {
        using SqlConnection conexao = new(_connectionString);
        conexao.Open();
        using SqlTransaction transacao = conexao.BeginTransaction();

        try
        {
            string sqlInativa = @"
                UPDATE MatriculaCliente SET StatusSituacao = 'I'
                WHERE Id = @id
                  AND IdCliente IN (SELECT Id FROM Cliente WHERE IdAcademia = @idAcademia)";

            using SqlCommand cmdInativa = new(sqlInativa, conexao, transacao);
            cmdInativa.Parameters.AddWithValue("@id",        idMatricula);
            cmdInativa.Parameters.AddWithValue("@idAcademia", idAcademia);
            cmdInativa.ExecuteNonQuery();

            string sqlCancela = @"
                UPDATE Pagamento
                SET IdStatusPagamento = (SELECT Id FROM StatusPagamento WHERE Nome = 'Cancelado')
                WHERE IdMatriculaCliente = @id
                  AND DataVencimento > CAST(GETDATE() AS DATE)
                  AND IdStatusPagamento = (SELECT Id FROM StatusPagamento WHERE Nome = 'Pendente')";

            using SqlCommand cmdCancela = new(sqlCancela, conexao, transacao);
            cmdCancela.Parameters.AddWithValue("@id", idMatricula);
            cmdCancela.ExecuteNonQuery();

            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();
            throw;
        }
    }

    public (decimal ValorModalidade, float PercentualDesconto, int Meses)? BuscarDadosParaMatricula(int idModalidade, int idPlano, int idAcademia)
    {
        string sql = @"
            SELECT mo.ValorModalidade, pl.PercentualDesconto, CAST(pl.TempoPlano AS INT) AS Meses
            FROM Modalidade mo, Plano pl
            WHERE mo.Id = @idModalidade AND mo.IdAcademia = @idAcademia
              AND pl.Id = @idPlano      AND pl.IdAcademia = @idAcademia";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@idModalidade", idModalidade);
        comando.Parameters.AddWithValue("@idPlano",      idPlano);
        comando.Parameters.AddWithValue("@idAcademia",   idAcademia);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();
        if (!reader.Read()) return null;

        return (
            (decimal)reader["ValorModalidade"],
            (float)(double)reader["PercentualDesconto"],
            (int)reader["Meses"]
        );
    }
}