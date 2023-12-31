CREATE EXTENSION PG_TRGM;
CREATE TABLE IF NOT EXISTS "Persons" (
    "Id" uuid CONSTRAINT PK_Persons PRIMARY key NOT NULL,
    "Apelido" VARCHAR(40),
    "Nome" VARCHAR(120),
    "Nascimento" date,
    "Stack" VARCHAR(1024),
    "SearchField" TEXT GENERATED ALWAYS AS (
        LOWER("Nome" || "Apelido" || "Stack")
    ) STORED
);

CREATE INDEX CONCURRENTLY IF NOT EXISTS IDX_PESSOAS_BUSCA_TGRM ON "Persons" USING GIST ("SearchField" GIST_TRGM_OPS(SIGLEN=64));
TRUNCATE TABLE "Persons";