namespace SistemaAcademia.Models;

public class FuncionarioViewModel
{
    public long   Id     { get; set; }
    public string Nome   { get; set; }
    public string CPF    { get; set; }
    public string Email  { get; set; }
    public string Cargo  { get; set; }
    public long   IdCargo { get; set; }
    public string Ativo  { get; set; }
}

public class EditarFuncionarioViewModel
{
    public long   Id                { get; set; }
    public string NomeFuncionario   { get; set; }
    public string EmailFuncionario  { get; set; }
    public long   IdCargo           { get; set; }
}
