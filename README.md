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




```
          /\      |‾‾| /‾‾/   /‾‾/
     /\  /  \     |  |/  /   /  /
    /  \/    \    |     (   /   ‾‾\
   /          \   |  |\  \ |  (‾)  |
  / __________ \  |__| \__\ \_____/ .io

  execution: local
     script: script.js
     output: -

  scenarios: (100.00%) 1 scenario, 600 max VUs, 3m55s max duration (incl. graceful stop):
           * default: Up to 600 looping VUs for 3m25s over 3 stages (gracefulRampDown: 30s, gracefulStop: 30s)


     ✓ criação status is 201, 422, or 400
     ✓ busca válida status is 2xx
     ✓ busca inválida status is 400

     checks.........................: 100.00% ✓ 120057     ✗ 0
     data_received..................: 18 MB   89 kB/s
     data_sent......................: 23 MB   112 kB/s
     http_req_blocked...............: avg=6.81µs   min=0s       med=0s       max=23.22ms  p(90)=0s       p(95)=0s
     http_req_connecting............: avg=3.74µs   min=0s       med=0s       max=11.06ms  p(90)=0s       p(95)=0s
   ✗ http_req_duration..............: avg=449.66ms min=524.59µs med=374.51ms max=3.7s     p(90)=980.4ms  p(95)=1.2s
       { expected_response:true }...: avg=399.66ms min=1.5ms    med=358.29ms max=3.48s    p(90)=781ms    p(95)=998.79ms
      http_req_failed................: 66.66%  ✓ 80038      ✗ 40019
     http_req_receiving.............: avg=69.59µs  min=0s       med=0s       max=542.79ms p(90)=0s       p(95)=328.6µs
     http_req_sending...............: avg=21.24µs  min=0s       med=0s       max=38.32ms  p(90)=0s       p(95)=0s
     http_req_tls_handshaking.......: avg=0s       min=0s       med=0s       max=0s       p(90)=0s       p(95)=0s
     http_req_waiting...............: avg=449.57ms min=519.2µs  med=374.41ms max=3.7s     p(90)=980.18ms p(95)=1.2s
     http_reqs......................: 120057  581.707264/s
     iteration_duration.............: avg=1.37s    min=8.11ms   med=1.29s    max=4.91s    p(90)=2.4s     p(95)=2.8s
     iterations.....................: 40019   193.902421/s
     vus............................: 294     min=1        max=599
     vus_max........................: 600     min=600      max=600


running (3m26.4s), 000/600 VUs, 40019 complete and 0 interrupted iterations
default ✓ [======================================] 000/600 VUs  3m25s
ERRO[0208] thresholds on metrics 'http_req_duration' have been crossed
```
