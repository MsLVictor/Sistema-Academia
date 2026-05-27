# SPEC — Sistema Academia

**Base:** PRD v2026-05-19
**Abordagem:** Spec Driven Development — este arquivo é a fonte da verdade para implementação.
**Última atualização:** 2026-05-19

---

## 0. Convenções Gerais

### Arquitetura
```
Controller → Service → Repository → SQL Server
```
- Controllers não acessam banco diretamente.
- Services contêm regras de negócio.
- Repositories contêm apenas SQL (ADO.NET, sem ORM).
- Não há injeção de dependência via container — instanciar diretamente nos controllers por enquanto.

### Sessão (HttpContext.Session)

| Chave | Tipo | Preenchida em | Descrição |
|---|---|---|---|
| `UsuarioNome` | string | Login de funcionário | Nome completo |
| `UsuarioEmail` | string | Login de funcionário | E-mail |
| `UsuarioCargo` | string | Login de funcionário | Nome do cargo ex: "Administrador" |
| `UsuarioId` | string | Login de funcionário | Id do Usuario |
| `UsuarioIdAcademia` | string | Login de funcionário | Id da Academia |
| `ClienteNome` | string | Login de cliente | Nome completo |
| `ClienteEmail` | string | Login de cliente | E-mail |
| `ClienteId` | string | Login de cliente | Id do Cliente |
| `ClienteIdAcademia` | string | Login de cliente | Id da Academia |
| `TipoSessao` | string | Qualquer login | `"Funcionario"` ou `"Cliente"` |

### Autorização (padrão atual — manual por action)
```csharp
if (HttpContext.Session.GetString("UsuarioCargo") != "Administrador")
    return RedirectToAction("Index", "Login");
```
Aplicar esse padrão até a implementação do middleware centralizado.

### Connection string
```csharp
Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? @"Data Source=...\SQLEXPRESS;Initial Catalog=SistemaAcademia;..."
```
Todos os repositories devem seguir esse padrão.

### Tratamento de erros padrão
- Violação de UNIQUE (CPF, CNPJ, Email): capturar `SqlException` com `Number == 2627` e retornar mensagem amigável.
- Violação de FK: `SqlException` com `Number == 547`.
- Erro genérico: logar e retornar mensagem genérica.

### Hash de senha
```csharp
byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(senha.Trim()));
// Armazenar como VARBINARY(32) no banco
```

### Limpeza de CPF/CNPJ antes de persistir
```csharp
string cpf = new string(valor.Where(char.IsDigit).ToArray()); // 11 dígitos
string cnpj = new string(valor.Where(char.IsDigit).ToArray()); // 14 dígitos
```

---

> **Responsividade:** todas as views são desktop-first por ora. Ajuste mobile será feito via Claude Design após a implementação funcional estar completa.

## Módulo 1 — Autenticação

### 1.1 Login de Funcionário ✅ Implementado

**Rota:** `POST /Login/Logar`
**Controller:** `LoginController.Logar(string email, string senha)`
**Autorização:** Anônimo

**Input:**

| Campo | Tipo | Validação |
|---|---|---|
| email | string | Obrigatório, formato e-mail |
| senha | string | Obrigatório |

**Fluxo:**
1. Fazer hash SHA-256 da senha.
2. Buscar `Usuario` por email + hash no banco (JOIN com `Cargo`).
3. Se encontrado: gravar sessão (`UsuarioNome`, `UsuarioEmail`, `UsuarioCargo`, `UsuarioId`, `UsuarioIdAcademia`, `TipoSessao = "Funcionario"`).
4. Redirecionar conforme cargo:
   - `Administrador` → `Home/Administrador`
   - `Gerente` → `Home/Gerente`
   - `Recepcionista` → `Home/Recepcionista`
   - `Instrutor` → `Home/Instrutor`
   - Qualquer outro → `Home/Administrador`

**Erro:** Credenciais inválidas → `ViewBag.Erro = "Usuário ou senha inválidos!"` e retorna a view.

**SQL (UsuarioRepository.ValidarLogin):**
```sql
SELECT us.Id, us.Nome, us.Email, us.IdAcademia, ca.Nome AS Cargo
FROM Usuario us
    INNER JOIN Cargo ca ON us.IdCargo = ca.Id
WHERE us.Email = @email AND us.Senha = @senha
```

**Pendência:** a sessão ainda não grava `UsuarioId` — adicionar `HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString())`.

---

### 1.2 Login de Cliente ❌ Pendente

**Rota:** `POST /Login/LogarCliente`
**Controller:** `LoginController.LogarCliente(string email, string senha)`
**Autorização:** Anônimo

**Input:**

| Campo | Tipo | Validação |
|---|---|---|
| email | string | Obrigatório, formato e-mail |
| senha | string | Obrigatório |

**Fluxo:**
1. Fazer hash SHA-256 da senha.
2. Buscar `Cliente` por email + hash.
3. Se encontrado: gravar sessão (`ClienteNome`, `ClienteEmail`, `ClienteId`, `ClienteIdAcademia`, `TipoSessao = "Cliente"`).
4. Redirecionar para `Home/Cliente`.

**Erro:** Credenciais inválidas → `ViewBag.ErroCliente = "Usuário ou senha inválidos!"` e retorna a view.

**SQL (ClienteRepository.ValidarLogin):**
```sql
SELECT Id, Nome, Email, IdAcademia
FROM Cliente
WHERE Email = @email AND Senha = @senha
```

---

### 1.3 Logout ✅ Implementado

**Rota:** `GET /Login/Sair`
**Fluxo:** `HttpContext.Session.Clear()` → redireciona para `Login/Index`.

---

### 1.4 Recuperação de Senha ❌ Pendente (fora do escopo atual)

Documentar apenas a intenção: envio de e-mail com link temporário. Implementar em versão futura.

---

## Módulo 2 — Onboarding de Academia

### 2.1 Wizard de Cadastro ✅ Implementado

**Rota:** `POST /Login/CadastrarAcademia`
**Controller:** `LoginController.CadastrarAcademia(CadastroAcademiaViewModel dados)`
**Autorização:** Anônimo

**ViewModel (CadastroAcademiaViewModel):**

| Campo | Tabela | Validação |
|---|---|---|
| NomeAdministrador | Usuario.Nome | Obrigatório |
| CPFAdministrador | Usuario.CPF | Obrigatório, 11 dígitos, único |
| EmailAdministrador | Usuario.Email | Obrigatório, formato, único |
| SenhaAdministrador | Usuario.Senha | Obrigatório, mín. 6 chars |
| NomeAcademia | Academia.Nome | Obrigatório |
| CNPJAcademia | Academia.CNPJ | Obrigatório, 14 dígitos, único |
| EmailAcademia | Academia.Email | Obrigatório, formato |
| Logradouro | Endereco.Logradouro | Obrigatório |
| Numero | Endereco.Numero | Obrigatório |
| Complemento | Endereco.Complemento | Opcional |
| Bairro | Endereco.Bairro | Obrigatório |
| Cidade | Endereco.Cidade | Obrigatório |
| Estado | Endereco.Estado | Obrigatório, 2 chars |
| CEP | Endereco.CEP | Obrigatório, 8 dígitos |

**Fluxo (transação):**
1. INSERT em `Endereco` → captura Id gerado.
2. INSERT em `Academia` com IdEndereco → captura Id gerado.
3. Buscar Id do Cargo "Administrador" em `Cargo`.
4. INSERT em `Usuario` com IdAcademia e IdCargo de Administrador.
5. Commit. Em falha: rollback completo.

**Sucesso:** `TempData["SucessoCadastro"]` → redirect para `Login/Index`.
**Erro:** `ViewBag.ErroCadastro = "Erro ao cadastrar. Verifique se o CNPJ ou CPF já está em uso."` → retorna view.

---

## Módulo 3 — Gestão de Funcionários

### 3.1 Cadastrar Funcionário ✅ Implementado

**Rota:** `POST /Funcionario/Cadastrar`
**Controller:** `FuncionarioController.Cadastrar(CadastroFuncionarioViewModel dados)`
**Autorização:** `UsuarioCargo == "Administrador"`

**Input (CadastroFuncionarioViewModel):**

| Campo | Validação |
|---|---|
| NomeFuncionario | Obrigatório |
| CPFFuncionario | Obrigatório, 11 dígitos, único |
| EmailFuncionario | Obrigatório, formato, único |
| SenhaFuncionario | Obrigatório, mín. 6 chars |
| IdCargo | Obrigatório, deve existir em Cargo |

**Fluxo:**
1. Limpar CPF (apenas dígitos).
2. Hash SHA-256 da senha.
3. Ler `UsuarioIdAcademia` da sessão.
4. INSERT em `Usuario`.

**Sucesso:** `TempData["SucessoFuncionario"]` → redirect para `Home/Administrador`.
**Erro CPF/Email duplicado:** `TempData["ErroFuncionario"] = "CPF ou e-mail já cadastrado."`.

**SQL:**
```sql
INSERT INTO Usuario (IdAcademia, IdCargo, Nome, CPF, Email, Senha)
VALUES (@idAcademia, @idCargo, @nome, @cpf, @email, @senha)
```

---

### 3.2 Listar Funcionários ❌ Pendente

**Rota:** `GET /Funcionario/Listar`
**Controller:** `FuncionarioController.Listar()`
**Autorização:** `UsuarioCargo IN ("Administrador", "Gerente")`

**Retorno:** `IEnumerable<UsuarioViewModel>` para a view.

**SQL (UsuarioRepository.ListarPorAcademia):**
```sql
SELECT us.Id, us.Nome, us.CPF, us.Email, ca.Nome AS Cargo
FROM Usuario us
    INNER JOIN Cargo ca ON us.IdCargo = ca.Id
WHERE us.IdAcademia = @idAcademia
ORDER BY us.Nome
```

---

### 3.3 Editar Funcionário ❌ Pendente

**Rotas:**
- `GET /Funcionario/Editar/{id}` → exibe formulário preenchido
- `POST /Funcionario/Editar/{id}` → salva alterações

**Autorização:** `UsuarioCargo == "Administrador"`

**Input (EditarFuncionarioViewModel):**

| Campo | Validação |
|---|---|
| Id | Obrigatório |
| NomeFuncionario | Obrigatório |
| EmailFuncionario | Obrigatório, formato, único (exceto o próprio) |
| IdCargo | Obrigatório |

**Regras:**
- Não permitir alterar CPF.
- Não exibir/alterar senha neste formulário (fluxo separado de troca de senha).
- Validar que o funcionário pertence à academia da sessão.

**SQL (UsuarioRepository.Atualizar):**
```sql
UPDATE Usuario
SET Nome = @nome, Email = @email, IdCargo = @idCargo
WHERE Id = @id AND IdAcademia = @idAcademia
```

---

### 3.4 Inativar / Reativar Funcionário ❌ Pendente

**Rota:** `POST /Funcionario/AlterarStatus/{id}`
**Autorização:** `UsuarioCargo == "Administrador"`

**Regra:** Não permitir inativar o próprio usuário logado.

**SQL (UsuarioRepository.AlterarStatus):**
```sql
-- A tabela Usuario não tem campo Ativo — adicionar na migration ou adaptar a regra
-- Por ora, sugestão: adicionar coluna Ativo CHAR(1) DEFAULT 'A' à tabela Usuario
UPDATE Usuario SET Ativo = @ativo WHERE Id = @id AND IdAcademia = @idAcademia
```

> **Atenção:** A tabela `Usuario` no script atual não possui coluna `Ativo`. Criar migration para adicionar antes de implementar.

---

## Módulo 4 — Gestão de Clientes

### 4.1 Cadastrar Cliente ❌ Pendente

**Rota:** `POST /Cliente/Cadastrar`
**Controller:** `ClienteController.Cadastrar(CadastroClienteViewModel dados)`
**Autorização:** `UsuarioCargo IN ("Administrador", "Recepcionista")`

**Input (CadastroClienteViewModel):**

| Campo | Tabela | Validação |
|---|---|---|
| Nome | Cliente.Nome | Obrigatório |
| CPF | Cliente.CPF | Obrigatório, 11 dígitos, único |
| DataNascimento | Cliente.DataNascimento | Obrigatório, não futura |
| Email | Cliente.Email | Obrigatório, formato, único |
| Senha | Cliente.Senha | Obrigatório, mín. 6 chars |
| IdOrientacaoSexual | Cliente.IdOrientacaoSexual | Obrigatório, deve existir |
| Telefone | Telefone.Telefone | Obrigatório |
| TelefoneOpcional | Telefone.Telefone | Opcional |
| CEP | Endereco.CEP | Obrigatório, 8 dígitos |
| Logradouro | Endereco.Logradouro | Obrigatório |
| Numero | Endereco.Numero | Obrigatório |
| Complemento | Endereco.Complemento | Opcional |
| Bairro | Endereco.Bairro | Obrigatório |
| Cidade | Endereco.Cidade | Obrigatório |
| Estado | Endereco.Estado | Obrigatório, 2 chars |

**UI — campos especiais:**
- `IdOrientacaoSexual` → `<select>` carregado via `GET /OrientacaoSexual/Listar` (ou via `ViewBag` preenchido pelo controller antes de renderizar o form). Criar `OrientacaoSexualRepository.BuscarTodos()`.
- `Estado` → `<select>` com os 27 estados brasileiros (mesmo padrão do wizard de academia).
- `CEP` → integração com ViaCEP (mesmo padrão do wizard): preenche automaticamente Logradouro, Bairro, Cidade e Estado ao sair do campo.

**Fluxo (transação):**
1. INSERT em `Endereco` → captura Id gerado.
2. Hash SHA-256 da senha.
3. INSERT em `Cliente` com IdAcademia (da sessão), IdEndereco e IdUsuario = Id do funcionário logado.
4. INSERT em `Telefone` (principal e, se preenchido, opcional).
5. Disparar e-mail de boas-vindas (async, não bloquear o fluxo).

**Sucesso:** Redirect para `Cliente/Listar` com `TempData["Sucesso"]`.
**Erro CPF/Email duplicado:** Retornar view com mensagem.

**SQL (ClienteRepository.Cadastrar):**
```sql
-- Passo 1
INSERT INTO Endereco (Logradouro, Numero, Complemento, Bairro, Cidade, Estado, CEP)
VALUES (@logradouro, @numero, @complemento, @bairro, @cidade, @estado, @cep);
SELECT SCOPE_IDENTITY();

-- Passo 2
INSERT INTO Cliente (IdAcademia, IdEndereco, IdUsuario, IdOrientacaoSexual, Nome, CPF, DataNascimento, Email, Senha)
VALUES (@idAcademia, @idEndereco, @idUsuario, @idOrientacaoSexual, @nome, @cpf, @dataNascimento, @email, @senha);
SELECT SCOPE_IDENTITY();

-- Passo 3
INSERT INTO Telefone (IdCliente, Telefone) VALUES (@idCliente, @telefone);
```

---

### 4.2 Listar Clientes ❌ Pendente

**Rota:** `GET /Cliente/Listar`
**Controller:** `ClienteController.Listar(string? busca, string? status)`
**Autorização:** `UsuarioCargo IN ("Administrador", "Gerente", "Recepcionista")`

**Parâmetros de busca (query string):**

| Parâmetro | Descrição |
|---|---|
| busca | Filtra por Nome ou CPF (LIKE) |
| status | `"A"` = ativos, `"I"` = inativos, vazio = todos |

**SQL (ClienteRepository.Listar):**
```sql
SELECT cl.Id, cl.Nome, cl.CPF, cl.Email, cl.DataNascimento,
       te.Telefone,
       mc.Id AS IdMatricula, mc.StatusSituacao,
       mo.Nome AS Modalidade
FROM Cliente cl
    LEFT JOIN Telefone te ON te.IdCliente = cl.Id
    LEFT JOIN MatriculaCliente mc ON mc.IdCliente = cl.Id AND mc.StatusSituacao = 'A'
    LEFT JOIN Modalidade mo ON mo.Id = mc.IdModalidade
WHERE cl.IdAcademia = @idAcademia
  AND (@busca IS NULL OR cl.Nome LIKE '%' + @busca + '%' OR cl.CPF LIKE '%' + @busca + '%')
ORDER BY cl.Nome
```

---

### 4.3 Editar Cliente ❌ Pendente

**Rotas:**
- `GET /Cliente/Editar/{id}` → formulário preenchido
- `POST /Cliente/Editar/{id}` → salva

**Autorização:** `UsuarioCargo IN ("Administrador", "Recepcionista")`

**Regras:**
- Não permitir alterar CPF.
- Validar que o cliente pertence à academia da sessão.
- Atualizar endereço (UPDATE em Endereco pelo IdEndereco do cliente).

**SQL (ClienteRepository.Atualizar):**
```sql
UPDATE Cliente
SET Nome = @nome, Email = @email, DataNascimento = @dataNascimento,
    IdOrientacaoSexual = @idOrientacaoSexual
WHERE Id = @id AND IdAcademia = @idAcademia;

UPDATE Endereco
SET Logradouro = @logradouro, Numero = @numero, Complemento = @complemento,
    Bairro = @bairro, Cidade = @cidade, Estado = @estado, CEP = @cep
WHERE Id = (SELECT IdEndereco FROM Cliente WHERE Id = @id);
```

---

### 4.4 Inativar / Reativar Cliente ❌ Pendente

**Rota:** `POST /Cliente/AlterarStatus/{id}`
**Autorização:** `UsuarioCargo IN ("Administrador", "Recepcionista")`

> **Atenção:** Assim como `Usuario`, a tabela `Cliente` não possui coluna `Ativo`. Criar migration para adicionar `Ativo CHAR(1) DEFAULT 'A'`.

**Regra:** Ao inativar cliente, cancelar todas as matrículas ativas (`StatusSituacao = 'I'`).

---

### 4.5 Gerar QR Code do Cliente ❌ Pendente

**Rota:** `GET /Cliente/QrCode/{id}`
**Autorização:** `UsuarioCargo IN ("Administrador", "Recepcionista")` ou `ClienteId == id`

**Fluxo:**
- Gerar QR Code contendo o `ClienteId` como payload.
- Usar biblioteca `QRCoder` (NuGet) para gerar PNG.
- Retornar como `FileContentResult` (`image/png`).

**Segurança:** O QR Code deve conter apenas o Id do cliente — o servidor valida a academia no check-in.

---

## Módulo 5 — Check-in via QR Code

### 5.1 Registrar Entrada ❌ Pendente

**Rota:** `POST /CheckIn/Entrada`
**Controller:** `CheckInController.Entrada(int idCliente)`
**Autorização:** `UsuarioCargo == "Recepcionista"`

**Fluxo:**
1. Validar que `idCliente` pertence à academia da sessão.
2. Verificar se o cliente tem matrícula ativa (`StatusSituacao = 'A'`).
3. Verificar que não existe um check-in aberto (sem `DataHoraSaida`) para o cliente.
4. INSERT em `CheckIn`.

**Erro — matrícula inativa:** Retornar erro `"Aluno sem matrícula ativa."`.
**Erro — já está na academia:** Retornar erro `"Aluno já registrou entrada."`.

**SQL (CheckInRepository.RegistrarEntrada):**
```sql
INSERT INTO CheckIn (IdCliente, IdAcademia, IdUsuarioRegistro, DataHoraEntrada)
VALUES (@idCliente, @idAcademia, @idUsuario, GETDATE())
```

---

### 5.2 Registrar Saída ✅ Implementado

**Rota:** `POST /CheckIn/Saida/{idCheckIn}`
**Autorização:** `UsuarioCargo == "Recepcionista"`

**SQL (CheckInRepository.RegistrarSaida):**
```sql
UPDATE CheckIn
SET DataHoraSaida = GETDATE()
WHERE Id = @idCheckIn AND IdAcademia = @idAcademia AND DataHoraSaida IS NULL
```

---

### 5.3 Histórico de Check-ins ✅ Implementado

**Rota:** `GET /CheckIn/Historico/{idCliente}`
**Autorização:** `UsuarioCargo IN ("Administrador", "Gerente", "Recepcionista", "Instrutor")`

**SQL:**
```sql
SELECT ci.Id, ci.DataHoraEntrada, ci.DataHoraSaida,
       us.Nome AS Recepcionista
FROM CheckIn ci
    INNER JOIN Usuario us ON us.Id = ci.IdUsuarioRegistro
WHERE ci.IdCliente = @idCliente AND ci.IdAcademia = @idAcademia
ORDER BY ci.DataHoraEntrada DESC
```

---

## Módulo 6 — Planos e Modalidades

### 6.1 CRUD de Planos ✅ Implementado

**Rotas:**
- `GET /Plano/Listar` — lista planos da academia
- `POST /Plano/Cadastrar` — cria novo plano
- `POST /Plano/Editar/{id}` — atualiza
- `POST /Plano/Excluir/{id}` — exclui (somente se sem matrículas vinculadas)

**Autorização:** `UsuarioCargo IN ("Administrador", "Gerente")`

**Input (PlanoViewModel):**

| Campo | Tipo | Validação |
|---|---|---|
| Nome | string | Obrigatório |
| TempoPlano | CHAR(2) | Obrigatório, ex: `"01"` a `"12"` (meses) |
| PercentualDesconto | float | Obrigatório, 0 a 100 |

**SQL (PlanoRepository.Cadastrar):**
```sql
INSERT INTO Plano (IdAcademia, Nome, TempoPlano, PercentualDesconto)
VALUES (@idAcademia, @nome, @tempoPLano, @percentualDesconto)
```

---

### 6.2 CRUD de Modalidades ✅ Implementado

**Rotas:**
- `GET /Modalidade/Listar`
- `POST /Modalidade/Cadastrar`
- `POST /Modalidade/Editar/{id}`
- `POST /Modalidade/Excluir/{id}` — somente se sem matrículas vinculadas

**Autorização:** `UsuarioCargo IN ("Administrador", "Gerente")`

**Input:**

| Campo | Validação |
|---|---|
| Nome | Obrigatório |
| ValorModalidade | Obrigatório, > 0, decimal |

**SQL:**
```sql
INSERT INTO Modalidade (IdAcademia, Nome, ValorModalidade)
VALUES (@idAcademia, @nome, @valorModalidade)
```

---

## Módulo 7 — Matrículas

### 7.1 Matricular Aluno ✅ Implementado

**Rota:** `POST /Matricula/Cadastrar`
**Controller:** `MatriculaController.Cadastrar(MatriculaViewModel dados)`
**Autorização:** `UsuarioCargo == "Recepcionista"`

**Input:**

| Campo | Validação |
|---|---|
| IdCliente | Obrigatório, deve pertencer à academia |
| IdModalidade | Obrigatório, deve pertencer à academia |
| IdPlano | Obrigatório, deve pertencer à academia |
| DataInicio | Obrigatório, não retroativa |

**Fluxo (transação):**
1. Validar que o cliente não possui matrícula ativa.
2. INSERT em `MatriculaCliente` com `StatusSituacao = 'A'`.
3. Capturar Id da matrícula gerada.
4. Chamar `GerarMensalidades(idMatricula, dataInicio, tempoPlano, valorModalidade, percentualDesconto)`.

**Regra de geração de mensalidades:**
- Quantidade de parcelas = `TempoPlano` (em meses, valor numérico da coluna CHAR(2)).
- Valor de cada parcela = `ValorModalidade * (1 - PercentualDesconto / 100)`.
- `DataVencimento` = `DataInicio.AddMonths(i)` para i de 0 até quantidade-1.
- `IdStatusPagamento` = Id correspondente a "Pendente" em `StatusPagamento`.
- `IdMetodoPagamento` = não definido na geração, deixar nulo (ou criar campo nullable).

> **Atenção:** `Pagamento.IdMetodoPagamento` é `NOT NULL` no script atual. Avaliar tornar nullable ou usar um valor padrão "Não definido" na tabela `MetodoPagamento`.

**SQL — INSERT matrícula:**
```sql
INSERT INTO MatriculaCliente (IdCliente, IdModalidade, IdPlano, DataInicio, StatusSituacao)
VALUES (@idCliente, @idModalidade, @idPlano, @dataInicio, 'A');
SELECT SCOPE_IDENTITY();
```

**SQL — INSERT mensalidades (em loop):**
```sql
INSERT INTO Pagamento (IdMatriculaCliente, IdMetodoPagamento, IdStatusPagamento, ValorPago, DataVencimento)
VALUES (@idMatricula, @idMetodoPagamento, @idStatusPendente, @valor, @dataVencimento)
```

---

### 7.2 Cancelar / Suspender Matrícula ✅ Implementado

**Rota:** `POST /Matricula/Cancelar/{id}`
**Autorização:** `UsuarioCargo IN ("Administrador", "Recepcionista")`

**Fluxo:**
1. UPDATE `MatriculaCliente.StatusSituacao = 'I'`.
2. Cancelar pagamentos futuros (`DataVencimento > GETDATE()` e status Pendente → Cancelado).

**SQL:**
```sql
UPDATE MatriculaCliente SET StatusSituacao = 'I' WHERE Id = @id AND IdAcademia via JOIN;

UPDATE Pagamento
SET IdStatusPagamento = (SELECT Id FROM StatusPagamento WHERE Nome = 'Cancelado')
WHERE IdMatriculaCliente = @id
  AND DataVencimento > CAST(GETDATE() AS DATE)
  AND IdStatusPagamento = (SELECT Id FROM StatusPagamento WHERE Nome = 'Pendente')
```

---

## Módulo 8 — Pagamentos

### 8.1 Listar Mensalidades de um Aluno ✅ Implementado

**Rota:** `GET /Pagamento/Listar/{idCliente}`
**Autorização:** `UsuarioCargo IN ("Administrador", "Gerente", "Recepcionista")`

**SQL:**
```sql
SELECT pg.Id, pg.ValorPago, pg.DataVencimento, pg.DataPagamento,
       sp.Nome AS Status, mp.Nome AS MetodoPagamento
FROM Pagamento pg
    INNER JOIN MatriculaCliente mc ON mc.Id = pg.IdMatriculaCliente
    INNER JOIN StatusPagamento sp ON sp.Id = pg.IdStatusPagamento
    LEFT JOIN MetodoPagamento mp ON mp.Id = pg.IdMetodoPagamento
WHERE mc.IdCliente = @idCliente
ORDER BY pg.DataVencimento DESC
```

---

### 8.2 Registrar Pagamento ✅ Implementado (manual)

**Rota:** `POST /Pagamento/Registrar`
**Autorização:** `UsuarioCargo == "Recepcionista"`

**Input:**

| Campo | Validação |
|---|---|
| IdPagamento | Obrigatório |
| IdMetodoPagamento | Obrigatório |
| ValorPago | Obrigatório, > 0 |

**Fluxo:**
1. UPDATE em `Pagamento`: definir `IdMetodoPagamento`, `ValorPago`, `DataPagamento = GETDATE()`.
2. Atualizar `IdStatusPagamento` para "Pago".
3. Verificar se parcelas vencidas ainda em Pendente → marcar como Atrasado (job ou trigger futuro).

**SQL:**
```sql
UPDATE Pagamento
SET IdMetodoPagamento = @idMetodo,
    ValorPago = @valorPago,
    DataPagamento = CAST(GETDATE() AS DATE),
    IdStatusPagamento = (SELECT Id FROM StatusPagamento WHERE Nome = 'Pago')
WHERE Id = @id
```

---

### 8.3 Marcar Vencidos como Atrasado ✅ Implementado (ação manual do Administrador)

**Rota:** `POST /Pagamento/AtualizarVencidos` (chamada via job agendado ou ação manual)
**Autorização:** Apenas sistema ou `UsuarioCargo == "Administrador"`

**SQL:**
```sql
UPDATE Pagamento
SET IdStatusPagamento = (SELECT Id FROM StatusPagamento WHERE Nome = 'Atrasado')
WHERE DataVencimento < CAST(GETDATE() AS DATE)
  AND IdStatusPagamento = (SELECT Id FROM StatusPagamento WHERE Nome = 'Pendente')
  AND IdMatriculaCliente IN (
      SELECT Id FROM MatriculaCliente mc
      INNER JOIN Cliente cl ON cl.Id = mc.IdCliente
      WHERE cl.IdAcademia = @idAcademia
  )
```

---

## Módulo 9 — Avaliação Física

### 9.1 Cadastrar Avaliação ❌ Pendente

**Rota:** `POST /AvaliacaoFisica/Cadastrar`
**Controller:** `AvaliacaoFisicaController.Cadastrar(AvaliacaoFisicaViewModel dados)`
**Autorização:** `UsuarioCargo == "Instrutor"`

**Input:**

| Campo | Tipo | Validação |
|---|---|---|
| IdCliente | int | Obrigatório, deve pertencer à academia |
| DataAvaliacao | date | Obrigatório, não futura |
| Peso | float | Opcional |
| Altura | float | Opcional |
| PorcentagemGorduraCorporal | float | Opcional, 0-100 |
| PorcentagemMassaMagra | float | Opcional, 0-100 |
| Torax | float | Opcional |
| Cintura | float | Opcional |
| Coxa | float | Opcional |
| Panturrilha | float | Opcional |
| Biceps | float | Opcional |

**SQL:**
```sql
INSERT INTO AvaliacaoFisica
    (IdCliente, Peso, PorcentagemGorduraCorporal, PorcentagemMassaMagra,
     Altura, Torax, Cintura, Coxa, Panturrilha, Biceps, DataAvaliacao)
VALUES
    (@idCliente, @peso, @pctGordura, @pctMassaMagra,
     @altura, @torax, @cintura, @coxa, @panturrilha, @biceps, @dataAvaliacao)
```

---

### 9.2 Listar Avaliações do Aluno ❌ Pendente

**Rota:** `GET /AvaliacaoFisica/Historico/{idCliente}`
**Autorização:** `UsuarioCargo == "Instrutor"` ou `ClienteId == idCliente`

**SQL:**
```sql
SELECT * FROM AvaliacaoFisica
WHERE IdCliente = @idCliente
ORDER BY DataAvaliacao DESC
```

---

## Módulo 10 — Treinos

### 10.1 CRUD de Exercícios ❌ Pendente

**Rotas:**
- `GET /Exercicio/Listar` — catálogo completo (sem filtro de academia — exercícios são globais)
- `POST /Exercicio/Cadastrar`
- `POST /Exercicio/Editar/{id}`

**Autorização:** `UsuarioCargo == "Instrutor"`

**Input:**

| Campo | Validação |
|---|---|
| Nome | Obrigatório |
| GrupoMuscular | Obrigatório (ex: Peito, Costas, Perna, Ombro, Bíceps, Tríceps, Core) |
| Descricao | Opcional |

**SQL:**
```sql
INSERT INTO Exercicio (Nome, GrupoMuscular, Descricao)
VALUES (@nome, @grupoMuscular, @descricao)
```

---

### 10.2 Criar Ficha de Treino ❌ Pendente

**Rota:** `POST /Treino/Criar`
**Autorização:** `UsuarioCargo == "Instrutor"`

**Input (TreinoViewModel):**

| Campo | Validação |
|---|---|
| IdCliente | Obrigatório, pertence à academia |
| Nome | Obrigatório |
| Observacao | Opcional |
| Exercicios | Lista de TreinoExercicioViewModel, mínimo 1 |

**TreinoExercicioViewModel:**

| Campo | Validação |
|---|---|
| IdExercicio | Obrigatório |
| Series | Obrigatório, > 0 |
| Repeticoes | Obrigatório, > 0 |
| CargaKg | Opcional |
| Observacao | Opcional |
| Ordem | Obrigatório, > 0, único por treino |

**Fluxo (transação):**
1. INSERT em `Treino` → captura Id.
2. Para cada exercício: INSERT em `TreinoExercicio`.

**SQL — INSERT treino:**
```sql
INSERT INTO Treino (IdAcademia, IdInstrutor, IdCliente, Nome, Observacao, DataCriacao, Ativo)
VALUES (@idAcademia, @idInstrutor, @idCliente, @nome, @observacao, CAST(GETDATE() AS DATE), 'A');
SELECT SCOPE_IDENTITY();
```

**SQL — INSERT exercício:**
```sql
INSERT INTO TreinoExercicio (IdTreino, IdExercicio, Series, Repeticoes, CargaKg, Observacao, Ordem)
VALUES (@idTreino, @idExercicio, @series, @repeticoes, @cargaKg, @observacao, @ordem)
```

---

### 10.3 Visualizar Ficha ❌ Pendente

**Rota:** `GET /Treino/Detalhe/{id}`
**Autorização:** `UsuarioCargo == "Instrutor"` ou `ClienteId` dono do treino

**SQL:**
```sql
SELECT tr.Id, tr.Nome, tr.Observacao, tr.DataCriacao,
       cl.Nome AS NomeCliente,
       ex.Nome AS Exercicio, ex.GrupoMuscular,
       te.Series, te.Repeticoes, te.CargaKg, te.Observacao AS ObsExercicio, te.Ordem
FROM Treino tr
    INNER JOIN Cliente cl ON cl.Id = tr.IdCliente
    INNER JOIN TreinoExercicio te ON te.IdTreino = tr.Id
    INNER JOIN Exercicio ex ON ex.Id = te.IdExercicio
WHERE tr.Id = @id AND tr.IdAcademia = @idAcademia
ORDER BY te.Ordem
```

---

### 10.4 Inativar Treino ❌ Pendente

**Rota:** `POST /Treino/Inativar/{id}`
**Autorização:** `UsuarioCargo == "Instrutor"`

**SQL:**
```sql
UPDATE Treino SET Ativo = 'I' WHERE Id = @id AND IdInstrutor = @idInstrutor
```

---

## Módulo 11 — Eventos

### 11.1 Criar Evento ❌ Pendente

**Rota:** `POST /Evento/Criar`
**Autorização:** `UsuarioCargo IN ("Administrador", "Gerente")`

**Input:**

| Campo | Validação |
|---|---|
| Titulo | Obrigatório |
| Descricao | Opcional |
| DataInicio | Obrigatório |
| DataFim | Opcional, deve ser >= DataInicio |

**SQL:**
```sql
INSERT INTO Evento (IdAcademia, IdUsuario, Titulo, Descricao, DataInicio, DataFim, Ativo)
VALUES (@idAcademia, @idUsuario, @titulo, @descricao, @dataInicio, @dataFim, 'A')
```

---

### 11.2 Listar Eventos Ativos ❌ Pendente

**Rota:** `GET /Evento/Listar`
**Autorização:** Qualquer sessão válida (funcionário ou cliente)

**SQL:**
```sql
SELECT ev.Id, ev.Titulo, ev.Descricao, ev.DataInicio, ev.DataFim, us.Nome AS Autor
FROM Evento ev
    INNER JOIN Usuario us ON us.Id = ev.IdUsuario
WHERE ev.IdAcademia = @idAcademia AND ev.Ativo = 'A'
  AND (ev.DataFim IS NULL OR ev.DataFim >= CAST(GETDATE() AS DATE))
ORDER BY ev.DataInicio
```

---

### 11.3 Editar / Inativar Evento ❌ Pendente

**Rotas:**
- `POST /Evento/Editar/{id}`
- `POST /Evento/Inativar/{id}`

**Autorização:** `UsuarioCargo IN ("Administrador", "Gerente")`

---

## Módulo 12 — Permissões por Cargo

### 12.1 Listar Matriz Cargo × Permissão ❌ Pendente

**Rota:** `GET /Permissao/Listar`
**Autorização:** `UsuarioCargo == "Administrador"`

**SQL:**
```sql
SELECT ca.Id AS IdCargo, ca.Nome AS Cargo,
       pe.Id AS IdPermissao, pe.Nome AS Permissao,
       CASE WHEN cp.Id IS NOT NULL THEN 1 ELSE 0 END AS Ativo
FROM Cargo ca
CROSS JOIN Permissao pe
LEFT JOIN CargoPermissao cp ON cp.IdCargo = ca.Id AND cp.IdPermissao = pe.Id
ORDER BY ca.Nome, pe.Nome
```

---

### 12.2 Ativar / Desativar Permissão ❌ Pendente

**Rota:** `POST /Permissao/Alternar`
**Input:** `{ IdCargo, IdPermissao }`
**Autorização:** `UsuarioCargo == "Administrador"`

**Fluxo:**
- Se existe registro em `CargoPermissao` → DELETE.
- Se não existe → INSERT.

---

## Módulo 13 — Relatórios

### 13.1 Financeiro ❌ Pendente

**Rota:** `GET /Relatorio/Financeiro?mes=MM&ano=YYYY`
**Autorização:** `UsuarioCargo IN ("Administrador", "Gerente")`

**Dados retornados:**
- Receita total do mês (soma de `ValorPago` com status Pago)
- Inadimplência do mês (soma de `ValorPago` com status Atrasado)
- Quantidade de alunos adimplentes vs inadimplentes

**SQL:**
```sql
SELECT
    SUM(CASE WHEN sp.Nome = 'Pago' THEN pg.ValorPago ELSE 0 END) AS ReceitaTotal,
    SUM(CASE WHEN sp.Nome = 'Atrasado' THEN pg.ValorPago ELSE 0 END) AS Inadimplencia,
    COUNT(DISTINCT CASE WHEN sp.Nome = 'Atrasado' THEN mc.IdCliente END) AS AlunosInadimplentes
FROM Pagamento pg
    INNER JOIN StatusPagamento sp ON sp.Id = pg.IdStatusPagamento
    INNER JOIN MatriculaCliente mc ON mc.Id = pg.IdMatriculaCliente
    INNER JOIN Cliente cl ON cl.Id = mc.IdCliente
WHERE cl.IdAcademia = @idAcademia
  AND MONTH(pg.DataVencimento) = @mes AND YEAR(pg.DataVencimento) = @ano
```

---

### 13.2 Alunos ❌ Pendente

**Rota:** `GET /Relatorio/Alunos?mes=MM&ano=YYYY`
**Autorização:** `UsuarioCargo IN ("Administrador", "Gerente")`

**Dados retornados:**
- Total de alunos ativos
- Novos alunos no período (DataInicio da MatriculaCliente dentro do mês)
- Alunos inativos

**SQL:**
```sql
SELECT
    COUNT(CASE WHEN mc.StatusSituacao = 'A' THEN 1 END) AS Ativos,
    COUNT(CASE WHEN mc.StatusSituacao = 'I' THEN 1 END) AS Inativos,
    COUNT(CASE WHEN mc.StatusSituacao = 'A'
               AND MONTH(mc.DataInicio) = @mes
               AND YEAR(mc.DataInicio) = @ano THEN 1 END) AS Novos
FROM MatriculaCliente mc
    INNER JOIN Cliente cl ON cl.Id = mc.IdCliente
WHERE cl.IdAcademia = @idAcademia
```

---

### 13.3 Frequência ❌ Pendente

**Rota:** `GET /Relatorio/Frequencia?mes=MM&ano=YYYY`
**Autorização:** `UsuarioCargo IN ("Administrador", "Gerente", "Instrutor")`

**SQL:**
```sql
SELECT cl.Nome, COUNT(ci.Id) AS TotalVisitas,
       MAX(ci.DataHoraEntrada) AS UltimaVisita
FROM CheckIn ci
    INNER JOIN Cliente cl ON cl.Id = ci.IdCliente
WHERE ci.IdAcademia = @idAcademia
  AND MONTH(ci.DataHoraEntrada) = @mes AND YEAR(ci.DataHoraEntrada) = @ano
GROUP BY cl.Id, cl.Nome
ORDER BY TotalVisitas DESC
```

---

### 13.4 Evolução Física ❌ Pendente

**Rota:** `GET /Relatorio/EvolucaoFisica/{idCliente}`
**Autorização:** `UsuarioCargo == "Instrutor"` ou `ClienteId == idCliente`

**Retorno:** Lista de avaliações ordenadas por data — frontend renderiza gráfico de linha por medida.

---

## Módulo 14 — Notificações por E-mail

### 14.1 E-mail de Boas-vindas ❌ Pendente

**Gatilho:** Após INSERT bem-sucedido de `Cliente` (chamado de forma assíncrona no service).
**Destinatário:** `Cliente.Email`
**Conteúdo:** Nome do cliente, nome da academia, instruções de acesso.
**Implementação sugerida:** Criar `EmailService` com `SmtpClient` ou biblioteca `MailKit`.

---

### 14.2 E-mail de Vencimento de Mensalidade ❌ Pendente

**Gatilho:** Job agendado (`IHostedService` ou `BackgroundService`) rodando diariamente.
**Regra:** Enviar para clientes com `Pagamento.DataVencimento == HOJE + 3 dias` e status Pendente.

**SQL para busca:**
```sql
SELECT cl.Email, cl.Nome, pg.DataVencimento, pg.ValorPago, ac.Nome AS Academia
FROM Pagamento pg
    INNER JOIN MatriculaCliente mc ON mc.Id = pg.IdMatriculaCliente
    INNER JOIN Cliente cl ON cl.Id = mc.IdCliente
    INNER JOIN Academia ac ON ac.Id = cl.IdAcademia
    INNER JOIN StatusPagamento sp ON sp.Id = pg.IdStatusPagamento
WHERE pg.DataVencimento = CAST(DATEADD(DAY, 3, GETDATE()) AS DATE)
  AND sp.Nome = 'Pendente'
```

---

## Módulo 15 — Auditoria

### 15.1 Escrita no AuditoriaLog ❌ Pendente

**Quando:** Em todo INSERT, UPDATE, DELETE nas tabelas críticas (Cliente, Usuario, MatriculaCliente, Pagamento).
**Implementação sugerida:** Método helper no repositório base ou decorador.

**SQL (AuditoriaRepository.Registrar):**
```sql
INSERT INTO AuditoriaLog
    (IdAcademia, IdUsuario, TabelaAfetada, RegistroId, Acao, CampoAlterado, ValorAnterior, ValorNovo, IpOrigem)
VALUES
    (@idAcademia, @idUsuario, @tabela, @registroId, @acao, @campo, @anterior, @novo, @ip)
```

**IP de origem:** `HttpContext.Connection.RemoteIpAddress?.ToString()` — injetar via controller antes de chamar o service.

---

## Dashboard por Perfil — Dados Reais

### Administrador ❌ Pendente (UI pronta, dados estáticos)

Cards a preencher com queries reais:
- Total de alunos ativos → `SELECT COUNT(*) FROM Cliente + MatriculaCliente`
- Receita do mês → query do Relatório Financeiro
- Vencimentos próximos (7 dias) → `WHERE DataVencimento BETWEEN HOJE AND HOJE+7 AND Status = 'Pendente'`
- Alertas de inadimplência → alunos com status Atrasado

### Gerente ❌ Pendente (view ainda não existe)

Igual ao Administrador, sem acesso a permissões e configurações críticas.

### Recepcionista ❌ Pendente (UI pronta, dados estáticos)

- Check-ins do dia
- Mensalidades vencendo essa semana
- Últimos alunos cadastrados

### Instrutor ❌ Pendente (UI pronta, dados estáticos)

- Lista dos alunos com treino atribuído pelo instrutor logado
- Avaliações físicas pendentes (alunos sem avaliação nos últimos 30 dias)

### Cliente ❌ Pendente (UI pronta, dados estáticos)

- Status da mensalidade atual
- Ficha de treino ativa
- Próxima avaliação física