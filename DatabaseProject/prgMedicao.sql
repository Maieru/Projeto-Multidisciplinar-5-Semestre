CREATE PROCEDURE spInsertMedicao
	@Id				INT,
	@DispositivoId	INT, 
	@DataMedicao	DATETIME,
	@ValorChuva		FLOAT,
	@ValorNivel		FLOAT
AS
BEGIN
	INSERT INTO tbMedicao(DispositivoId, DataMedicao, ValorChuva, ValorNivel)
				  VALUES (@DispositivoId, @DataMedicao, @ValorChuva, @ValorNivel)

	RETURN 0
END
GO

CREATE PROCEDURE spDeleteMedicao
	@Id				INT
AS
BEGIN
	DELETE tbMedicao
	WHERE id = @Id

	RETURN 0
END
GO

CREATE PROCEDURE spSelectMedicao
	@Id				INT
AS
BEGIN
	SELECT *
	FROM tbMedicao
	WHERE Id = @Id
END
GO


CREATE PROCEDURE spListMedicao
AS
BEGIN
	SELECT * 
	FROM tbMedicao
	ORDER BY DataMedicao DESC
END
GO

CREATE PROCEDURE spSearchMedicao
(
	@DispositivoId	VARCHAR(MAX), 
	@DataInicial	VARCHAR(MAX),
	@DataFinal		VARCHAR(MAX)
)
AS
BEGIN
	IF @DataInicial = ''
		SET @DataInicial = NULL

	IF @DataFinal = ''
		SET @DataFinal = NULL

	SELECT *
	FROM tbMedicao
	WHERE DispositivoId LIKE CONCAT('%', ISNULL(@DispositivoId, ''), '%') AND
		  DataMedicao > CAST(ISNULL(@DataInicial, '1800-01-1') AS DATETIME) AND
		  DataMedicao < CAST(ISNULL(@DataFinal, '9999-12-31') AS DATETIME)
	ORDER BY DataMedicao DESC
END
GO

----------------------------------------// Dashboard //----------------------------------------

-- Dispositivos
CREATE FUNCTION fncSelecionaMedicaoUltimoDiaPorDispositivo
(
	@DispositivoId	INT
)
RETURNS TABLE AS
RETURN
(
	SELECT DATEPART(HOUR, dataMedicao) [ParteDataMedicao],
		   AVG(ValorChuva)			   [MediaChuva],
		   AVG(ValorNivel)			   [MediaNivel]
	FROM tbMedicao
	WHERE DispositivoId = @DispositivoId AND
		  DataMedicao >= DATEADD(HOUR, -24, GETDATE())
	GROUP BY DATEPART(HOUR, DataMedicao)
)
GO

CREATE PROCEDURE spSelecionaMedicaoUltimoDiaPorDispositivo
(
	@DispositivoId	INT
)
AS
BEGIN
	SELECT *
	FROM fncSelecionaMedicaoUltimoDiaPorDispositivo(@DispositivoId)
END
GO

CREATE FUNCTION fncSelecionaMedicaoUltimoMesPorDispositivo
(
	@DispositivoId	INT
)
RETURNS TABLE AS
RETURN
(
	SELECT DATEPART(DAY, dataMedicao)  [ParteDataMedicao],
		   AVG(ValorChuva)			   [MediaChuva],
		   AVG(ValorNivel)			   [MediaNivel]
	FROM tbMedicao
	WHERE DispositivoId = @DispositivoId AND
		  DataMedicao >= DATEADD(MONTH, -1, GETDATE())
	GROUP BY DATEPART(DAY, DataMedicao)
)
GO

CREATE PROCEDURE spSelecionaMedicaoUltimoMesPorDispositivo
(
	@DispositivoId	INT
)
AS
BEGIN
	SELECT *
	FROM fncSelecionaMedicaoUltimoMesPorDispositivo(@DispositivoId)
END
GO

----------------------------------------// Trigger //----------------------------------------

GO
CREATE TRIGGER trgInclusaoMedicoes ON tbMedicao
INSTEAD OF INSERT AS
BEGIN
	-- Esta trigger tem a função de validar os dados inseridos nas medições, garantidno que os mesmos
	-- são condizentes. Caso uma medição seja suspeita, deve ser adicionado a tabela de medições suspeitas

	DECLARE @medicaoValida BIT
	
	DECLARE @DispositivoId	INT
	DECLARE @DataMedicao	DATETIME
	DECLARE @ValorChuva		FLOAT
	DECLARE @ValorNivel		FLOAT

	DECLARE cursorMedicoes CURSOR STATIC FORWARD_ONLY FOR
		SELECT DispositivoId, DataMedicao, ValorChuva, ValorNivel
		FROM inserted

	OPEN cursorMedicoes

	FETCH NEXT FROM cursorMedicoes
		INTO @DispositivoId, @DataMedicao, @ValorChuva, @ValorNivel

	WHILE(@@FETCH_STATUS = 0)
	BEGIN
		SET @medicaoValida = 1
		DECLARE @ValorReferenciaDispositivo FLOAT

		SET @ValorReferenciaDispositivo = (SELECT MedicaoReferencia FROM tbDispositivos WHERE Id = @DispositivoId)

		IF @ValorNivel > @ValorReferenciaDispositivo OR @ValorNivel > 300
		BEGIN
			INSERT INTO tbMedicoesSuspeitas(DispositivoId, DataMedicao, ValorChuva, ValorNivel, Detalhes)
				VALUES (@DispositivoId, @DataMedicao, @ValorChuva, @ValorNivel, 'Adicionado porque o valor de nível ultrapassa limite do sensor')
			  
			SET @medicaoValida = 0
		END

		IF @ValorNivel > (SELECT TOP 10 AVG(ValorNivel) FROM tbMedicao WHERE DispositivoId = @DispositivoId) * 2
		BEGIN
			INSERT INTO tbMedicoesSuspeitas(DispositivoId, DataMedicao, ValorChuva, ValorNivel, Detalhes)
				VALUES (@DispositivoId, @DataMedicao, @ValorChuva, @ValorNivel, 'Adicionado porque o valor de nível era muito superior a média dos ultimos 10 registros')
			  
			SET @medicaoValida = 0
		END

		IF @ValorChuva > (SELECT TOP 10 AVG(ValorChuva) FROM tbMedicao WHERE DispositivoId = @DispositivoId) * 2 + 5 
		BEGIN
			INSERT INTO tbMedicoesSuspeitas(DispositivoId, DataMedicao, ValorChuva, ValorNivel, Detalhes)
				VALUES (@DispositivoId, @DataMedicao, @ValorChuva, @ValorNivel, 'Adicionado porque o valor da chuva era muito superior a média dos ultimos 10 registros')
			
			SET @medicaoValida = 0
		END

		IF @medicaoValida = 1
		BEGIN
			INSERT INTO tbMedicao(DispositivoId, DataMedicao, ValorChuva, ValorNivel)
				VALUES (@DispositivoId, @DataMedicao, @ValorChuva, @ValorNivel)
		END

		FETCH NEXT FROM cursorMedicoes
			INTO @DispositivoId, @DataMedicao, @ValorChuva, @ValorNivel
	END

	CLOSE cursorMedicoes
	DEALLOCATE cursorMedicoes
END
GO