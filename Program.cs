using MongoDB.Driver;
using System.Collections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

var client = new MongoClient("mongodb+srv://Lodea:13kSViP92Pu0DQbQ@cluster0.kw4y8.mongodb.net/test");
var db = client.GetDatabase("PalindromeDB");
var messages = db.GetCollection<Message>("Messages");

builder.Services.AddSingleton(messages);

var app = builder.Build();

var port = System.Environment.GetEnvironmentVariable("PORT");

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
    Console.WriteLine(environmentVariable.Key+ " , "+environmentVariable.Value);

app.Urls.Add("http://*:"+Environment.GetEnvironmentVariable("PORT"));
app.Run();
