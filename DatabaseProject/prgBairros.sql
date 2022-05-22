CREATE PROCEDURE spInsertBairros
	@Id				INT, 
	@Descricao		VARCHAR(50),
	@CEP			VARCHAR(20),
	@Latitude		FLOAT,
	@Longitude		FLOAT
AS
BEGIN
	INSERT INTO tbBairros(Id, Descricao, Latitude, Longitude, CEP)
				  VALUES (@Id, @Descricao, @Latitude, @Longitude, @CEP)

	RETURN 0
END
GO

CREATE PROCEDURE spUpdateBairros
	@Id				INT, 
	@Descricao		VARCHAR(50),
	@CEP			VARCHAR(20),
	@Latitude		FLOAT,
	@Longitude		FLOAT
AS
BEGIN
	UPDATE tbBairros
	SET Descricao = @Descricao,
		CEP	= @CEP,
		Latitude = @Latitude,
		Longitude = @Longitude
	WHERE Id = @Id

	RETURN 0
END
GO

CREATE PROCEDURE spDeleteBairros
	@Id				INT
AS
BEGIN
	DELETE tbBairros
	WHERE Id = @Id

	RETURN 0
END
GO

CREATE PROCEDURE spSelectBairros
	@Id				INT
AS
BEGIN
	SELECT * 
	FROM tbBairros
	WHERE Id = @Id
END
GO

CREATE PROCEDURE spListBairros
AS
BEGIN
	SELECT * 
	FROM tbBairros
END
GO

CREATE PROCEDURE spSearchBairros
(
	@Id				VARCHAR(MAX), 
	@Descricao		VARCHAR(50),
	@CEP			VARCHAR(20)
)
AS
BEGIN
	SELECT *
	FROM tbBairros
	WHERE Id LIKE CONCAT('%', ISNULL(@Id, ''), '%') AND
		  Descricao LIKE CONCAT('%', ISNULL(@Descricao, ''), '%') AND
		  CEP LIKE CONCAT('%', ISNULL(@CEP, ''), '%') 
END
GO

CREATE PROCEDURE spGetLatestMesureFromBairros
AS
BEGIN
	SELECT b.*,
	   (SELECT TOP 1 m.ValorChuva
		FROM tbMedicao m
		INNER JOIN tbDispositivos d on D.Id = m.DispositivoId
		WHERE d.BairroID = b.Id
		ORDER BY m.DataMedicao DESC) [ValorChuva],
		(SELECT TOP 1 m.ValorNivel
		FROM tbMedicao m
		INNER JOIN tbDispositivos d on D.Id = m.DispositivoId
		WHERE d.BairroID = b.Id
		ORDER BY m.DataMedicao DESC) [ValorNivel]
	FROM tbBairros b
END