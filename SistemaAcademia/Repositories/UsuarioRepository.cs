using Microsoft.Data.SqlClient;
using SistemaAcademia.Enum;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class UsuarioRepository
{
    private readonly string _connectionString = @"Server=MALLIH\SQLEXPRESS;Database=SistemaAcademia;Trusted_Connection=True;TrustServerCertificate=True;";

    public void Adicionar(Usuario usuario)
    {
        using (var conexao = new SqlConnection(_connectionString))
        {
            string sql = "INSERT INTO Usuario (Nome, CPF, Email, Cargo) VALUES(@nome, @cpf, @email, @cargo)";

            var comando = new SqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@nome", usuario.Nome);
            comando.Parameters.AddWithValue("@cpf", usuario.Cpf);
            comando.Parameters.AddWithValue("@email", usuario.Email);
            comando.Parameters.AddWithValue("@cargo", usuario.CargoEnum.ToString());

            conexao.Open();
            comando.ExecuteNonQuery();
        }
    }

    public List<Usuario> ListarTodos()
    {
        var lista = new List<Usuario>();
        using(var conexao = new SqlConnection(_connectionString))
        {
            string sql = "SELECT Id, Nome, CPF, Email, Cargo FROM Usuario";
            var comando = new SqlCommand(sql, conexao);

            conexao.Open();

            using(var reader = comando.ExecuteReader())
            {
                while (reader.Read())
                {
                    string nome = reader["Nome"].ToString();
                    string cpf = reader["CPF"].ToString();
                    string email = reader["Email"].ToString();
                    CargosEnum cargo = (CargosEnum)System.Enum.Parse(typeof(CargosEnum), reader["Cargo"].ToString());

                    var usuario = new Usuario(nome, cpf, email, cargo);

                    usuario.Id = (int)reader["Id"];

                    lista.Add(usuario);

                }
            }
        }
        return lista;
    }

    public Usuario BuscarPorCpf(string cpf)
    {
        using(var conexao = new SqlConnection(_connectionString))
        {
            string sql = "SELECT Id, Nome, CPF, Email, Cargo FROM Usuario WHERE CPF = @cpf";

            var cmd = new SqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@cpf", cpf);

            conexao.Open();
            using(var reader = cmd.ExecuteReader())
            {
                if(reader.Read())
                    return MapearUsuario(reader);
            }
        }
        return null;
    }

    public void Atualizar(Usuario usuario)
    {
        using(var conexao = new SqlConnection(_connectionString))
        {
            string sql = @"UPDATE Usuario
                           SET Nome = @nome,
                               Email = @Email,
                               Cargo = @Cargo
                           WHERE Id = @id";
            
            var comando = new SqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@nome", usuario.Nome);
            comando.Parameters.AddWithValue("@email", usuario.Email);
            comando.Parameters.AddWithValue("@Cargo", usuario.CargoEnum.ToString());
            comando.Parameters.AddWithValue("@id", usuario.Id);

            conexao.Open();
            comando.ExecuteNonQuery();
        }
    }

    public void Excluir(int id)
    {
        using(var conexao = new SqlConnection(_connectionString))
        {
            string sql = "DELETE FROM Usuario WHERE Id = @id";

            var comando = new SqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@id", id);

            conexao.Open();

            int linhasAfetadas = comando.ExecuteNonQuery();

            if(linhasAfetadas > 0)
            {
                Console.WriteLine("\n [SUCESSO] Usuário removido do sistema.");
                return;
            }  

            Console.WriteLine("\n[AVISO] Nenhum usuário encontrado com esse ID.");
            
        }
    }

    private Usuario MapearUsuario(SqlDataReader reader)
    {
        string nome = reader["Nome"].ToString();
        string cpf = reader["CPF"].ToString();
        string email = reader["Email"].ToString();

        CargosEnum cargo = System.Enum.Parse<CargosEnum>(reader["Cargo"].ToString(), true);

        var usuario = new Usuario(nome, cpf, email, cargo);

        usuario.Id = (int)reader["Id"];

        return usuario;
    }
}
