# Redis Lock
<p>O pacote do Redis para a plataforma .NET tem um próprio recurso que permite inserir e retirar lock de uma determinada chave.</p>

## Contexto
<p>Em alguns cenários, é necessário bloquear o acesso a um determinado registro para que não haja conflitos ou inconsistência ao longo do processamento. </p>
<p>Para exemplificar, podemos imaginar o cenário de um processamento assíncrono via mensageria que busca um registro salvo no Redis. Porém, um fluxo de processamento não pode buscar o mesmo registro que outro fluxo.</p>
<p>Sendo assim, pode-se utilizar o lock para informar que determinado registro está sendo usado e, assim, buscar encontrar um novo registro que esteja disponível.</p>
<p>Outro exemplo de uso é para implementar um lock distrbuído, cenário comum em microservices.</p>

## Utilização
<p>Neste exemplo, foi criado 4 endpoints para o teste. </p>

- Um endpoint para adicionar um key e valor no Redis.
- Um endpoint para retornar o valor de uma determinada key. Nesse endpoint, é realizado o lock para esse registro. Caso o lock ainda esteja ativo, irá retornar uma mensagem padrão. Caso o lock esteja inativo, retorna o valor da key no Redis.
- Um endpoint para remover o lock baseado na key informada.

<p>Para adicionar um lock no Redis, existe o método 'LockTakeAsync'. Nele é preciso informar a chave do lock, o valor e o tempo do lock.</p>
<p>Se caso o registro já esteja com o lock ativo, ele irá retornar o valor 'false'. Caso no momento o lock esteja inativo, ele irá retornar 'true'. </p>

```csharp
await redisDataBase.LockTakeAsync($"lock-{key}", key, TimeSpan.FromMinutes(1));
```
<p>Para remover um lock no Redis, existe o método 'LockReleaseAsync'. Nele é preciso informar a chave do lock, o valor.</p>

```csharp
await redisDataBase.LockReleaseAsync($"lock-{key}", key);
```

## Pacotes
- StackExchange.Redis
