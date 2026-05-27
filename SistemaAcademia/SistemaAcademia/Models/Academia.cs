namespace SistemaAcademia.Models;

public class Academia(string nome, string cnpj, string email, long idEndereco)
{
    public long Id { get; init; }
    public string Nome { get; } = nome;
    public string CNPJ { get; } = cnpj;
    public string Email { get; } = email;
    public long IdEndereco { get; } = idEndereco;
}