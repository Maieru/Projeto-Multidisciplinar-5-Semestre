CREATE PROCEDURE spInsertLogWeb
	@Id				INT, 
	@Mensagem		VARCHAR(1000),
	@Callstack		VARCHAR(1000),
	@TipoOperacao	INT,
	@Detalhes		VARCHAR(1000),
	@DataGeracao	DATETIME
AS
BEGIN
	INSERT INTO tbLogWeb(Id, Mensagem, Callstack, TipoOperacao, Detalhes, DataGeracao)
				 VALUES (@Id, @Mensagem, @Callstack, @TipoOperacao, @Detalhes, @DataGeracao)

	RETURN 0
END
GO

-- Não se faz update de Log

CREATE PROCEDURE spDeleteLogWeb
	@Id				INT
AS
BEGIN
	DELETE tbLogWeb
	WHERE Id = @Id

	RETURN 0
END
GO

CREATE PROCEDURE spSelectLogWeb
	@Id				INT
AS
BEGIN
	SELECT * 
	FROM tbLogWeb
	WHERE Id = @Id
END
GO

CREATE PROCEDURE spListLogWeb
AS
BEGIN
	SELECT * 
	FROM tbLogWeb
	ORDER BY DataGeracao DESC
END
GO

CREATE PROCEDURE spSearchLogWeb
(
	@TipoLog		VARCHAR(MAX), 
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
	FROM tbLogWeb
	WHERE TipoOperacao LIKE CONCAT('%', ISNULL(@TipoLog, ''), '%') AND
		  DataGeracao > CAST(ISNULL(@DataInicial, '1800-01-1') AS DATETIME) AND
		  DataGeracao < CAST(ISNULL(@DataFinal, '9999-12-31') AS DATETIME)
	ORDER BY DataGeracao DESC
END
GO

CREATE NONCLUSTERED INDEX IX_DATALOG_LOGWEB
    ON tbLogWeb (DataGeracao);
GO