CREATE PROCEDURE spInsertSubscriberAlerta
	@Id					INT, 
	@BairroId			INT,
	@Telefone			VARCHAR(100),
	@Email				VARCHAR(200),
	@UsuarioId			INT,
	@UltimaNotificacao	INT
AS
BEGIN
	IF NOT EXISTS (SELECT * FROM tbBairros WHERE Id = @BairroId)
		RAISERROR ( 'O bairro informado não existe.', 16, 1)

	INSERT INTO tbSubscriberAlerta (Id, BairroId, Telefone, Email, UsuarioId, UltimaNotificacao)
						    VALUES (@Id, @BairroId, @Telefone, @Email, @UsuarioId, NULL)

	RETURN 0
END
GO

CREATE PROCEDURE spUpdateSubscriberAlerta
	@Id				INT, 
	@BairroId		INT,
	@Telefone		VARCHAR(100),
	@Email			VARCHAR(200),
	@UsuarioId		INT,
	@UltimaNotificacao	INT
AS
BEGIN
	IF NOT EXISTS (SELECT * FROM tbBairros WHERE Id = @BairroId)
		RAISERROR ( 'O bairro informado não existe.', 16, 1)

	UPDATE tbSubscriberAlerta
	SET BairroId = @BairroId,
		Telefone = @Telefone,
		Email = @Email,
		UsuarioId = @UsuarioId,
		UltimaNotificacao = @UltimaNotificacao
	WHERE Id = @Id

	RETURN 0
END
GO

CREATE PROCEDURE spDeleteSubscriberAlerta
	@Id				INT
AS
BEGIN
	DELETE tbSubscriberAlerta
	WHERE Id = @Id

	RETURN 0
END
GO

CREATE PROCEDURE spSelectSubscriberAlerta
	@Id				INT
AS
BEGIN
	SELECT * 
	FROM tbSubscriberAlerta
	WHERE Id = @Id
END
GO

CREATE PROCEDURE spListSubscriberAlerta
AS
BEGIN
	SELECT * 
	FROM tbSubscriberAlerta
END
GO