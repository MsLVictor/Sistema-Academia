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

    public string Nome { get; private set; }
    public DateTime DataNascimento { get; private set; }
    public string CPF { get; private set; }
    public string Email { get; private set; }
    public string Senha { get; private set; }
    public bool SituacaoCadastro { get; private set; } = true;
    public void DesativarCadastro() => SituacaoCadastro = false;
    
    public void AtivarCadastro() => SituacaoCadastro = true;


    public void AtualizarEmail(string novoEmail)
    {
        if (!string.IsNullOrEmpty(novoEmail) && novoEmail.Contains("@"))
        {
            Email = novoEmail;
            Console.WriteLine("Email atualizado com sucesso!");
        }
        else
            System.Console.WriteLine("Email inválido.");

    }
    public void AtualizarSenha(string novaSenha)
    {
        if (novaSenha.Length >= 6)
        {
            Senha = novaSenha;
            System.Console.WriteLine("Senha atualizada com sucesso!");
        }
        else
            System.Console.WriteLine("Senha inválida, Coloque uma senha com 6 dígitos ou mais.");
    }
}

