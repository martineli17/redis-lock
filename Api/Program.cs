using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var redisDataBase = ConnectionMultiplexer.Connect("localhost,abortConnect=false,connectTimeout=30000,responseTimeout=30000").GetDatabase(0);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("add/{key}/{value}", async ([FromRoute] string key, [FromRoute] string value) =>
{
    await redisDataBase.StringSetAsync(key, value, TimeSpan.FromMinutes(10));
});
app.MapGet("get/{key}", async ([FromRoute] string key) =>
{
    var lockTake = await redisDataBase.LockTakeAsync($"lock-{key}", key, TimeSpan.FromMinutes(1));

    if (lockTake)
        return Results.Ok(new { Value = (string)redisDataBase.StringGet(key)! });
    
    return Results.Ok(new { Value = "Key requested is locked yet. Wait to release " });
});
app.MapGet("release/{key}", async ([FromRoute] string key) =>
{
    await redisDataBase.LockReleaseAsync($"lock-{key}", key);
});
app.MapGet("batch-lock/{key}", () =>
{
    Parallel.For(1, 10, async (a) =>
    {
        var lockTake = await redisDataBase.LockTakeAsync($"lock-teste", a, TimeSpan.FromMinutes(1));
        if(lockTake)
            Console.WriteLine($"Loop number {a} set lock");
    });

    return Results.Ok();
});
app.Run("http://localhost:3000");