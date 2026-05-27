namespace SistemaAcademia.Models;

public class CadastroAcademiaViewModel
{
    public string NomeAdministrador { get; set; } = string.Empty;
    public string CPFAdministrador { get; set; } = string.Empty;
    public string EmailAdministrador { get; set; } = string.Empty;
    public string SenhaAdministrador { get; set; } = string.Empty;

    public string NomeAcademia { get; set; } = string.Empty;
    public string CNPJAcademia { get; set; } = string.Empty;
    public string EmailAcademia { get; set; } = string.Empty;

    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}
