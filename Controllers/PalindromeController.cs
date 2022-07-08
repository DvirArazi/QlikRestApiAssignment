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
public class PalindromeController : ControllerBase {
    IMongoCollection<MessageRepo> _messages;

    public PalindromeController(List<MessageRepo> messages) {
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

        var rtn = (isPalindrome: isPalindrome, isInDB: isInDB);
        return Newtonsoft.Json.JsonConvert.SerializeObject(new {rtn.isPalindrome, rtn.isInDB});
    }

    [HttpGet("delete/{text}")]
    public async Task<string> Delete(string text) {
        var result = await _messages.DeleteOneAsync(message => message.Text == text);

        return result.DeletedCount > 0 ? "Message was deleted." : "Message does not exist in the database.";
    }

    [HttpGet("update/{oldText}/{newText}")]
    public async Task<string> Update(string oldText, string newText) {
        bool isPalindrome = Utils.IsPalindrome(newText);
        bool isInDB = await containsMessage(newText);

        if (!isInDB) {
            await _messages.UpdateOneAsync(
                message => message.Text == oldText,
                Builders<MessageRepo>.Update
                    .Set(message => message.Text, newText)
                    .Set(message => message.IsPalindrome, isPalindrome)
            );
        }

        var rtn = (isPalindrome: isPalindrome, isInDB: isInDB);
        return Newtonsoft.Json.JsonConvert.SerializeObject(new {rtn.isPalindrome, rtn.isInDB});
    }

    [HttpGet("isInDB/{text}")]
    async Task<bool> containsMessage(string text) {
        return await _messages.Find(message => message.Text == text).AnyAsync();
    }
}