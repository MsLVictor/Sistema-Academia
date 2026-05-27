USE SistemaAcademia;
GO

-- 1. Academias (10 Unidades)
INSERT INTO Academia (Nome, CNPJ) VALUES 
('Iron Fitness Centro', '12345678000101'), ('Iron Fitness Sul', '12345678000102'),
('Iron Fitness Norte', '12345678000103'), ('CrossLife Unidade 1', '12345678000104'),
('CrossLife Unidade 2', '12345678000105'), ('BioBody Prime', '12345678000106'),
('BioBody Express', '12345678000107'), ('Arena Beach Tennis', '12345678000108'),
('Studio Pilates Viver', '12345678000109'), ('Academia Municipal', '12345678000110');

-- 2. Cargos
INSERT INTO Cargo (Nome, Descricao) VALUES 
('Administrador', 'Acesso total ao sistema'), ('Recepcionista', 'Atendimento e matrículas'),
('Instrutor Musculação', 'Prescrição de treinos'), ('Nutricionista', 'Avaliação dietética'),
('Serviços Gerais', 'Manutenção da unidade');

-- 3. Modalidades
INSERT INTO Modalidade (Nome, ValorModalidade) VALUES 
('Musculação', 90.00), ('Crossfit', 180.00), ('Natação', 150.00), 
('Pilates', 200.00), ('Zumba', 80.00), ('Yoga', 120.00);

-- 4. Planos
INSERT INTO Plano (Nome, TempoPlano, PercentualDesconto) VALUES 
('Mensal', '01', 0), ('Trimestral', '03', 5), ('Semestral', '06', 10), ('Anual', '12', 20);

-- 5. Métodos e Status de Pagamento
INSERT INTO MetodoPagamento (Nome) VALUES ('Dinheiro'), ('Cartão Crédito'), ('Cartão Débito'), ('PIX'), ('Boleto');
INSERT INTO StatusPagamento (Nome) VALUES ('Pendente'), ('Pago'), ('Atrasado'), ('Cancelado');

-- 6. Usuarios (Funcionários) - Exemplo de loop para gerar volume
DECLARE @i INT = 1;
WHILE @i <= 50
BEGIN
    INSERT INTO Usuario (IdAcademia, IdCargo, Nome, CPF, Email)
    VALUES (ABS(CHECKSUM(NEWID())) % 10 + 1, ABS(CHECKSUM(NEWID())) % 5 + 1, 
            'Funcionario ' + CAST(@i AS VARCHAR), 
            CAST(10000000000 + @i AS CHAR(11)), 'func' + CAST(@i AS VARCHAR) + '@academia.com');
    SET @i = @i + 1;
END

-- 7. Clientes (Vamos gerar 100+)
SET @i = 1;
WHILE @i <= 120
BEGIN
    INSERT INTO Cliente (IdUsuario, IdAcademia, Nome, CPF, DataNascimento, Email)
    VALUES (ABS(CHECKSUM(NEWID())) % 50 + 1, ABS(CHECKSUM(NEWID())) % 10 + 1, 
            'Cliente Aluno ' + CAST(@i AS VARCHAR), 
            CAST(20000000000 + @i AS CHAR(11)), 
            DATEADD(DAY, -(ABS(CHECKSUM(NEWID())) % 15000 + 6000), GETDATE()), 
            'cliente' + CAST(@i AS VARCHAR) + '@email.com');
    SET @i = @i + 1;
END

-- 8. Matriculas (Relacionando clientes a modalidades e planos)
SET @i = 1;
WHILE @i <= 120
BEGIN
    INSERT INTO MatriculaCliente (IdCliente, IdModalidade, IdPlano, DataInicio, StatusSituacao)
    VALUES (@i, ABS(CHECKSUM(NEWID())) % 6 + 1, ABS(CHECKSUM(NEWID())) % 4 + 1, GETDATE(), 'A');
    SET @i = @i + 1;
END
GO

-- 1. Populando Telefone (2 telefones para cada um dos 120 clientes)
DECLARE @i INT = 1;
WHILE @i <= 120
BEGIN
    INSERT INTO Telefone (IdCliente, Telefone)
    VALUES (@i, '(83) 9' + CAST(90000000 + @i AS VARCHAR)),
           (@i, '(83) 32' + CAST(100000 + @i AS VARCHAR));
    SET @i = @i + 1;
END

-- 2. Populando AvaliacaoFisica (1 avaliação para cada cliente com pesos variados)
SET @i = 1;
WHILE @i <= 120
BEGIN
    INSERT INTO AvaliacaoFisica (IdCliente, Peso, Altura, Torax, Cintura, Coxa, Panturrilha, Biceps, DataAvaliacao)
    VALUES (
        @i, 
        (60 + (ABS(CHECKSUM(NEWID())) % 50)), -- Peso entre 60kg e 110kg
        (1.55 + (CAST(ABS(CHECKSUM(NEWID())) % 45 AS FLOAT) / 100)), -- Altura entre 1.55m e 2.00m
        90.0, 80.0, 55.0, 38.0, 35.0,
        DATEADD(DAY, -(ABS(CHECKSUM(NEWID())) % 30), '2026-04-01') -- Datas em Março/Abril de 2026
    );
    SET @i = @i + 1;
END
GO

-- 1. Garante que os Status de Pagamento existem
IF NOT EXISTS (SELECT 1 FROM StatusPagamento)
BEGIN
    INSERT INTO StatusPagamento (Nome) VALUES ('Pendente'), ('Pago'), ('Atrasado'), ('Cancelado');
END

-- 2. Gera Pagamentos para as 120 Matrículas (Simulando 3 meses para cada)
DECLARE @m INT = 1; -- Contador de Matrícula
DECLARE @mes INT = 0; -- Contador de meses atrás

WHILE @m <= 120
BEGIN
    SET @mes = 0;
    WHILE @mes < 3 -- Gera 3 mensalidades para cada aluno
    BEGIN
        INSERT INTO Pagamento (IdMatriculaCliente, IdMetodoPagamento, IdStatusPagamento, ValorPago, DataVencimento, DataPagamento)
        VALUES (
            @m, 
            (ABS(CHECKSUM(NEWID())) % 5 + 1), -- Método aleatório
            CASE 
                WHEN @mes = 0 THEN 2 -- O mais antigo está 'Pago'
                WHEN @mes = 1 THEN (ABS(CHECKSUM(NEWID())) % 2 + 2) -- 'Pago' ou 'Atrasado'
                ELSE 1 -- O mais recente está 'Pendente'
            END,
            (SELECT m.ValorModalidade FROM Modalidade m INNER JOIN MatriculaCliente mc ON m.Id = mc.IdModalidade WHERE mc.Id = @m),
            DATEADD(MONTH, -@mes, GETDATE()), -- Vencimentos retroativos
            CASE 
                WHEN @mes < 2 THEN DATEADD(DAY, -2, DATEADD(MONTH, -@mes, GETDATE())) -- Pagou 2 dias antes do vencimento
                ELSE '1900-01-01' -- Se for pendente, a data fica padrão (ou trate como null se seu banco permitir)
            END
        );
        SET @mes = @mes + 1;
    END
    SET @m = @m + 1;
END
GO
