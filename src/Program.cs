using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MinimalWebApiSSE.Reposirory;
using MinimalWebApiSSE.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCors(options
        => options.AddPolicy(
            name: "cors",
            configurePolicy: policyBuilder =>
            {
                policyBuilder.AllowAnyHeader();
                policyBuilder.AllowAnyMethod();
                policyBuilder.AllowAnyOrigin();
            }));
builder.Services.AddControllers();
builder.Services.AddSingleton<IMessageRepository, MessageRepository>();

builder.Services.AddSingleton<IOrderObservable, OrdersObservable>();


await using var app = builder.Build();

app.UseCors("cors");
app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

app.MapGet(
    pattern: "/sse",
    requestDelegate: async context =>
    {
        context.Response.Headers.Add("Content-Type", "text/event-stream");

        while (true)
        {
            await context.Response.WriteAsync($"data: ${DateTimeOffset.Now}\n\n");
            await context.Response.Body.FlushAsync();
            await Task.Delay(5000);
        }
    });


await app.RunAsync();