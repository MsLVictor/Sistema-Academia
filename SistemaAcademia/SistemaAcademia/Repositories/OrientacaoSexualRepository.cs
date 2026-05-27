using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class OrientacaoSexualRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

    public IEnumerable<OrientacaoSexual> BuscarTodos()
    {
        string sql = "SELECT Id, Nome FROM OrientacaoSexual ORDER BY Nome";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();
        var lista = new List<OrientacaoSexual>();

        while (reader.Read())
            lista.Add(new OrientacaoSexual((int)reader["Id"], reader["Nome"].ToString()));

        return lista;
    }
}