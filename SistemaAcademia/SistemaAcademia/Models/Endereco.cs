namespace SistemaAcademia.Models;

public class Endereco(string logradouro, string numero, string complemento, string bairro, string cep, string cidade, string estado)
{
    public int Id { get; init; }
    public string Logradouro { get; } = logradouro;
    public string Numero { get; } = numero;
    public string Complemento { get; } = complemento;
    public string Bairro { get; } = bairro;
    public string CEP { get; } = cep;
    public string Cidade { get; } = cidade;
    public string Estado { get; } = estado;
}