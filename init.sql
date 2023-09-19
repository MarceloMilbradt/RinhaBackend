CREATE EXTENSION pg_trgm;
CREATE TABLE  IF NOT EXISTS "Persons" (

	"Id" uuid NOT NULL,
	"Apelido" varchar(50) NULL,
	"Nome" varchar(150) NOT NULL,
	"Nascimento" date NOT NULL,
	"Stack" text NOT NULL,
	"SearchField" varchar(500) NULL,
	CONSTRAINT "PK_Persons" PRIMARY KEY ("Id")
);
CREATE INDEX "IX_Persons_Id" ON public."Persons" USING btree ("Id");
CREATE INDEX idx_searchfield_trgm_gin ON public."Persons" USING gin ("SearchField" gin_trgm_ops);