using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class DashboardRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

    public DashboardViewModel Carregar(long idAcademia)
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

        var vm = new DashboardViewModel
        {
            TotalAlunos       = (int)r["TotalAlunos"],
            ReceitaMes        = Convert.ToDecimal(r["ReceitaMes"]),
            VencimentosSemana = (int)r["VencimentosSemana"],
            ParcelasAtrasadas = (int)r["ParcelasAtrasadas"],
            PresentesHoje     = (int)r["PresentesHoje"]
        };
        r.Close();

        vm.TendenciaReceita = CarregarTendencia(conexao, idAcademia);
        vm.AlertasAtraso    = CarregarAlertas(conexao, idAcademia);

        return vm;
    }

    private List<ReceitaMensalItem> CarregarTendencia(SqlConnection conexao, long idAcademia)
    {
        string sql = @"
            SELECT YEAR(pg.DataPagamento) AS Ano, MONTH(pg.DataPagamento) AS Mes,
                   SUM(pg.ValorPago) AS Total
            FROM Pagamento pg
            INNER JOIN MatriculaCliente mc ON mc.Id = pg.IdMatriculaCliente
            INNER JOIN Cliente cl          ON cl.Id = mc.IdCliente
            INNER JOIN StatusPagamento sp  ON sp.Id = pg.IdStatusPagamento
            WHERE cl.IdAcademia = @id
              AND sp.Nome = 'Pago'
              AND pg.DataPagamento >= DATEFROMPARTS(YEAR(DATEADD(MONTH,-5,GETDATE())), MONTH(DATEADD(MONTH,-5,GETDATE())), 1)
            GROUP BY YEAR(pg.DataPagamento), MONTH(pg.DataPagamento)
            ORDER BY Ano, Mes";

        using SqlCommand cmd = new(sql, conexao);
        cmd.Parameters.AddWithValue("@id", idAcademia);
        using SqlDataReader r = cmd.ExecuteReader();

        var porMes = new Dictionary<(int, int), decimal>();
        while (r.Read())
            porMes[(r.GetInt32(0), r.GetInt32(1))] = Convert.ToDecimal(r["Total"]);
        r.Close();

        string[] nomeMes = ["Jan","Fev","Mar","Abr","Mai","Jun","Jul","Ago","Set","Out","Nov","Dez"];
        var lista = new List<ReceitaMensalItem>();
        for (int i = 5; i >= 0; i--)
        {
            var d = DateTime.Today.AddMonths(-i);
            porMes.TryGetValue((d.Year, d.Month), out decimal valor);
            lista.Add(new ReceitaMensalItem(nomeMes[d.Month - 1], valor));
        }
        return lista;
    }

    private List<AlertaAtrasoItem> CarregarAlertas(SqlConnection conexao, long idAcademia)
    {
        string sql = @"
            SELECT TOP 5
                cl.Nome,
                DATEDIFF(DAY, pg.DataVencimento, GETDATE()) AS DiasAtraso,
                pg.ValorPago AS ValorParcela
            FROM Pagamento pg
            INNER JOIN MatriculaCliente mc ON mc.Id = pg.IdMatriculaCliente
            INNER JOIN Cliente cl          ON cl.Id = mc.IdCliente
            INNER JOIN StatusPagamento sp  ON sp.Id = pg.IdStatusPagamento
            WHERE cl.IdAcademia = @id
              AND sp.Nome = 'Pendente'
              AND pg.DataVencimento < CAST(GETDATE() AS DATE)
            ORDER BY DiasAtraso DESC";

        using SqlCommand cmd = new(sql, conexao);
        cmd.Parameters.AddWithValue("@id", idAcademia);
        using SqlDataReader r = cmd.ExecuteReader();

        var lista = new List<AlertaAtrasoItem>();
        while (r.Read())
            lista.Add(new AlertaAtrasoItem(
                r["Nome"].ToString(),
                (int)r["DiasAtraso"],
                Convert.ToDecimal(r["ValorParcela"])));
        return lista;
    }
}
