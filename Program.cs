using MongoDB.Driver;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton(new List<Message>());
builder.Services.AddSingleton((_) => new MongoClient("mongodb+srv://Lodea:<password>@cluster0.kw4y8.mongodb.net/test"));

var app = builder.Build();

var port = System.Environment.GetEnvironmentVariable("PORT");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
    Console.WriteLine(environmentVariable.Key+ " , "+environmentVariable.Value);

app.Urls.Add("http://*:"+Environment.GetEnvironmentVariable("PORT"));
app.Run();
