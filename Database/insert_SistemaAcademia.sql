USE SistemaAcademia;
GO

INSERT INTO Cargo (Nome) VALUES
    ('Administrador'),
    ('Gerente'),
    ('Recepcionista'),
    ('Instrutor');
GO

INSERT INTO OrientacaoSexual (Nome) VALUES
    ('Heterossexual'),
    ('Homossexual'),
    ('Bissexual'),
    ('Outro'),
    ('Prefiro não informar');
GO

INSERT INTO StatusPagamento (Nome) VALUES
    ('Pendente'),
    ('Pago'),
    ('Atrasado'),
    ('Cancelado');
GO

INSERT INTO MetodoPagamento (Nome) VALUES
    ('Pix'),
    ('Cartão de Crédito'),
    ('Cartão de Débito'),
    ('Boleto'),
    ('Dinheiro');
GO