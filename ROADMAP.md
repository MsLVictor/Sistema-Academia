# ROADMAP — Sistema Academia

**Base:** SPEC v2026-05-19
**Critério de modelo:** Haiku = CRUD simples | Sonnet = regras de negócio + transações | Opus = lógica complexa + relatórios + arquitetura

---

## Legenda de Modelos

| Modelo | Quando usar |
|---|---|
| **Haiku** | Listagens, CRUDs de tabelas simples, formulários sem regras de negócio, views estáticas |
| **Sonnet** | Transações multi-tabela moderadas, regras de negócio, autenticação, integração entre módulos |
| **Opus** | Lógica de negócio crítica e interdependente, relatórios com agregações complexas, arquitetura de serviços, jobs assíncronos |

---

## Etapa 1 — Fundação ✅ / 🔧 Pendente

> Objetivo: corrigir a base existente e garantir que o alicerce esteja sólido antes de construir.

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 1.1 | Gravar `UsuarioId` na sessão após login | `LoginController`, `UsuarioRepository` | Haiku | ✅ |
| 1.2 | Login de cliente (`POST /Login/LogarCliente`) | `LoginController`, `ClienteRepository` | Sonnet | ❌ |
| 1.3 | View do Gerente (`Home/Gerente`) | `HomeController`, `Gerente.cshtml` | Haiku | ❌ |
| 1.4 | Corrigir redirecionamento de cargo "Gerente" no switch do login | `LoginController` | Haiku | ❌ |
| 1.5 | Filtro de sessão global (`SessaoAutorizadaAttribute`) | `Filters/SessaoAutorizadaAttribute.cs` | Haiku | ✅ |

---

## Etapa 2 — Gestão de Funcionários ✅

> Objetivo: completar o CRUD de funcionários iniciado na Etapa 1.

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 2.1 | Listar funcionários da academia (`GET /Funcionario/Listar`) | `FuncionarioController`, `UsuarioRepository` | Haiku | ✅ |
| 2.2 | Editar funcionário (`GET+POST /Funcionario/Editar/{id}`) | `FuncionarioController`, `UsuarioRepository` | Haiku | ✅ |
| 2.3 | Inativar / reativar funcionário (`POST /Funcionario/AlterarStatus/{id}`) | `FuncionarioController`, `UsuarioRepository` | Haiku | ✅ |
| 2.4 | Conectar tabela de funcionários no dashboard do Administrador com dados reais | `Home/Administrador.cshtml` | Haiku | ✅ |

---

## Etapa 3 — Gestão de Clientes ✅ / 🔧 Pendente

> Objetivo: implementar o `ClienteRepository` e todo o fluxo de alunos.

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 3.1 | Cadastrar cliente com endereço e telefone (transação multi-tabela) | `ClienteController`, `ClienteRepository` | Sonnet | ✅ |
| 3.2 | Listar clientes com busca e filtros | `ClienteController`, `ClienteRepository` | Haiku | ✅ |
| 3.3 | Editar cliente e endereço | `ClienteController`, `ClienteRepository` | Haiku | ✅ |
| 3.4 | Inativar / reativar cliente | `ClienteController`, `ClienteRepository` | Sonnet | ✅ |
| 3.5 | Gerar QR Code do cliente (`GET /Cliente/QrCode/{id}`) | `ClienteController` + NuGet QRCoder | Sonnet | ❌ |
| 3.6 | Conectar tabela de alunos no dashboard com dados reais | `Home/Administrador.cshtml` | Haiku | ❌ |

---

## Etapa 4 — Planos, Modalidades e Matrículas ✅ / 🔧 Pendente

> Objetivo: habilitar a matrícula de alunos com geração automática de mensalidades — núcleo do negócio.

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 4.1 | CRUD de Planos | `PlanoController`, `PlanoRepository` | Haiku | ✅ |
| 4.2 | CRUD de Modalidades | `ModalidadeController`, `ModalidadeRepository` | Haiku | ✅ |
| 4.3 | Matricular aluno (transação: MatriculaCliente + geração de Pagamentos) | `MatriculaController`, `MatriculaRepository` | **Opus** | ✅ |
| 4.4 | Cancelar / suspender matrícula (e cancelar parcelas futuras) | `MatriculaController`, `MatriculaRepository` | Sonnet | ✅ |
| 4.5 | Renovar matrícula | `MatriculaController` | Sonnet | ❌ |

---

## Etapa 5 — Pagamentos ✅ / 🔧 Pendente

> Objetivo: controle financeiro do dia a dia da recepção.

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 5.1 | Listar mensalidades de um aluno | `PagamentoController`, `PagamentoRepository` | Haiku | ✅ |
| 5.2 | Registrar pagamento manual (método + valor + data) | `PagamentoController`, `PagamentoRepository` | Haiku | ✅ |
| 5.3 | Marcar parcelas vencidas como Atrasado (ação manual) | `PagamentoController`, `PagamentoRepository` | Sonnet | ✅ |
| 5.4 | Métodos de pagamento seedados no banco | `insert_SistemaAcademia.sql` | Haiku | ✅ |
| 5.5 | Alertas de inadimplência no dashboard | `Home/Administrador.cshtml`, `PagamentoRepository` | Haiku | ❌ |
| 5.6 | Integração Asaas (PIX dinâmico + boleto) — hook preparado | `PagamentoService`, `PagamentoRepository` | Sonnet | 🔧 Aguardando credenciais Asaas |

---

## Etapa 6 — Check-in por CPF ✅ / 🔧 Pendente

> Objetivo: registro de frequência dos alunos na recepção via CPF (QR Code adiado — depende do modelo da catraca).

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 6.1 | Registrar entrada por CPF (`POST /CheckIn/Entrada`) | `CheckInController`, `CheckInRepository`, `CheckInService` | Sonnet | ✅ |
| 6.2 | View scanner mobile-first com AJAX e feedback animado | `Views/CheckIn/Scanner.cshtml` | Sonnet | ✅ |
| 6.3 | Registrar saída (`POST /CheckIn/Saida/{id}`) | `CheckInController`, `CheckInRepository` | Haiku | ✅ |
| 6.4 | Histórico de check-ins por aluno | `CheckInController`, `CheckInRepository` | Haiku | ✅ |
| 6.5 | Integração com catraca (hook `LiberarCatraca`) | `CheckInService` | — | 🔧 Aguardando modelo/protocolo |
| 6.6 | QR Code por aluno (alternativa futura ao CPF) | `ClienteController`, `CheckInController` | Sonnet | ❌ |

---

## Etapa 7 — Avaliação Física

> Objetivo: registro e acompanhamento de evolução corporal pelo instrutor.

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 7.1 | Cadastrar avaliação física | `AvaliacaoFisicaController`, `AvaliacaoFisicaRepository` | Haiku | ❌ |
| 7.2 | Listar histórico de avaliações por aluno | `AvaliacaoFisicaController` | Haiku | ❌ |
| 7.3 | Gráfico comparativo de evolução (frontend Chart.js) | `Home/Instrutor.cshtml`, `Home/Cliente.cshtml` | Sonnet | ❌ |

---

## Etapa 8 — Treinos

> Objetivo: fichas de treino individuais criadas pelo instrutor para cada aluno.

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 8.1 | CRUD de Exercícios (catálogo global) | `ExercicioController`, `ExercicioRepository` | Haiku | ❌ |
| 8.2 | Criar ficha de treino com exercícios (transação: Treino + TreinoExercicio) | `TreinoController`, `TreinoRepository` | Sonnet | ❌ |
| 8.3 | Visualizar ficha de treino | `TreinoController` | Haiku | ❌ |
| 8.4 | Editar exercícios da ficha (reordenar, alterar séries/carga) | `TreinoController`, `TreinoRepository` | Haiku | ❌ |
| 8.5 | Inativar treino | `TreinoController` | Haiku | ❌ |

---

## Etapa 9 — Eventos e Comunicados

> Objetivo: comunicação interna da academia com alunos e funcionários.

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 9.1 | Criar evento | `EventoController`, `EventoRepository` | Haiku | ❌ |
| 9.2 | Listar eventos ativos (todos os perfis) | `EventoController` | Haiku | ❌ |
| 9.3 | Editar e inativar evento | `EventoController` | Haiku | ❌ |

---

## Etapa 10 — Permissões por Cargo

> Objetivo: matriz de controle de acesso configurável pelo administrador.

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 10.1 | Listar matriz cargo × permissão | `PermissaoController`, `PermissaoRepository` | Sonnet | ❌ |
| 10.2 | Alternar permissão de cargo (toggle INSERT/DELETE) | `PermissaoController` | Haiku | ❌ |
| 10.3 | Popular tabela `Permissao` com permissões do sistema (seed) | `insert_SistemaAcademia.sql` | Haiku | ❌ |

---

## Etapa 11 — Relatórios

> Objetivo: visão gerencial com dados reais do banco.

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 11.1 | Relatório financeiro (receita, inadimplência por mês) | `RelatorioController`, `RelatorioRepository` | **Opus** | ❌ |
| 11.2 | Relatório de alunos (ativos, inativos, novos) | `RelatorioController` | Sonnet | ❌ |
| 11.3 | Relatório de frequência (check-ins por aluno no mês) | `RelatorioController` | Sonnet | ❌ |
| 11.4 | Relatório de evolução física (gráfico por aluno) | `RelatorioController` | Sonnet | ❌ |
| 11.5 | Conectar todos os cards do dashboard Administrador com dados reais | `Home/Administrador.cshtml` | **Opus** | ❌ |

---

## Etapa 12 — Notificações por E-mail

> Objetivo: comunicação automática com alunos via e-mail.

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 12.1 | Criar `EmailService` (SmtpClient / MailKit) | `Services/EmailService.cs` | Sonnet | ❌ |
| 12.2 | E-mail de boas-vindas ao cadastrar cliente | `ClienteService`, `EmailService` | Sonnet | ❌ |
| 12.3 | Job diário de aviso de vencimento (D-3) via `BackgroundService` | `Services/VencimentoNotificacaoJob.cs` | **Opus** | ❌ |

---

## Etapa 13 — Auditoria

> Objetivo: rastreabilidade de quem fez o quê no sistema.

| # | Tarefa | Arquivo(s) | Modelo | Status |
|---|---|---|---|---|
| 13.1 | Criar `AuditoriaRepository` com método `Registrar` | `Repositories/AuditoriaRepository.cs` | Haiku | ❌ |
| 13.2 | Injetar auditoria nas operações críticas (Cliente, Pagamento, Matrícula) | Todos os repositories afetados | Sonnet | ❌ |

---

## Resumo por Modelo

| Modelo | Etapas | Exemplos |
|---|---|---|
| **Haiku** | 1.3, 1.4, 2.1–2.4, 3.2, 3.3, 4.1, 4.2, 5.1, 5.2, 5.4, 5.5, 6.3, 6.4, 7.1, 7.2, 8.1, 8.3, 8.4, 8.5, 9.1–9.3, 10.2, 10.3, 13.1 | CRUDs simples, listagens, views |
| **Sonnet** | 1.2, 3.1, 3.4, 3.5, 4.4, 4.5, 5.3, 6.1, 6.2, 7.3, 8.2, 10.1, 11.2, 11.3, 11.4, 12.1–12.2, 13.2 | Transações moderadas, regras de negócio, integrações |
| **Opus** | 4.3, 11.1, 11.5, 12.3 | Matrícula + geração de mensalidades, relatório financeiro, job de e-mail, dashboard com dados reais |