create database SistemaAcademia;

USE SistemaAcademia;

CREATE TABLE Usuario
(
	Id INT IDENTITY,
	Nome VARCHAR(150) NOT NULL,
	CPF CHAR(11) NOT NULL,
	Email VARCHAR(250) NOT NULL,
	Cargo VARCHAR(150)NOT NULL,

	CONSTRAINT Pk_IdUsuario PRIMARY KEY(Id)
)

CREATE TABLE Cliente
(
	Id INT IDENTITY,
	IdUsuario INT NOT NULL,
	Nome VARCHAR(250) NOT NULL,
	CPF CHAR(11) NOT NULL,
	DataNascimento DATE NOT NULL,
	Email VARCHAR(250) NOT NULL,

	CONSTRAINT Pk_IdCliente PRIMARY KEY(Id),
	CONSTRAINT FK_IdUsuario FOREIGN KEY(IdUsuario) REFERENCES Usuario(Id)
)

CREATE TABLE Telefone
(
	Id INT IDENTITY,
	IdCliente INT NOT NULL,
	Telefone VARCHAR(15) NOT NULL,
	TelefoneOpciocional VARCHAR(15),

	CONSTRAINT PK_IdTelefone PRIMARY KEY(Id),
	CONSTRAINT Fk_IdCliente_Telefone FOREIGN KEY(IdCliente) REFERENCES Cliente(Id)
)

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
)

CREATE TABLE Modalidade 
(
	Id INT IDENTITY,
	Nome VARCHAR(250) NOT NULL,
	ValorModalidade DECIMAL(10,2) NOT NULL,

	CONSTRAINT Pk_IdModalidade PRIMARY KEY(Id)
)

CREATE TABLE Plano
(
	Id INT IDENTITY,
	Nome VARCHAR(255) NOT NULL,
	TempoPlano CHAR(2) NOT NULL,
	PercentualDesconto FLOAT NOT NULL,

	CONSTRAINT Pk_IdPlano PRIMARY KEY(Id)
);

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
)


CREATE TABLE MetodoPagamento(
	Id INT IDENTITY,
	Nome VARCHAR(255) NOT NULL,

	CONSTRAINT Pk_IdMetodoPagamento PRIMARY KEY(Id)
)

CREATE TABLE StatusPagamento
(
	Id INT IDENTITY,
	Nome VARCHAR(255),

	CONSTRAINT Pk_IdStatusPagamento PRIMARY KEY(Id)
)

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

CREATE TABLE TabelaTeste (
    Id INT IDENTITY PRIMARY KEY,
    Mensagem VARCHAR(255) NOT NULL,
    DataTeste DATETIME DEFAULT GETDATE()
);

INSERT INTO Usuario(Nome, CPF, Email, Cargo)
VALUES('Victor Melquiades Leite', '09769920436', 'victormleite25@gmail.com', 'Admin');

INSERT INTO Cliente(IdUsuario, Nome, CPF, DataNascimento, Email)
VALUES
(1, 'Adriano Imperador', '11122233344', '1982-02-17', 'didico@email.com'),
(1, 'Ronaldinho Gaucho', '22233344455', '1980-03-21', 'bruxo@email.com'),
(1, 'Enzo Gabriel', '33344455566', '2010-05-10', 'enzo@email.com'),
(1, 'Valentina Silva', '44455566677', '2012-08-15', 'valen@email.com'),
(1, 'Marta Vieira', '55566677788', '1986-02-19', 'rainha@email.com'),
(1, 'Claudio Pitbull', '66677788899', '1982-01-08', 'pitbull@email.com'),
(1, 'Juliana Paes', '77788899900', '1979-03-26', 'ju@email.com'),
(1, 'Sérgio Moro', '88899900011', '1972-08-01', 'moro@email.com'),
(1, 'Fausto Silva', '99900011122', '1950-05-02', 'faustao@email.com'),
(1, 'Anitta Poderosa', '00011122233', '1993-03-30', 'anitta@email.com'),
(1, 'Casimiro Miguel', '12121212121', '1993-10-20', 'caze@email.com'),
(1, 'Paola Carosella', '23232323232', '1972-10-30', 'paola@email.com'),
(1, 'Seu Jorge', '34343434343', '1970-06-08', 'seujorge@email.com'),
(1, 'Gisele Bundchen', '45454545454', '1980-07-20', 'gisele@email.com'),
(1, 'Wagner Moura', '56565656565', '1976-06-27', 'capitao@email.com');


INSERT INTO MatriculaCliente(IdCliente, IdModalidade, IdPlano, DataInicio, StatusSituacao) VALUES
(1, 1, 1, '2023-01-10', 'A'),
(3, 1, 2, '2023-02-15', 'A'),
(4, 2, 1, '2023-03-20', 'I'), -- Inativo
(5, 2, 2, '2023-04-25', 'A'),
(6, 1, 1, '2023-05-30', 'A'),
(7, 1, 2, '2023-06-05', 'I'), -- Inativo
(8, 2, 1, '2023-07-10', 'A'),
(9, 2, 2, '2023-08-15', 'A'),
(10, 1, 1, '2023-09-20', 'A'),
(11, 1, 2, '2023-10-25', 'A'),
(12, 2, 1, '2023-11-30', 'I'), -- Inativo
(13, 2, 2, '2023-12-05', 'A'),
(14, 1, 1, '2024-01-10', 'A'),
(15, 1, 2, '2024-02-15', 'A'),
(16, 2, 1, '2024-03-20', 'A');


INSERT INTO Pagamento (IdMatriculaCliente, ValorPago, DataVencimento, DataPagamento, IdMetodoPagamento, IdStatusPagamento) VALUES
(1, 80.00,  '2024-04-10', GETDATE(), 1, 1), -- Pago
(3, 150.00, '2024-04-15', GETDATE(), 1, 1), -- Pago
(4, 80.00,  '2024-03-20', '20240320', 1, 1), -- Pago (está inativo mas pagou o que devia)
(5, 150.00, '2024-05-25', GETDATE(), 1, 2), -- PENDENTE
(6, 80.00,  '2024-05-30', GETDATE(), 1, 2), -- PENDENTE
(7, 150.00, '2024-04-05', GETDATE(), 1, 1), -- Pago
(8, 80.00,  '2024-05-10', GETDATE(), 1, 2), -- PENDENTE
(9, 150.00, '2024-05-15', GETDATE(), 1, 2), -- PENDENTE
(10, 80.00, '2024-04-20', GETDATE(), 1, 1), -- Pago
(11, 150.00, '2024-04-25', GETDATE(), 1, 1), -- Pago
(12, 80.00, '2024-03-30', GETDATE(), 1, 2), -- PENDENTE
(13, 150.00, '2024-05-05', GETDATE(), 1, 2), -- PENDENTE
(14, 80.00, '2024-04-10', GETDATE(), 1, 1), -- Pago
(15, 150.00, '2024-04-15', GETDATE(), 1, 1), -- Pago
(16, 80.00, '2024-05-20', GETDATE(), 1, 2); -- PENDENTE

INSERT INTO Telefone (IdCliente, Telefone, TelefoneOpciocional) VALUES
(1,  '83911111111', '8332221111'), -- Nilvan
(2,  '21999998888', NULL),           -- Adriano
(3,  '51988887777', '5133334444'), -- Ronaldinho
(4,  '11977776666', NULL),           -- Enzo
(5,  '11966665555', '1132321010'), -- Valentina
(6,  '71955554444', NULL),           -- Marta
(7,  '21944443333', '2125252525'), -- Claudio Pitbull
(8,  '11933332222', NULL),           -- Juliana Paes
(9,  '41922221111', '4130303030'), -- Sérgio Moro
(10, '11911110000', NULL),           -- Fausto Silva
(11, '21900009999', '2121212121'), -- Anitta
(12, '21987654321', NULL),           -- Casimiro
(13, '11912345678', '1130304040'), -- Paola
(14, '21955556666', NULL),           -- Seu Jorge
(15, '11944445555', NULL),           -- Gisele
(16, '71933334444', '7133445566');


INSERT INTO Modalidade(Nome, ValorModalidade)
VALUES('Musculacao', 70)

INSERT INTO Modalidade(Nome, ValorModalidade)
VALUES('Danca', 70)

INSERT INTO MetodoPagamento(Nome)
values('Dinheiro')

INSERT INTO StatusPagamento(Nome)
VALUES('PAGO')

INSERT INTO StatusPagamento(Nome)
VALUES('PENDENTE')

INSERT INTO StatusPagamento(Nome)
VALUES('CANCELADO')

DELETE FROM MatriculaCliente WHERE Id = 3




SELECT * FROM Usuario

SELECT * FROM MatriculaCliente

SELECT * FROM Cliente

SELECT * FROM TabelaTeste

SELECT * from Plano;

DELETE FROM Plano WHERE Id = 2 

SELECT
	C.Nome AS Cliente,
	M.Nome AS Modalidade,
	P.Nome AS Plano,
	Pag.ValorPago,
	S.Nome AS StatusPagamento
FROM MatriculaCliente MC
JOIN Cliente C ON MC.IdCliente = C.Id
JOIN Modalidade M ON MC.IdModalidade = M.Id
JOIN Plano P ON MC.IdPlano = P.Id
JOIN Pagamento Pag ON Pag.IdMatriculaCliente = MC.Id
JOIN StatusPagamento S ON Pag.IdStatusPagamento = S.Id

SELECT 
	C.Nome,
	M.Nome,
	P.Nome,
	Pag.ValorPago
FROM MatriculaCliente MC
JOIN Cliente C on MC.IdCliente = C.Id
JOIN Modalidade M on MC.IdModalidade = M.Id
JOIN Plano P on MC.IdPlano = P.Id
JOIN Pagamento Pag ON Pag.IdMatriculaCliente = MC.Id

SELECT
	C.Nome AS Cliente
FROM MatriculaCliente MC
JOIN Cliente C on MC.IdCliente = C.Id
WHERE MC.StatusSituacao = 'I';

