CREATE DATABASE SistemaAcademia;
GO
USE SistemaAcademia;
GO

CREATE TABLE OrientacaoSexual
(
    Id INT IDENTITY,
    Nome VARCHAR(50) NOT NULL,

    CONSTRAINT Pk_IdOrientacaoSexual PRIMARY KEY(Id)
);
GO

CREATE TABLE Cargo
(
    Id INT IDENTITY,
    Nome VARCHAR(150) NOT NULL,

    CONSTRAINT Pk_IdCargo PRIMARY KEY(Id)
);
GO

CREATE TABLE Permissao
(
    Id INT IDENTITY,
    Nome VARCHAR(100) NOT NULL,
    Descricao VARCHAR(255),

    CONSTRAINT Pk_IdPermissao PRIMARY KEY(Id),
    CONSTRAINT Un_NomePermissao UNIQUE (Nome)
);
GO

CREATE TABLE CargoPermissao
(
    Id INT IDENTITY,
    IdCargo INT NOT NULL,
    IdPermissao INT NOT NULL,

    CONSTRAINT Pk_IdCargoPermissao PRIMARY KEY(Id),
    CONSTRAINT Fk_IdCargo_CargoPermissao FOREIGN KEY(IdCargo) REFERENCES Cargo(Id),
    CONSTRAINT Fk_IdPermissao_CargoPermissao FOREIGN KEY(IdPermissao) REFERENCES Permissao(Id),
    CONSTRAINT Un_CargoPermissao UNIQUE (IdCargo, IdPermissao)
);
GO

CREATE TABLE Endereco
(
    Id INT IDENTITY,
    Logradouro VARCHAR(255) NOT NULL,
    Numero VARCHAR(10)  NOT NULL,
    Complemento VARCHAR(100),
    Bairro VARCHAR(100) NOT NULL,
    Cidade VARCHAR(100) NOT NULL,
    Estado CHAR(2)      NOT NULL,
    CEP CHAR(8)      NOT NULL,

    CONSTRAINT Pk_IdEndereco PRIMARY KEY(Id)
);
GO

CREATE TABLE Academia
(
    Id INT IDENTITY,
    IdEndereco INT NOT NULL,
    Nome VARCHAR(150) NOT NULL,
    CNPJ CHAR(14) NOT NULL,
    Email VARCHAR(250) NOT NULL,
    Ativo CHAR(1) NOT NULL DEFAULT 'A',

    CONSTRAINT Pk_IdAcademia            PRIMARY KEY(Id),
    CONSTRAINT Fk_IdEndereco_Academia   FOREIGN KEY(IdEndereco) REFERENCES Endereco(Id),
    CONSTRAINT Chk_AtivoAcademia        CHECK(Ativo LIKE '[aA]' OR Ativo LIKE '[iI]'),
    CONSTRAINT Un_CNPJAcademia UNIQUE (CNPJ)
);
GO

CREATE TABLE Usuario
(
    Id INT IDENTITY,
    IdAcademia INT NOT NULL,
    IdCargo INT NOT NULL,
    Nome VARCHAR(250) NOT NULL,
    CPF CHAR(11) NOT NULL,
    Email VARCHAR(250) NOT NULL,
    Senha VARBINARY(32) NOT NULL,
    Ativo CHAR(1) NOT NULL DEFAULT 'A',

    CONSTRAINT Pk_IdUsuario PRIMARY KEY(Id),
    CONSTRAINT Chk_AtivoUsuario CHECK(Ativo LIKE '[aA]' OR Ativo LIKE '[iI]'),
    CONSTRAINT Fk_IdAcademia_Usuario FOREIGN KEY(IdAcademia) REFERENCES Academia(Id),
    CONSTRAINT Fk_IdCargo_Usuario FOREIGN KEY(IdCargo) REFERENCES Cargo(Id),
    CONSTRAINT Un_CPFUsuario UNIQUE (CPF),
    CONSTRAINT Un_EmailUsuario UNIQUE (EMAIL)
);
GO

CREATE TABLE Cliente
(
    Id INT IDENTITY,
    IdAcademia INT NOT NULL,
    IdEndereco INT NOT NULL,
    IdUsuario INT NOT NULL,
    IdOrientacaoSexual INT NOT NULL,
    Nome VARCHAR(250) NOT NULL,
    CPF CHAR(11) NOT NULL,
    DataNascimento DATE NOT NULL,
    Email VARCHAR(250) NOT NULL,
    Senha VARBINARY(32) NOT NULL,
    Ativo CHAR(1) NOT NULL DEFAULT 'A',

    CONSTRAINT Pk_IdCliente PRIMARY KEY(Id),
    CONSTRAINT Chk_AtivoCliente CHECK(Ativo LIKE '[aA]' OR Ativo LIKE '[iI]'),
    CONSTRAINT Fk_IdAcademia_Cliente FOREIGN KEY(IdAcademia) REFERENCES Academia(Id),
    CONSTRAINT Fk_IdEndereco_Cliente FOREIGN KEY(IdEndereco) REFERENCES Endereco(Id),
    CONSTRAINT Fk_IdUsuario_Cliente FOREIGN KEY(IdUsuario) REFERENCES Usuario(Id),
    CONSTRAINT Fk_IdOrientacaoSexual_Cliente FOREIGN KEY(IdOrientacaoSexual) REFERENCES OrientacaoSexual(Id),
    CONSTRAINT Un_CPFCliente UNIQUE (CPF),
    CONSTRAINT Un_EmailCliente UNIQUE (EMAIL)
);
GO

CREATE TABLE Telefone
(
    Id                  INT IDENTITY,
    IdCliente           INT         NOT NULL,
    Telefone            VARCHAR(15) NOT NULL,

    CONSTRAINT Pk_IdTelefone            PRIMARY KEY(Id),
    CONSTRAINT Fk_IdCliente_Telefone    FOREIGN KEY(IdCliente) REFERENCES Cliente(Id)
);
GO

CREATE TABLE CheckIn
(
    Id INT IDENTITY,
    IdCliente INT NOT NULL,
    IdAcademia INT NOT NULL,
    IdUsuarioRegistro INT NOT NULL,
    DataHoraEntrada DATETIME NOT NULL DEFAULT GETDATE(),
    DataHoraSaida DATETIME,

    CONSTRAINT Pk_IdCheckIn PRIMARY KEY(Id),
    CONSTRAINT Fk_IdCliente_CheckIn FOREIGN KEY(IdCliente) REFERENCES Cliente(Id),
    CONSTRAINT Fk_IdAcademia_CheckIn FOREIGN KEY(IdAcademia) REFERENCES Academia(Id),
    CONSTRAINT Fk_IdUsuarioRegistro_CheckIn FOREIGN KEY(IdUsuarioRegistro) REFERENCES Usuario(Id)
);
GO

CREATE TABLE AvaliacaoFisica
(
    Id INT IDENTITY,
    IdCliente INT NOT NULL,
    Peso FLOAT,
    PorcentagemGorduraCorporal FLOAT,
    PorcentagemMassaMagra FLOAT,
    Altura FLOAT,
    Torax FLOAT,
    Cintura FLOAT,
    Coxa FLOAT,
    Panturrilha FLOAT,
    Biceps FLOAT,
    DataAvaliacao DATE,

    CONSTRAINT Pk_IdAvaliacaoFisica PRIMARY KEY(Id),
    CONSTRAINT Fk_IdCliente_AvaliacaoFisica FOREIGN KEY(IdCliente) REFERENCES Cliente(Id)
);
GO

CREATE TABLE Exercicio
(
    Id INT IDENTITY,
    Nome VARCHAR(250) NOT NULL,
    GrupoMuscular VARCHAR(100) NOT NULL,
    Descricao VARCHAR(500),

    CONSTRAINT Pk_IdExercicio PRIMARY KEY(Id)
);
GO

CREATE TABLE Modalidade
(
    Id INT IDENTITY,
    IdAcademia INT NOT NULL,
    Nome VARCHAR(250) NOT NULL,
    ValorModalidade DECIMAL(10,2) NOT NULL,

    CONSTRAINT Pk_IdModalidade PRIMARY KEY(Id),
    CONSTRAINT Fk_IdAcademia_Modalidade FOREIGN KEY(IdAcademia) REFERENCES Academia(Id)
);
GO

CREATE TABLE Plano
(
    Id INT IDENTITY,
    IdAcademia INT NOT NULL,
    Nome VARCHAR(255) NOT NULL,
    TempoPlano CHAR(2) NOT NULL,
    PercentualDesconto FLOAT NOT NULL,

    CONSTRAINT Pk_IdPlano PRIMARY KEY(Id),
    CONSTRAINT Fk_IdAcademia_Plano FOREIGN KEY(IdAcademia) REFERENCES Academia(Id)
);
GO

CREATE TABLE MatriculaCliente
(
    Id INT IDENTITY,
    IdCliente INT NOT NULL,
    IdModalidade INT NOT NULL,
    IdPlano INT NOT NULL,
    DataInicio DATETIME NOT NULL,
    StatusSituacao  CHAR(1) NOT NULL,

    CONSTRAINT Pk_IdMatriculaCliente PRIMARY KEY(Id),
    CONSTRAINT Fk_IdCliente_MatriculaCliente FOREIGN KEY(IdCliente) REFERENCES Cliente(Id),
    CONSTRAINT Fk_IdModalidade_MatriculaCliente FOREIGN KEY(IdModalidade) REFERENCES Modalidade(Id),
    CONSTRAINT Fk_IdPlano_MatriculaCliente FOREIGN KEY(IdPlano) REFERENCES Plano(Id),
    CONSTRAINT Chk_StatusSituacao CHECK(StatusSituacao LIKE '[aA]' OR StatusSituacao LIKE '[iI]')
);
GO

CREATE TABLE Treino
(
    Id INT IDENTITY,
    IdAcademia INT NOT NULL,
    IdInstrutor INT NOT NULL,
    IdCliente INT NOT NULL,
    Nome VARCHAR(250) NOT NULL,
    Observacao VARCHAR(500),
    DataCriacao DATE NOT NULL DEFAULT GETDATE(),
    Ativo CHAR(1) NOT NULL DEFAULT 'A',

    CONSTRAINT Pk_IdTreino PRIMARY KEY(Id),
    CONSTRAINT Fk_IdAcademia_Treino FOREIGN KEY(IdAcademia) REFERENCES Academia(Id),
    CONSTRAINT Fk_IdInstrutor_Treino FOREIGN KEY(IdInstrutor) REFERENCES Usuario(Id),
    CONSTRAINT Fk_IdCliente_Treino FOREIGN KEY(IdCliente) REFERENCES Cliente(Id),
    CONSTRAINT Chk_AtivoTreino CHECK(Ativo LIKE '[aA]' OR Ativo LIKE '[iI]')
);
GO

CREATE TABLE TreinoExercicio
(
    Id INT IDENTITY,
    IdTreino INT NOT NULL,
    IdExercicio INT NOT NULL,
    Series INT NOT NULL,
    Repeticoes INT NOT NULL,
    CargaKg DECIMAL(5,2),
    Observacao VARCHAR(255),
    Ordem INT NOT NULL,

    CONSTRAINT Pk_IdTreinoExercicio PRIMARY KEY(Id),
    CONSTRAINT Fk_IdTreino_TreinoExercicio FOREIGN KEY(IdTreino) REFERENCES Treino(Id),
    CONSTRAINT Fk_IdExercicio_TreinoExercicio FOREIGN KEY(IdExercicio) REFERENCES Exercicio(Id)
);
GO

CREATE TABLE Evento
(
    Id INT IDENTITY,
    IdAcademia INT NOT NULL,
    IdUsuario INT NOT NULL,
    Titulo VARCHAR(255) NOT NULL,
    Descricao VARCHAR(MAX),
    DataInicio DATETIME NOT NULL,
    DataFim DATETIME,
    Ativo CHAR(1) NOT NULL DEFAULT 'A',

    CONSTRAINT Pk_IdEvento PRIMARY KEY(Id),
    CONSTRAINT Fk_IdAcademia_Evento FOREIGN KEY(IdAcademia) REFERENCES Academia(Id),
    CONSTRAINT Fk_IdUsuario_Evento FOREIGN KEY(IdUsuario) REFERENCES Usuario(Id),
    CONSTRAINT Chk_AtivoEvento CHECK(Ativo LIKE '[aA]' OR Ativo LIKE '[iI]')
);
GO

CREATE TABLE MetodoPagamento
(
    Id INT IDENTITY,
    Nome VARCHAR(255) NOT NULL,

    CONSTRAINT Pk_IdMetodoPagamento PRIMARY KEY(Id)
);
GO

CREATE TABLE StatusPagamento
(
    Id INT IDENTITY,
    Nome VARCHAR(255) NOT NULL,

    CONSTRAINT Pk_IdStatusPagamento PRIMARY KEY(Id)
);
GO

CREATE TABLE Pagamento
(
    Id INT IDENTITY,
    IdMatriculaCliente INT NOT NULL,
    IdMetodoPagamento INT,
    IdStatusPagamento INT NOT NULL,
    ValorPago DECIMAL(10,2) NOT NULL,
    DataVencimento DATE NOT NULL,
    DataPagamento DATE,

    CONSTRAINT Pk_IdPagamento PRIMARY KEY(Id),
    CONSTRAINT Fk_IdMatriculaCliente_Pagamento FOREIGN KEY(IdMatriculaCliente) REFERENCES MatriculaCliente(Id),
    CONSTRAINT Fk_IdMetodoPagamento_Pagamento FOREIGN KEY(IdMetodoPagamento) REFERENCES MetodoPagamento(Id),
    CONSTRAINT Fk_IdStatusPagamento_Pagamento FOREIGN KEY(IdStatusPagamento) REFERENCES StatusPagamento(Id)
);
GO

CREATE TABLE AuditoriaLog
(
    Id BIGINT IDENTITY,
    IdAcademia INT NOT NULL,
    IdUsuario INT,
    TabelaAfetada VARCHAR(50) NOT NULL,
    RegistroId INT NOT NULL,
    Acao VARCHAR(10) NOT NULL,
    CampoAlterado VARCHAR(50),
    ValorAnterior VARCHAR(255),
    ValorNovo VARCHAR(255),
    DataHora DATETIME NOT NULL DEFAULT GETDATE(),
    IpOrigem VARCHAR(45),

    CONSTRAINT Pk_IdAuditoriaLog PRIMARY KEY(Id),
    CONSTRAINT Fk_IdAcademia_Auditoria FOREIGN KEY(IdAcademia) REFERENCES Academia(Id),
    CONSTRAINT Chk_AcaoAuditoria CHECK(Acao IN ('INSERT', 'UPDATE', 'DELETE'))
);
GO