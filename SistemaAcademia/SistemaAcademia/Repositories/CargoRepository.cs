using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class CargoRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

    public List<Cargo> BuscarTodos()
    {
        List<Cargo> cargos = new List<Cargo>();

        using SqlConnection conexao = new SqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT Id, Nome FROM Cargo ORDER BY Nome";

        using SqlCommand comando = new SqlCommand(sql, conexao);
        using SqlDataReader leitor = comando.ExecuteReader();

        while (leitor.Read())
        {
            long   identificador = (long)leitor["Id"];
            string nome          = leitor["Nome"].ToString()!;
            cargos.Add(new Cargo(identificador, nome));
        }

        return cargos;
    }
}
