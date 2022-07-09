using System.ComponentModel;
using System.Diagnostics;
using System.Net.WebSockets;
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
        //TRY to fix later
        var blues = _messages.Find(_=>true).ToList().Select((message, _) => message.ToJson()).ToList();

        return Newtonsoft.Json.JsonConvert.SerializeObject(new {
            titles = new string[] {"Is Palindrome", "Letter Count", "Ordered"}, 
            messages = _messages.Find(_=>true).ToList()
        });
    }


    public class AddParams { public string? Text {get; set;} }
    [HttpPost("add")]
    public async Task<ActionResult<string>> Add([FromBody] AddParams addParams) {
        if (addParams.Text == null) {
            return BadRequest();
        }
        
        MessageRepo? message = await getMessageByText(addParams.Text);

        if (message == null) {
            var newMessage = new MessageRepo(addParams.Text);
            await _messages.InsertOneAsync(newMessage);
            return Newtonsoft.Json.JsonConvert.SerializeObject(new {traits = newMessage.Traits, isInDB = false});
        } else {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new {traits = message.Traits, isInDB = true});
        }
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

        MessageRepo? message = await getMessageByText(updateParams.NewText);

        if (message == null) {
            var newTraits = new TextTraits(updateParams.NewText);

            await _messages.UpdateOneAsync(
                message => message.Text == updateParams.OldText,
                Builders<MessageRepo>.Update
                    .Set(message => message.Text, updateParams.NewText)
                    .Set(message => message.Traits, newTraits)
            );

            return Newtonsoft.Json.JsonConvert.SerializeObject(new {traits = newTraits, isInDB = false});
        } else {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new {traits = message.Traits, isInDB = true});
        }
    }


    [ApiExplorerSettings(IgnoreApi = true)]
    async Task<MessageRepo?> getMessageByText(string text) {
        var find = _messages.Find(message => message.Text == text);
        if (find.CountDocuments() > 0) {
            return await find.FirstAsync();
        } else {
            return null;
        }
    }
}