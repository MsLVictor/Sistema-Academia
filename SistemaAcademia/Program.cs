using Microsoft.Data.SqlClient;
using SistemaAcademia.Enum;
using SistemaAcademia.Models;
using SistemaAcademia.Repositories;

bool menuInfinito = true;

while (menuInfinito)
{
    int opcaoMenu;

    System.Console.WriteLine(" 1 - Cadastrar Usuário.");
    System.Console.WriteLine(" 2 - Listar Usuários.");
    System.Console.WriteLine(" 0 - sair");
    while (!int.TryParse(Console.ReadLine(), out opcaoMenu))
        System.Console.WriteLine("Opção errada.");

    switch (opcaoMenu)
    {
        case 1:
            CriarUsuario();
            break;

        case 2:
            ListarUsuarios();
            break;

        case 0:
            menuInfinito = false;
            break;

    }
}


static void ListarUsuarios()
{
    var repositorio = new UsuarioRepository();

    Console.WriteLine("=== LISTA DE USUÁRIOS NO BANCO ===");
    Console.WriteLine("------------------------------------------------------------");
    Console.WriteLine("{0,-5} | {1,-20} | {2,-15} | {3,-15}", "ID", "NOME", "CPF", "CARGO");
    Console.WriteLine("------------------------------------------------------------");

    var usuarios = repositorio.ListarTodos();

    foreach (var usuario in usuarios)
    {
        Console.WriteLine("{0,-5} | {1,-20} | {2,-15} | {3,-15}",
            usuario.Id,
            usuario.Nome,
            usuario.Cpf,
            usuario.CargoEnum);
    }

    Console.WriteLine("------------------------------------------------------------");
    Console.WriteLine($"Total de usuários: {usuarios.Count}");
    System.Console.WriteLine("Pressione qualquer tecla para voltar pro menu anterior");
    Console.ReadKey();
}

static void CriarUsuario()
{
    System.Console.WriteLine("=== CADASTRO DE USUÁRIO ===");

    System.Console.WriteLine("Nome: ");
    string nome = Console.ReadLine();

    System.Console.WriteLine("CPF: ");
    string cpf = Console.ReadLine();

    System.Console.WriteLine("Email:");
    string email = Console.ReadLine();

    Console.WriteLine("Selecione o Cargo:");
    Console.WriteLine("1 - Administrador");
    Console.WriteLine("2 - Instrutor");
    Console.WriteLine("3 - Recepcionista");
    Console.Write("Opção: ");
    int opcaoCargo;

    while (!int.TryParse(Console.ReadLine(), out opcaoCargo) || opcaoCargo < 1 || opcaoCargo > 3)
        System.Console.WriteLine("Opcão inválida.");

    CargosEnum cargoEscolhido = opcaoCargo switch
    {
        1 => CargosEnum.Admin,
        2 => CargosEnum.Professor,
        3 => CargosEnum.Recepcionista,
        _ => CargosEnum.Professor
    };

    Usuario usuario = new Usuario(nome, cpf, email, cargoEscolhido);
    
    var repositorio = new UsuarioRepository();

    repositorio.Adicionar(usuario);

    System.Console.WriteLine("\n[SUCESSO] Usuário cadastrado com sucesso!");
}
