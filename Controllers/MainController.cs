using System.ComponentModel;
using System.Diagnostics;
// using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Newtonsoft.Json;
// using Newtonsoft.Json;

namespace QuikRestApiAssignment.Controllers;

[ApiController]
[Route("")]
public class MainController : ControllerBase {
    IMongoCollection<MessageRepo> _messages;

    public MainController(List<MessageRepo> messages) {
        var client = new MongoClient("mongodb+srv://Lodea:Dkxr7214!@cluster0.kw4y8.mongodb.net/test");
        var db = client.GetDatabase("PalindromeDB");
        _messages = db.GetCollection<MessageRepo>("Messages");
    }

    [HttpGet]
    public ContentResult Home() {
        return base.Content(
            System.IO.File.ReadAllText("wwwroot/index.html"),
            "text/html"
        );
    }

    [HttpGet("all")]
    public string All() {
        var blues = _messages.Find(_=>true).ToList().Select((message, _) => message.ToJson()).ToList();
        foreach (var blue in blues) {
            Console.WriteLine(blue);
        }
        Console.WriteLine(blues.ToJson());
        Console.WriteLine(_messages.Find(_=>true).First());

        return Newtonsoft.Json.JsonConvert.SerializeObject(_messages.Find(_=>true).ToList());
    }

    public class AddParams { public string? Text {get; set;} }
    [HttpPost("add")]
    public async Task<ActionResult<string>> Add([FromBody] AddParams addParams) {
        if (addParams.Text == null) {
            return BadRequest();
        }
        
        bool isPalindrome = Utils.IsPalindrome(addParams.Text);
        bool isInDB = await containsMessage(addParams.Text);

        if (!isInDB) {
            await _messages.InsertOneAsync(new MessageRepo(addParams.Text, isPalindrome));
        }

        return Newtonsoft.Json.JsonConvert.SerializeObject(new {isPalindrome = isPalindrome, isInDB = isInDB});
    }

    public class DeleteParams { public string? Text {get; set;} }
    [HttpDelete("delete")]
    public async Task<ActionResult<string>> Delete([FromBody] DeleteParams deleteParams) {
        if (deleteParams.Text == null) {
            return BadRequest();
        }
        
        var dbRes = await _messages.DeleteOneAsync(message => message.Text == deleteParams.Text);

        return Newtonsoft.Json.JsonConvert.SerializeObject(new {wasFound = dbRes.DeletedCount > 0});;
    }

    public class UpdateParams {
        public string? OldText {get; set;}
        public string? NewText {get; set;}
    }
    [HttpPatch("update")]
    public async Task<ActionResult<string>> Update([FromBody] UpdateParams updateParams) {
        if (updateParams.OldText == null || updateParams.NewText == null) {
            return BadRequest();
        }
        bool isPalindrome = Utils.IsPalindrome(updateParams.NewText);
        bool isInDB = await containsMessage(updateParams.NewText);

        if (!isInDB) {
            await _messages.UpdateOneAsync(
                message => message.Text == updateParams.OldText,
                Builders<MessageRepo>.Update
                    .Set(message => message.Text, updateParams.NewText)
                    .Set(message => message.IsPalindrome, isPalindrome)
            );
        }

        return Newtonsoft.Json.JsonConvert.SerializeObject(new {isPalindrome = isPalindrome, isInDB = isInDB});
    }

    [HttpGet("isInDB/{text}")]
    async Task<bool> containsMessage(string text) {
        return await _messages.Find(message => message.Text == text).AnyAsync();
    }
}