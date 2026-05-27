# PRD — Sistema de Gestão de Academia

**Produto:** Sistema Academia (FitManager)
**Tipo:** Aplicação Web — ASP.NET Core MVC
**Branch ativa:** `feat/victor-leite`
**Última atualização:** 2026-05-19

---

## Visão Geral

Sistema web para gestão completa de academias. Suporta múltiplas academias isoladas (multi-tenant por CNPJ). Possui dois tipos de acesso: **funcionários** (Administrador, Gerente, Recepcionista, Instrutor) e **clientes** (alunos com login próprio).

---

## Stack Técnica

| Camada | Tecnologia |
|---|---|
| Backend | ASP.NET Core MVC (.NET 10) |
| Banco de dados | SQL Server (SQLEXPRESS) |
| Acesso a dados | ADO.NET direto (sem ORM) |
| Frontend | Bootstrap 5, jQuery, CSS com OKLCH |
| Segurança | SHA-256 para senhas, sessão HTTP (30 min) |

---

## Perfis de Acesso

| Perfil | Tipo | Descrição |
|---|---|---|
| Administrador | Funcionário | Acesso total ao sistema |
| Gerente | Funcionário | Acesso intermediário — sem configurações críticas |
| Recepcionista | Funcionário | Gerencia check-in, clientes e mensalidades |
| Instrutor | Funcionário | Gerencia treinos e avaliações físicas |
| Cliente | Aluno | Login próprio — acessa seus dados pessoais |

---

## Modelo de Dados (banco atual)

### Tabelas implementadas no script SQL

- [x] `OrientacaoSexual` — dado cadastral do aluno (sem impacto em regras de negócio)
- [x] `Cargo` — Administrador, Gerente, Recepcionista, Instrutor
- [x] `Permissao` — permissões granulares do sistema
- [x] `CargoPermissao` — matriz cargo × permissão
- [x] `Endereco` — usado por Academia e Cliente
- [x] `Academia` — tenant principal, com CNPJ único e flag Ativo
- [x] `Usuario` — funcionários com cargo e senha SHA-256
- [x] `Cliente` — alunos com login próprio, endereço e orientação sexual
- [x] `Telefone` — telefones do cliente (múltiplos)
- [x] `CheckIn` — registro de entrada/saída via QR Code
- [x] `AvaliacaoFisica` — medidas corporais com data e % gordura/massa magra
- [x] `Exercicio` — catálogo de exercícios com grupo muscular
- [x] `Modalidade` — aulas/atividades da academia por tenant
- [x] `Plano` — planos de assinatura com desconto percentual
- [x] `MatriculaCliente` — vínculo aluno × modalidade × plano
- [x] `Treino` — ficha de treino individual criada pelo instrutor
- [x] `TreinoExercicio` — exercícios da ficha com séries, reps, carga e ordem
- [x] `Evento` — comunicados e eventos criados por Admin/Gerente
- [x] `MetodoPagamento` — lookup: Pix, Cartão, Boleto, Dinheiro etc.
- [x] `StatusPagamento` — lookup: Pendente, Pago, Atrasado, Cancelado
- [x] `Pagamento` — mensalidades geradas automaticamente na matrícula
- [x] `AuditoriaLog` — log de INSERT/UPDATE/DELETE com IP e usuário

---

## Funcionalidades por Módulo

---

### 1. Autenticação

| Funcionalidade | Status | Quem acessa |
|---|---|---|
| Login de funcionário (Usuario) | ✅ Feito | Todos os cargos |
| Login de cliente (Cliente) | ❌ Pendente | Aluno |
| Logout com limpeza de sessão | ✅ Feito | Todos |
| Redirecionamento por cargo após login | ✅ Feito | Funcionários |
| SHA-256 nas senhas | ✅ Feito | — |
| Recuperação de senha por e-mail | ❌ Pendente | Todos |

---

### 2. Cadastro de Academia (Onboarding)

| Funcionalidade | Status | Quem acessa |
|---|---|---|
| Wizard 3 etapas: Admin → Academia → Endereço | ✅ Feito | — |
| Validação de CNPJ único | ✅ Feito | — |
| Transação multi-tabela (rollback em falha) | ✅ Feito | — |
| Máscaras de input (CNPJ, CPF, CEP) | ✅ Feito | — |
| Validação client-side nos formulários | ✅ Feito | — |

---

### 3. Gestão de Funcionários

| Funcionalidade | Status | Quem acessa |
|---|---|---|
| Cadastro de funcionário com cargo | ✅ Feito | Administrador |
| Listagem de cargos (CargoRepository) | ✅ Feito | — |
| Listagem real de funcionários do banco | ❌ Pendente | Administrador, Gerente |
| Edição de dados do funcionário | ❌ Pendente | Administrador |
| Inativação / reativação de funcionário | ❌ Pendente | Administrador |

---

### 4. Gestão de Clientes (Alunos)

| Funcionalidade | Status | Quem acessa |
|---|---|---|
| Cadastro de cliente com endereço e orientação sexual | ❌ Pendente | Recepcionista |
| Listagem de clientes com busca e filtros | ❌ Pendente | Recepcionista, Gerente, Administrador |
| Edição de dados do cliente | ❌ Pendente | Recepcionista |
| Inativação / reativação de cliente | ❌ Pendente | Recepcionista, Administrador |
| Cadastro de telefone do cliente | ❌ Pendente | Recepcionista |
| Geração de QR Code para check-in | ❌ Pendente | Recepcionista |

---

### 5. Check-in via QR Code

| Funcionalidade | Status | Quem acessa |
|---|---|---|
| Leitura de QR Code na recepção | ❌ Pendente | Recepcionista |
| Registro automático de DataHoraEntrada | ❌ Pendente | Sistema |
| Registro de DataHoraSaida | ❌ Pendente | Recepcionista |
| Histórico de check-ins por aluno | ❌ Pendente | Recepcionista, Instrutor, Administrador |

---

### 6. Planos e Matrículas

| Funcionalidade | Status | Quem acessa |
|---|---|---|
| CRUD de planos (com desconto e tempo) | ❌ Pendente | Administrador, Gerente |
| CRUD de modalidades por academia | ❌ Pendente | Administrador, Gerente |
| Matrícula de aluno em modalidade + plano | ❌ Pendente | Recepcionista |
| Geração automática de mensalidades ao matricular | ❌ Pendente | Sistema |
| Renovação de matrícula | ❌ Pendente | Recepcionista |
| Suspensão / cancelamento de matrícula | ❌ Pendente | Recepcionista, Administrador |

---

### 7. Pagamentos e Mensalidades

| Funcionalidade | Status | Quem acessa |
|---|---|---|
| Listagem de mensalidades do aluno | ❌ Pendente | Recepcionista |
| Registro de pagamento (com método de pagamento) | ❌ Pendente | Recepcionista |
| Atualização de status (Pago, Atrasado, Cancelado) | ❌ Pendente | Recepcionista |
| Alertas de inadimplência no dashboard | ❌ Pendente | Administrador, Gerente |
| Histórico de pagamentos por aluno | ❌ Pendente | Recepcionista, Administrador |
| E-mail automático de aviso de vencimento | ❌ Pendente | Sistema |

---

### 8. Avaliação Física

| Funcionalidade | Status | Quem acessa |
|---|---|---|
| Cadastro de avaliação física do aluno | ❌ Pendente | Instrutor |
| Listagem de avaliações do aluno | ❌ Pendente | Instrutor, Cliente |
| Comparativo entre avaliações (evolução) | ❌ Pendente | Instrutor, Cliente |
| Gráfico de evolução corporal | ❌ Pendente | Instrutor, Cliente |

---

### 9. Treinos

| Funcionalidade | Status | Quem acessa |
|---|---|---|
| Catálogo de exercícios (CRUD de Exercicio) | ❌ Pendente | Instrutor |
| Criação de ficha de treino para aluno | ❌ Pendente | Instrutor |
| Adição de exercícios à ficha (séries, reps, carga, ordem) | ❌ Pendente | Instrutor |
| Edição e inativação de treino | ❌ Pendente | Instrutor |
| Visualização da ficha de treino | ❌ Pendente | Cliente, Instrutor |
| Anotações de progressão por treino | ❌ Pendente | Instrutor |

---

### 10. Eventos e Comunicados

| Funcionalidade | Status | Quem acessa |
|---|---|---|
| Criação de evento (título, descrição, período) | ❌ Pendente | Administrador, Gerente |
| Listagem de eventos ativos | ❌ Pendente | Todos (incluindo Cliente) |
| Edição e inativação de evento | ❌ Pendente | Administrador, Gerente |

---

### 11. Permissões por Cargo

| Funcionalidade | Status | Quem acessa |
|---|---|---|
| Listagem da matriz cargo × permissão | ❌ Pendente | Administrador |
| Ativação/desativação de permissões por cargo | ❌ Pendente | Administrador |
| Verificação de permissão nas actions do controller | ❌ Pendente | Sistema |

---

### 12. Relatórios

| Relatório | Status | Quem acessa |
|---|---|---|
| Financeiro: receita mensal e inadimplência | ❌ Pendente | Administrador, Gerente |
| Alunos: ativos, inativos e novos por período | ❌ Pendente | Administrador, Gerente |
| Frequência: check-ins por aluno | ❌ Pendente | Administrador, Gerente, Instrutor |
| Evolução física: comparativo de avaliações | ❌ Pendente | Instrutor |

---

### 13. Notificações por E-mail

| Notificação | Gatilho | Status |
|---|---|---|
| Boas-vindas ao aluno | Cadastro de cliente | ❌ Pendente |
| Aviso de vencimento de mensalidade | D-3 antes do vencimento | ❌ Pendente |

---

### 14. Dashboard por Perfil

| Painel | Status | Dados reais |
|---|---|---|
| Administrador (cards, gráficos, alertas) | UI feita | ❌ Estático |
| Gerente | ❌ Pendente | ❌ |
| Recepcionista (check-in, mensalidades) | UI feita | ❌ Estático |
| Instrutor (meus alunos, treinos) | UI feita | ❌ Estático |
| Cliente (treinos, mensalidades, evolução) | UI feita | ❌ Estático |

---

### 15. Infraestrutura e Qualidade

| Item | Status |
|---|---|
| Queries parametrizadas (prevenção SQL Injection) | ✅ Feito |
| Transações para operações multi-tabela | ✅ Feito |
| Connection string via variável de ambiente | ✅ Feito |
| AuditoriaLog no banco | ✅ Script pronto |
| Middleware de autorização centralizado por cargo | ❌ Pendente |
| Injeção de dependência via container ASP.NET Core | ❌ Pendente |
| Testes automatizados (unitários e integração) | ❌ Pendente |
| Escrita no AuditoriaLog nas operações de dados | ❌ Pendente |
| Deploy em produção | ❌ Pendente |

---

## Backlog Prioritário

| # | Entrega | Módulo |
|---|---|---|
| 1 | Login do cliente | Autenticação |
| 2 | CRUD de clientes (ClienteRepository) | Gestão de Clientes |
| 3 | Dashboard com dados reais do banco | Dashboard |
| 4 | CRUD de planos e modalidades | Planos |
| 5 | Matrícula + geração automática de mensalidades | Matrículas / Pagamentos |
| 6 | Registro de pagamento pela recepcionista | Pagamentos |
| 7 | QR Code e sistema de check-in | Check-in |
| 8 | Ficha de treino (instrutor → aluno) | Treinos |
| 9 | Avaliação física com gráfico de evolução | Avaliação Física |
| 10 | Relatórios com dados reais | Relatórios |
| 11 | Matriz de permissões persistida e verificada | Permissões |
| 12 | Painel do Gerente | Dashboard |
| 13 | E-mails automáticos (boas-vindas e vencimento) | Notificações |
| 14 | Eventos e comunicados | Eventos |
| 15 | AuditoriaLog nas operações de dados | Infraestrutura |