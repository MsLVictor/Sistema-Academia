
SELECT * FROM Usuario

SELECT * FROM Cargo

SELECT * FROM MatriculaCliente

SELECT * FROM Cliente

select * FROM Plano;



--5 
SELECT	cl.Nome as 'Nome Cliente',
		mo.Nome as 'Modalidade',
		pl.Nome as 'Plano'
	FROM MatriculaCliente as mc
		JOIN Cliente AS cl
			ON mc.IdCliente = cl.Id
		JOIN Modalidade AS mo
			ON mc.IdModalidade = mo.Id
		JOIN Plano AS pl
			ON mc.IdModalidade = pl.Id

--1 listar usuario nome da academia onde trabalha e seu cargo
SELECT	us.Nome as 'Funcionário',
		ca.Nome as 'Cargo',
		ac.Nome as 'Academia'
	FROM Usuario AS us
		INNER JOIN Cargo AS ca
			ON us.IdCargo = ca.Id
		INNER JOIN Academia AS ac
			ON us.IdAcademia = ac.Id
--2 Liste o nome de cliente e nome de modalidade que ele está praticando
SELECT	cl.Nome as 'Cliente',
		mo.Nome as 'Modalidade'
	FROM MatriculaCliente AS mc
		INNER JOIN Cliente AS cl
			ON mc.IdCliente = cl.Id
		INNER JOIN Modalidade AS mo
			ON mc.IdModalidade = mo.Id
--3 Liste o nome do cliente e seu telefone
SELECT	cl.Nome as 'Cliente',
		te.Telefone
	FROM Telefone AS te
		INNER JOIN Cliente AS cl
			ON te.IdCliente = cl.Id

--4
SELECT	mo.Nome as 'Modalidade',
		SUM(mo.ValorModalidade)
	FROM MatriculaCliente AS mc
		INNER JOIN Modalidade AS mo
			ON mc.IdModalidade = mo.Id
	GROUP BY mo.Nome

--5
SELECT	cl.Nome as 'Cliente',
		af.Peso
	FROM AvaliacaoFisica AS af
		INNER JOIN Cliente AS cl
			ON af.IdCliente = cl.Id
	WHERE YEAR(af.DataAvaliacao) = YEAR(GETDATE())
--6
SELECT	ac.Nome as 'Academia',
		COUNT(cl.Id) as 'Quantidade Clientes'
	FROM Cliente AS cl
		INNER JOIN Academia AS ac
			ON cl.IdAcademia = ac.Id
	GROUP BY ac.Nome

--7
SELECT	cl.Nome as 'Cliente',
		af.Peso as 'Peso'
	FROM AvaliacaoFisica AS af
		INNER JOIN Cliente AS cl
			ON af.IdCliente = cl.Id
	WHERE af.Peso > (
		SELECT AVG(Peso)
		FROM AvaliacaoFisica
	)

--8
SELECT	pl.Nome as 'Plano',
		COUNT(cl.Id) as 'Quantidade Clientes'
	FROM MatriculaCliente AS mc
		INNER JOIN Plano AS pl
			ON mc.IdPlano = pl.Id
		INNER JOIN Cliente AS cl
			ON mc.IdCliente = cl.Id
	GROUP BY pl.Nome
	HAVING COUNT(cl.Id) = (
		SELECT MAX(Contagem)
			FROM (
				SELECT COUNT(Id) AS Contagem
					FROM MatriculaCliente
					GROUP BY IdPlano
		) AS Rascunho
	)
	
-- 8 com sub query
SELECT	pl.Nome as 'Plano',
		COUNT(cl.Id) as 'Quantidade Clientes'
	FROM MatriculaCliente AS mc
		INNER JOIN Cliente AS cl
			ON mc.IdCliente = cl.Id
		INNER JOIN Plano AS pl
			ON mc.IdPlano = pl.Id
	GROUP BY pl.Nome
	HAVING COUNT(cl.Id) = (
		SELECT MAX(Contagem)
			FROM (
				SELECT COUNT(Id) as Contagem
				FROM MatriculaCliente
				GROUP BY IdPlano
			) AS Rascunho
	)

--9
SELECT cl.Nome as 'Cliente'
	FROM Cliente AS cl
		INNER JOIN MatriculaCliente AS mc
			ON mc.IdCliente = cl.Id
	WHERE NOT EXISTS (
		SELECT 1
			FROM Pagamento AS pa
			WHERE pa.IdMatriculaCliente = mc.Id
	);

--10
SELECT	ac.Nome as 'Academia',
		SUM(pa.ValorPago) as 'Total Pagamentos'
	FROM Academia AS ac
		INNER JOIN Cliente AS cl
			ON cl.IdAcademia = ac.Id
		INNER JOIN MatriculaCliente AS mc
			ON mc.IdCliente = cl.Id
		INNER JOIN Pagamento AS pa
			ON pa.IdMatriculaCliente = mc.Id
	GROUP BY ac.Nome
	HAVING SUM(pa.ValorPago) > (
		SELECT AVG(Rascunho.SomaFaturamento)
			FROM (
				SELECT SUM(ValorPago) AS SomaFaturamento
					FROM Pagamento AS pa
						INNER JOIN MatriculaCliente AS mc
							ON pa.IdMatriculaCliente = mc.Id
						INNER JOIN Cliente AS cl
							ON mc.IdCliente = cl.Id
						GROUP BY cl.IdAcademia
			) AS Rascunho
	);


	--1

	SELECT	cl.Nome as 'Cliente',
			pl.Nome as 'Plano'
		FROM Cliente AS cl
			INNER JOIN MatriculaCliente AS mc
				ON mc.IdCliente = cl.Id
			INNER JOIN Plano AS pl
				ON mc.IdPlano = pl.Id
		WHERE mc.IdPlano = (
			SELECT	TOP 1 pl.Id
				FROM Cliente AS cl
					INNER JOIN MatriculaCliente AS mc
						ON mc.IdCliente = cl.Id
					INNER JOIN Plano AS pl
						ON mc.IdPlano = pl.Id
		)
--2
SELECT	mo.Nome as 'Modalidade',
		mo.ValorModalidade
	FROM Modalidade AS mo
	WHERE mo.ValorModalidade > (
		SELECT ValorModalidade
			FROM Modalidade
			WHERE Modalidade.Nome = 'Zumba'
	)

--3