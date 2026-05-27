create database SistemaAcademia;
GO

USE SistemaAcademia;
GO

CREATE TABLE Academia
(
    Id INT IDENTITY,
    Nome VARCHAR(250) NOT NULL,
    CNPJ CHAR(14) NOT NULL,

    CONSTRAINT Pk_IdAcademia PRIMARY KEY(Id),
    CONSTRAINT Uq_CNPJAcademia UNIQUE(CNPJ)
);

CREATE TABLE Cargo (
	Id INT IDENTITY,
	Nome VARCHAR(100) NOT NULL,
	Descricao VARCHAR(1000),

	CONSTRAINT Pk_IdCargo PRIMARY KEY(Id)
);
GO

CREATE TABLE Usuario
(
	Id INT IDENTITY,
	IdAcademia INT NOT NULL,
	IdCargo INT NOT NULL,
	Nome VARCHAR(150) NOT NULL,
	CPF CHAR(11) NOT NULL,
	Email VARCHAR(250) NOT NULL,

	CONSTRAINT PK_IdUsuario PRIMARY KEY(Id),
	CONSTRAINT FK_IdAcademia_Usuario FOREIGN KEY(IdAcademia) REFERENCES Academia(Id),
	CONSTRAINT Fk_IdCargo_Usuario FOREIGN KEY(IdCargo) REFERENCES Cargo(Id),
	CONSTRAINT UQ_CPFUsuario UNIQUE(CPF)
);
GO

CREATE TABLE Cliente
(
	Id INT IDENTITY,
	IdUsuario INT NOT NULL,
	IdAcademia INT NOT NULL,
	Nome VARCHAR(250) NOT NULL,
	CPF CHAR(11) NOT NULL,
	DataNascimento DATE NOT NULL,
	Email VARCHAR(250) NOT NULL,

	CONSTRAINT Pk_IdCliente PRIMARY KEY(Id),
	CONSTRAINT FK_IdUsuario FOREIGN KEY(IdUsuario) REFERENCES Usuario(Id),
	CONSTRAINT FK_IdAcademia_Cliente FOREIGN KEY(IdAcademia) REFERENCES Academia(Id),
	CONSTRAINT UQ_CPFCliente UNIQUE(CPF)
);
GO

CREATE TABLE Telefone
(
	Id INT IDENTITY,
	IdCliente INT NOT NULL,
	Telefone VARCHAR(15) NOT NULL

	CONSTRAINT PK_IdTelefone PRIMARY KEY(Id),
	CONSTRAINT Fk_IdCliente_Telefone FOREIGN KEY(IdCliente) REFERENCES Cliente(Id)
);
GO

CREATE TABLE AvaliacaoFisica
(
	Id INT IDENTITY,
	IdCliente INT NOT NULL,
	Peso FLOAT,
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

CREATE TABLE Modalidade 
(
	Id INT IDENTITY,
	Nome VARCHAR(250) NOT NULL,
	ValorModalidade DECIMAL(10,2) NOT NULL,

	CONSTRAINT Pk_IdModalidade PRIMARY KEY(Id)
);
GO

CREATE TABLE Plano
(
	Id INT IDENTITY,
	Nome VARCHAR(255) NOT NULL,
	TempoPlano CHAR(2) NOT NULL,
	PercentualDesconto FLOAT NOT NULL,

	CONSTRAINT Pk_IdPlano PRIMARY KEY(Id)
);
GO

CREATE TABLE MatriculaCliente
(
	Id INT IDENTITY,
	IdCliente INT NOT NULL,
	IdModalidade INT NOT NULL,
	IdPlano INT NOT NULL,
	DataInicio DATE NOT NULL,
	StatusSituacao CHAR(1) NOT NULL,

	CONSTRAINT Pk_IdMatriculaCliente PRIMARY KEY(Id),
	CONSTRAINT Fk_IdCliente_MatriculaCliente FOREIGN KEY(IdCliente) REFERENCES Cliente(Id),
	CONSTRAINT Fk_IdModalidade_MatriculaCliente FOREIGN KEY(IdModalidade) REFERENCES Modalidade(Id),
	CONSTRAINT Fk_IdPlano_MatriculaCliente FOREIGN KEY(IdPlano) REFERENCES Plano(Id),
	CONSTRAINT Chk_StatusSituacao CHECK(StatusSituacao LIKE '[a,A]' OR StatusSituacao LIKE '[i,I]')
);
GO

CREATE TABLE MetodoPagamento(
	Id INT IDENTITY,
	Nome VARCHAR(255) NOT NULL,

	CONSTRAINT Pk_IdMetodoPagamento PRIMARY KEY(Id)
);
GO

CREATE TABLE StatusPagamento
(
	Id INT IDENTITY,
	Nome VARCHAR(255),

	CONSTRAINT Pk_IdStatusPagamento PRIMARY KEY(Id)
);
GO

CREATE TABLE Pagamento
(
	Id INT IDENTITY,
	IdMatriculaCliente INT NOT NULL,
	IdMetodoPagamento INT NOT NULL,
	IdStatusPagamento INT NOT NULL,
	ValorPago DECIMAL(10,2) NOT NULL,
	DataVencimento DATE NOT NULL,
	DataPagamento DATE NOT NULL,

	CONSTRAINT Pk_IdPagamento PRIMARY KEY(Id),
	CONSTRAINT Fk_IdMatriculaCliente_Pagamento FOREIGN KEY(IdMatriculaCliente) REFERENCES MatriculaCliente(Id),
	CONSTRAINT Fk_IdMetodoPagamento_Pagamento FOREIGN KEY(IdMetodoPagamento) REFERENCES MetodoPagamento(Id),
	CONSTRAINT Fk_IdStatusPagamento_Pagamento FOREIGN KEY(IdStatusPagamento) REFERENCES StatusPagamento(Id)
);
GO
