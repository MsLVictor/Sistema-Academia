using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class DashboardRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

    public DashboardViewModel Carregar(int idAcademia)
    {
        string sql = @"
            SELECT
                (SELECT COUNT(*)
                 FROM Cliente
                 WHERE IdAcademia = @id AND Ativo = 'A') AS TotalAlunos,

                (SELECT ISNULL(SUM(pg.ValorPago), 0)
                 FROM Pagamento pg
                 INNER JOIN MatriculaCliente mc ON mc.Id = pg.IdMatriculaCliente
                 INNER JOIN Cliente cl          ON cl.Id = mc.IdCliente
                 INNER JOIN StatusPagamento sp  ON sp.Id = pg.IdStatusPagamento
                 WHERE cl.IdAcademia = @id
                   AND sp.Nome = 'Pago'
                   AND MONTH(pg.DataPagamento) = MONTH(GETDATE())
                   AND YEAR(pg.DataPagamento)  = YEAR(GETDATE())) AS ReceitaMes,

                (SELECT COUNT(*)
                 FROM Pagamento pg
                 INNER JOIN MatriculaCliente mc ON mc.Id = pg.IdMatriculaCliente
                 INNER JOIN Cliente cl          ON cl.Id = mc.IdCliente
                 INNER JOIN StatusPagamento sp  ON sp.Id = pg.IdStatusPagamento
                 WHERE cl.IdAcademia = @id
                   AND sp.Nome IN ('Pendente', 'Atrasado')
                   AND pg.DataVencimento BETWEEN CAST(GETDATE() AS DATE)
                                             AND CAST(DATEADD(DAY, 7, GETDATE()) AS DATE)) AS VencimentosSemana,

                (SELECT COUNT(*)
                 FROM Pagamento pg
                 INNER JOIN MatriculaCliente mc ON mc.Id = pg.IdMatriculaCliente
                 INNER JOIN Cliente cl          ON cl.Id = mc.IdCliente
                 INNER JOIN StatusPagamento sp  ON sp.Id = pg.IdStatusPagamento
                 WHERE cl.IdAcademia = @id
                   AND sp.Nome = 'Atrasado') AS ParcelasAtrasadas,

                (SELECT COUNT(*)
                 FROM CheckIn
                 WHERE IdAcademia = @id
                   AND CAST(DataHoraEntrada AS DATE) = CAST(GETDATE() AS DATE)
                   AND DataHoraSaida IS NULL) AS PresentesHoje";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);
        comando.Parameters.AddWithValue("@id", idAcademia);

        conexao.Open();
        using SqlDataReader r = comando.ExecuteReader();
        if (!r.Read()) return new DashboardViewModel();

        return new DashboardViewModel
        {
            TotalAlunos       = (int)r["TotalAlunos"],
            ReceitaMes        = (decimal)r["ReceitaMes"],
            VencimentosSemana = (int)r["VencimentosSemana"],
            ParcelasAtrasadas = (int)r["ParcelasAtrasadas"],
            PresentesHoje     = (int)r["PresentesHoje"]
        };
    }
}
