namespace SistemaAcademia.Models;

public class CadastroClienteViewModel
{
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public int IdOrientacaoSexual { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string TelefoneOpcional { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;

    // matrícula opcional — preenchida na mesma tela de cadastro
    public int?      IdModalidade               { get; set; }
    public int?      IdPlano                    { get; set; }
    public DateTime? DataInicioMatricula         { get; set; }
    public bool      PrimeiroPagoNoAto           { get; set; }
    public int?      IdMetodoPagamentoPrimeiro   { get; set; }
}

public class ClienteListaViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string Ativo { get; set; } = string.Empty;
}

public class EditarClienteViewModel
{
    public int Id { get; set; }
    public string CPF { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public int IdOrientacaoSexual { get; set; }
    public string CEP { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}