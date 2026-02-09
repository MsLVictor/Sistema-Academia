using SistemaAcademia.Models;

Usuario usuario = new Usuario("Victor", "Melquiades Leite", "09769920436", "victormleite25@icloud.com", "12345678*", SistemaAcademia.Enum.CargosEnum.Recepcionista);

System.Console.WriteLine($"Professor {usuario.Nome} cadastrado com sucesso!");
