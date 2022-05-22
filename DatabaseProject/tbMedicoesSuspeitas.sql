CREATE TABLE tbMedicoesSuspeitas (
	Id				INT				PRIMARY KEY   IDENTITY(1, 1),
	DispositivoId	INT				NOT NULL,
	DataMedicao		DATETIME		NOT NULL,
	ValorChuva		FLOAT			NULL,
	ValorNivel		FLOAT			NULL,
	Detalhes		VARCHAR(300)	NULL,
)