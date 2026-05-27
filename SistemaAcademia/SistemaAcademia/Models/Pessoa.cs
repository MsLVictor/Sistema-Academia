namespace SistemaAcademia.Models;

public abstract class Pessoa
{
    public Pessoa(string nome, string cpf, string email, string senha, DateTime dataNascimento)
    {
        Nome = nome;
        DataNascimento = dataNascimento;
        CPF = cpf;
        Email = email;
        Senha = senha;
    }

    public long     Id             { get; init; }
    public string   Nome           { get; }
    public DateTime DataNascimento { get; }
    public string   CPF            { get; }
    public string   Email          { get; private set; }
    public string   Senha          { get; private set; }
    public bool     SituacaoCadastro { get; private set; } = true;

    public void DesativarCadastro() => SituacaoCadastro = false;
    public void AtivarCadastro() => SituacaoCadastro = true;

    public void AtualizarEmail(string novoEmail)
    {
        if (!string.IsNullOrEmpty(novoEmail) && novoEmail.Contains("@"))
            Email = novoEmail;
    }

    public void AtualizarSenha(string novaSenha)
    {
        if (novaSenha.Length >= 6)
            Senha = novaSenha;
    }
}