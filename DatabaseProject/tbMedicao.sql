CREATE TABLE tbMedicao (
	Id				INT				PRIMARY KEY		IDENTITY(1, 1),
	DispositivoId	INT				NOT NULL CONSTRAINT FK_Medicao_Dispositivos 
												FOREIGN KEY REFERENCES tbDispositivos ON DELETE CASCADE,
	DataMedicao		DATETIME		NOT NULL,
	ValorChuva		FLOAT			NULL,
	ValorNivel		FLOAT			NULL,
)