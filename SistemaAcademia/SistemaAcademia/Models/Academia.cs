namespace SistemaAcademia.Models;

public class Academia(string nome, string cnpj, string email, int idEndereco)
{
    public int Id { get; init; }
    public string Nome { get; } = nome;
    public string CNPJ { get; } = cnpj;
    public string Email { get; } = email;
    public int IdEndereco { get; } = idEndereco;
}