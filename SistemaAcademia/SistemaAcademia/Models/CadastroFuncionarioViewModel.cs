namespace SistemaAcademia.Models;

public class CadastroFuncionarioViewModel
{
    public string NomeFuncionario { get; set; } = string.Empty;
    public string CPFFuncionario { get; set; } = string.Empty;
    public string EmailFuncionario { get; set; } = string.Empty;
    public string SenhaFuncionario { get; set; } = string.Empty;
    public int IdCargo { get; set; }
}
