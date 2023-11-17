# RinhaBackend

Esta é minha implementação em .NET da aplicação para o desafio de Rinha no backend.

[Repositório do Desafio](https://github.com/zanfranceschi/rinha-de-backend-2023-q3)

## Minha Abordagem

- Utilização do Nginx
- Postgres como banco de dados principal
- .NET 8 com [minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0)
- Uso do [EF Core](https://learn.microsoft.com/en-us/ef/core/) como ORM (Object-Relational Mapping)
- Utilização do [Npgsql](https://www.npgsql.org/) para pool de conexões e driver do banco de dados
- Implementação do [LazyCache](https://github.com/alastairtree/LazyCache) como cache primário
- Utilização do Redis como cache secundário
- Utilização adicional do EF Core como terceiro cache
- Implementação do [MemoryPack](https://github.com/Cysharp/MemoryPack) para serialização de entidades destinadas ao Redis (extremamente mais rápido que o uso de JSON)
- Uso do [BulkExtensions](https://github.com/borisdj/EFCore.BulkExtensions) para inserções periódicas em massa
- Geração de JSON em tempo de compilação por meio dos source generators do .NET 8 [JsonSourceGenerationOptions](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation)
- Utilização do K6 para execução dos testes (Gatlin não funcionou corretamente na minha máquina)

Quando a API recebe uma requisição do tipo `POST` para criar uma Pessoa, ela insere essa pessoa em uma fila global chamada `GlobalQueue`, em um cache local e no cache do Redis (`PersonService.CreateAsync`).

As próximas inserções são validadas contra esse cache do Redis usando o apelido como chave de busca, evitando assim uma consulta ao banco para verificar se a pessoa já existe.

A cada 5 segundos, o `BulkInsertWorker` limpa a fila e insere os registros de uma só vez no banco.

Na busca por termos, são utilizadas queries compiladas para que o SQL gerado seja armazenado em memória, e é empregado um `IAsyncEnumerable` para evitar a criação de uma segunda lista ao enumerar os resultados.

Na busca por `Id`, a API primeiro verifica o cache local; se não encontrar, verifica o cache do Redis e, se ainda não encontrar, acessa o cache local do EF para então consultar o banco. Vale ressaltar que o cache local do EF quase nunca será diferente do cache local em memória.

## Meu Resultado

Ao final dos testes, foram criadas 35.383 pessoas.

![Resultado 1](https://github.com/MarceloMilbradt/RinhaBackend/assets/25873918/bc74456f-307c-4d1c-b7bc-e5817fc893c9)

![Resultado 2](https://github.com/MarceloMilbradt/RinhaBackend/assets/25873918/0503c62b-bfe4-4a01-a7fb-d5ae26ed9de0)
