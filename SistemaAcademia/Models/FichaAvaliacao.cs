namespace SistemaAcademia.Models;

public class FichaAvaliacao
{
    public FichaAvaliacao(int idCliente, int idProfessor, double peso, double altura, double torax, double cintura, double coxa, double panturrilha, double biceps)
    {
        IdCliente = idCliente;
        IdProfessor = idProfessor;
        Peso = peso;
        Altura = altura;
        Torax = torax;
        Cintura = cintura;
        Coxa = coxa;
        Panturrilha = panturrilha;
        Biceps = biceps;
        Id++;
    }
    public int Id { get; } = 0;
    public DateTime DataAvaliacao = DateTime.Now;
    public int IdCliente {get;private set;}
    public int IdProfessor {get;private set;}
    public double Peso {get;private set;}
    public double Altura {get;private set;}
    public double Torax {get;private set;}
    public double Cintura {get;private set;}
    public double Coxa {get;private set;}
    public double Panturrilha { get; private set;}
    public double Biceps {get; private set;}

}
