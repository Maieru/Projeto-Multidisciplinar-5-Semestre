CREATE TABLE tbSubscriberAlerta (
	Id					INT				PRIMARY KEY,
	BairroId			INT	
						CONSTRAINT FK_Bairro_Subscriber 
							FOREIGN KEY REFERENCES tbBairros ON DELETE CASCADE NOT NULL,
	Telefone			VARCHAR(100)	NULL,
	Email				VARCHAR(200)	NULL,
	UsuarioId			INT
						CONSTRAINT FK_Usuario_Subscriber 
							FOREIGN KEY REFERENCES tbUsuario ON DELETE CASCADE NOT NULL,
	UltimaNotificacao	INT				NULL
)