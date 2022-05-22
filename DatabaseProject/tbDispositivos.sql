CREATE TABLE tbDispositivos (
	Id					INT				PRIMARY KEY,
	Descricao			VARCHAR(100)	NULL,
	BairroID			INT
						CONSTRAINT FK_Bairro_Dispositivo 
							FOREIGN KEY REFERENCES tbBairros ON DELETE CASCADE NOT NULL,
	DataAtualizacao		DATETIME		NOT NULL,
	MedicaoReferencia	FLOAT			NOT NULL
)