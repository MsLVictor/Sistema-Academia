using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using SistemaAcademia.Models;

namespace SistemaAcademia.Repositories;

public class ClienteRepository
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? @"Data Source=DESKTOP-5V5TG5F\SQLEXPRESS;Initial Catalog=SistemaAcademia;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name='SQL Server Management Studio';Command Timeout=0";

    public Cliente ValidarLogin(string email, string senha)
    {
        byte[] hashSenha = SHA256.HashData(Encoding.UTF8.GetBytes(senha.Trim()));

        string query = @"
            SELECT Id, Nome, Email, IdAcademia, IdUsuario, CPF, DataNascimento
            FROM Cliente
            WHERE Email = @email AND Senha = @senha AND Ativo = 'A'";

        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(query, connection);

        command.Parameters.AddWithValue("@email", email.Trim());
        command.Parameters.AddWithValue("@senha", hashSenha);

        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        if (!reader.Read()) return null;

        return new Cliente(
            reader["Nome"].ToString(),
            reader["CPF"].ToString(),
            reader["Email"].ToString(),
            "",
            (DateTime)reader["DataNascimento"],
            (long)reader["IdUsuario"])
        {
            Id         = (long)reader["Id"],
            IdAcademia = (long)reader["IdAcademia"]
        };
    }

    public IEnumerable<ClienteListaViewModel> Listar(long idAcademia, string busca, string status)
    {
        string sql = @"
            SELECT cl.Id, cl.Nome, cl.CPF, cl.Email, cl.DataNascimento, cl.Ativo,
                   (SELECT TOP 1 Telefone FROM Telefone WHERE IdCliente = cl.Id) AS Telefone
            FROM Cliente cl
            WHERE cl.IdAcademia = @idAcademia
              AND (@busca  IS NULL OR cl.Nome LIKE '%' + @busca + '%' OR cl.CPF LIKE '%' + @busca + '%')
              AND (@status IS NULL OR cl.Ativo = @status)
            ORDER BY cl.Nome";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);

        comando.Parameters.AddWithValue("@idAcademia", idAcademia);
        comando.Parameters.AddWithValue("@busca",  string.IsNullOrEmpty(busca)  ? DBNull.Value : (object)busca);
        comando.Parameters.AddWithValue("@status", string.IsNullOrEmpty(status) ? DBNull.Value : (object)status);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();
        var lista = new List<ClienteListaViewModel>();

        while (reader.Read())
        {
            lista.Add(new ClienteListaViewModel
            {
                Id             = (long)reader["Id"],
                Nome           = reader["Nome"].ToString(),
                CPF            = reader["CPF"].ToString(),
                Email          = reader["Email"].ToString(),
                DataNascimento = (DateTime)reader["DataNascimento"],
                Telefone       = reader["Telefone"] == DBNull.Value ? "" : reader["Telefone"].ToString(),
                Ativo          = reader["Ativo"].ToString()
            });
        }

        return lista;
    }

    public long Cadastrar(CadastroClienteViewModel dados, long idAcademia, long idUsuario)
    {
        string cpf   = string.Concat(dados.CPF.Where(char.IsDigit));
        string cep   = string.Concat(dados.CEP.Where(char.IsDigit));
        string tel   = string.Concat(dados.Telefone.Where(char.IsDigit));
        byte[] senha = SHA256.HashData(Encoding.UTF8.GetBytes(dados.Senha.Trim()));

        using SqlConnection conexao = new(_connectionString);
        conexao.Open();
        using SqlTransaction transacao = conexao.BeginTransaction();

        try
        {
            string sqlEndereco = @"
                INSERT INTO Endereco (Logradouro, Numero, Complemento, Bairro, CEP, Cidade, Estado)
                VALUES (@logradouro, @numero, @complemento, @bairro, @cep, @cidade, @estado);
                SELECT SCOPE_IDENTITY();";

            long idEndereco;
            using (var cmd = new SqlCommand(sqlEndereco, conexao, transacao))
            {
                cmd.Parameters.AddWithValue("@logradouro",  dados.Logradouro);
                cmd.Parameters.AddWithValue("@numero",      dados.Numero);
                cmd.Parameters.AddWithValue("@complemento", (object?)dados.Complemento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@bairro",      dados.Bairro);
                cmd.Parameters.AddWithValue("@cep",         cep);
                cmd.Parameters.AddWithValue("@cidade",      dados.Cidade);
                cmd.Parameters.AddWithValue("@estado",      dados.Estado);
                idEndereco = Convert.ToInt64(cmd.ExecuteScalar());
            }

            string sqlCliente = @"
                INSERT INTO Cliente (IdAcademia, IdEndereco, IdUsuario, IdOrientacaoSexual, Nome, CPF, DataNascimento, Email, Senha)
                VALUES (@idAcademia, @idEndereco, @idUsuario, @idOrientacaoSexual, @nome, @cpf, @dataNascimento, @email, @senha);
                SELECT SCOPE_IDENTITY();";

            long idCliente;
            using (var cmd = new SqlCommand(sqlCliente, conexao, transacao))
            {
                cmd.Parameters.AddWithValue("@idAcademia",         idAcademia);
                cmd.Parameters.AddWithValue("@idEndereco",         idEndereco);
                cmd.Parameters.AddWithValue("@idUsuario",          idUsuario);
                cmd.Parameters.AddWithValue("@idOrientacaoSexual", dados.IdOrientacaoSexual);
                cmd.Parameters.AddWithValue("@nome",               dados.Nome);
                cmd.Parameters.AddWithValue("@cpf",                cpf);
                cmd.Parameters.AddWithValue("@dataNascimento",     dados.DataNascimento);
                cmd.Parameters.AddWithValue("@email",              dados.Email.Trim());
                cmd.Parameters.AddWithValue("@senha",              senha);
                idCliente = Convert.ToInt64(cmd.ExecuteScalar());
            }

            string sqlTel = "INSERT INTO Telefone (IdCliente, Telefone) VALUES (@idCliente, @telefone)";
            using (var cmd = new SqlCommand(sqlTel, conexao, transacao))
            {
                cmd.Parameters.AddWithValue("@idCliente", idCliente);
                cmd.Parameters.AddWithValue("@telefone",  tel);
                cmd.ExecuteNonQuery();
            }

            if (!string.IsNullOrWhiteSpace(dados.TelefoneOpcional))
            {
                string telOpc = string.Concat(dados.TelefoneOpcional.Where(char.IsDigit));
                using var cmd = new SqlCommand(sqlTel, conexao, transacao);
                cmd.Parameters.AddWithValue("@idCliente", idCliente);
                cmd.Parameters.AddWithValue("@telefone",  telOpc);
                cmd.ExecuteNonQuery();
            }

            transacao.Commit();
            return idCliente;
        }
        catch
        {
            transacao.Rollback();
            throw;
        }
    }

    public EditarClienteViewModel BuscarPorId(long id, long idAcademia)
    {
        string sql = @"
            SELECT cl.Id, cl.Nome, cl.CPF, cl.Email, cl.DataNascimento, cl.IdOrientacaoSexual,
                   en.Logradouro, en.Numero, en.Complemento, en.Bairro, en.CEP, en.Cidade, en.Estado
            FROM Cliente cl
                INNER JOIN Endereco en ON en.Id = cl.IdEndereco
            WHERE cl.Id = @id AND cl.IdAcademia = @idAcademia";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);

        comando.Parameters.AddWithValue("@id",         id);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        using SqlDataReader reader = comando.ExecuteReader();

        if (!reader.Read()) return null;

        return new EditarClienteViewModel
        {
            Id                 = (long)reader["Id"],
            CPF                = reader["CPF"].ToString(),
            Nome               = reader["Nome"].ToString(),
            Email              = reader["Email"].ToString(),
            DataNascimento     = (DateTime)reader["DataNascimento"],
            IdOrientacaoSexual = (long)reader["IdOrientacaoSexual"],
            Logradouro         = reader["Logradouro"].ToString(),
            Numero             = reader["Numero"].ToString(),
            Complemento        = reader["Complemento"].ToString(),
            Bairro             = reader["Bairro"].ToString(),
            CEP                = reader["CEP"].ToString(),
            Cidade             = reader["Cidade"].ToString(),
            Estado             = reader["Estado"].ToString()
        };
    }

    public void Atualizar(EditarClienteViewModel dados, long idAcademia)
    {
        string cep = string.Concat(dados.CEP.Where(char.IsDigit));

        string sqlCliente = @"
            UPDATE Cliente
            SET Nome = @nome, Email = @email, DataNascimento = @dataNascimento,
                IdOrientacaoSexual = @idOrientacaoSexual
            WHERE Id = @id AND IdAcademia = @idAcademia";

        string sqlEndereco = @"
            UPDATE Endereco
            SET Logradouro = @logradouro, Numero = @numero, Complemento = @complemento,
                Bairro = @bairro, CEP = @cep, Cidade = @cidade, Estado = @estado
            WHERE Id = (SELECT IdEndereco FROM Cliente WHERE Id = @id)";

        using SqlConnection conexao = new(_connectionString);
        conexao.Open();
        using SqlTransaction transacao = conexao.BeginTransaction();

        try
        {
            using (var cmd = new SqlCommand(sqlCliente, conexao, transacao))
            {
                cmd.Parameters.AddWithValue("@nome",               dados.Nome);
                cmd.Parameters.AddWithValue("@email",              dados.Email.Trim());
                cmd.Parameters.AddWithValue("@dataNascimento",     dados.DataNascimento);
                cmd.Parameters.AddWithValue("@idOrientacaoSexual", dados.IdOrientacaoSexual);
                cmd.Parameters.AddWithValue("@id",                 dados.Id);
                cmd.Parameters.AddWithValue("@idAcademia",         idAcademia);
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SqlCommand(sqlEndereco, conexao, transacao))
            {
                cmd.Parameters.AddWithValue("@logradouro",  dados.Logradouro);
                cmd.Parameters.AddWithValue("@numero",      dados.Numero);
                cmd.Parameters.AddWithValue("@complemento", (object?)dados.Complemento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@bairro",      dados.Bairro);
                cmd.Parameters.AddWithValue("@cep",         cep);
                cmd.Parameters.AddWithValue("@cidade",      dados.Cidade);
                cmd.Parameters.AddWithValue("@estado",      dados.Estado);
                cmd.Parameters.AddWithValue("@id",          dados.Id);
                cmd.ExecuteNonQuery();
            }

            transacao.Commit();
        }
        catch
        {
            transacao.Rollback();
            throw;
        }
    }

    public void AlterarStatus(long id, string ativo, long idAcademia)
    {
        string sql = @"
            UPDATE Cliente SET Ativo = @ativo
            WHERE Id = @id AND IdAcademia = @idAcademia";

        using SqlConnection conexao = new(_connectionString);
        using SqlCommand comando    = new(sql, conexao);

        comando.Parameters.AddWithValue("@ativo",      ativo);
        comando.Parameters.AddWithValue("@id",         id);
        comando.Parameters.AddWithValue("@idAcademia", idAcademia);

        conexao.Open();
        comando.ExecuteNonQuery();
    }
}
