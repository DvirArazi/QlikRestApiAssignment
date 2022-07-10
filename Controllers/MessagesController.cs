using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace QuikRestApiAssignment.Controllers;

[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase {
    IMongoCollection<Message> _messages;

    public MessagesController(List<Message> messages) {
        var client = new MongoClient("mongodb+srv://Lodea:1234@cluster0.kw4y8.mongodb.net/test");
        var db = client.GetDatabase("PalindromeDB");
        _messages = db.GetCollection<Message>("Messages");
    }


    // GET /api/messages
    [HttpGet]
    public async Task<ContentResult> Get() {
        return new ContentResult {
            StatusCode = 200, //ok
            Content = toJson((await _messages.FindAsync(_=>true)).ToEnumerable())
        };
    }


    // GET /api/messages/{id}
    [HttpGet("{id}")]
    public async Task<ContentResult> GetMessage(String id) {
        var message = await getMessage(id);

        if (message == null) {
            return new ContentResult {
                StatusCode = 404 //not found
            }; //404 not found
        }

        return new ContentResult {
            StatusCode = 200, //ok
            Content = toJson(message)
        };
    }


    // POST /api/messages
    public class AddParams { public string? Text {get; set;} }
    [HttpPost]
    public async Task<ContentResult> Add([FromBody] AddParams addParams) {
        if (addParams.Text == null) {
            return new ContentResult {
                StatusCode = 400 //bad request
            };
        }
        
        Message? message = await getMessageByText(addParams.Text);

        if (message == null) {
            var newMessage = new Message(addParams.Text);
            await _messages.InsertOneAsync(newMessage);
            return new ContentResult {
                StatusCode = 200, //ok
                Content = toJson(newMessage)
            };
        } else {
            return new ContentResult {
                StatusCode = 409 //conflict
            };
        }
    }


    // DELETE /api/messages
    [HttpDelete("{id}")]
    public async Task<ContentResult> Delete(string id) {
        if (id == null) {
            return new ContentResult {
                StatusCode = 400 //bad request
            };
        }

        var message = await getMessage(id);
        if (message == null) {
            return new ContentResult {
                StatusCode = 404 //not found
            };
        }

        var dbRes = await _messages.DeleteOneAsync(message => message.Id == id);

        return new ContentResult {
            StatusCode = dbRes.DeletedCount > 0 ? 200 : 404, //ok : not found
            Content = toJson(message)
        };
    }

    // PATCH /api/messages
    public class UpdateParams {
        public string? NewText {get; set;}
    }
    [HttpPatch("{id}")]
    public async Task<ContentResult> Update(string id, [FromBody] UpdateParams updateParams) {
        if (id == null || updateParams.NewText == null) {
            return new ContentResult {
                StatusCode = 400, //bad request
            };
        }

        Message? originalMessage = await getMessage(id);

        if (originalMessage == null) {
            return new ContentResult {
                StatusCode = 404 //not found
            };
        }

        Message? secondMessage = await getMessageByText(updateParams.NewText);

        if (secondMessage == null) {
            var newMessage = new Message(updateParams.NewText);

            await _messages.UpdateOneAsync(
                message => message.Id == id,
                Builders<Message>.Update
                    .Set(message => message.Text, updateParams.NewText)
                    .Set(message => message.Traits, newMessage.Traits)
            );

            return new ContentResult {
                StatusCode = 200, //ok
                Content = toJson(newMessage)
            };
        } else {
            return new ContentResult {
                StatusCode = 403, //forbidden
                Content = toJson(secondMessage)
            };
        }
    }


    [ApiExplorerSettings(IgnoreApi = true)]
    async Task<Message?> getMessageByText(string text) {
        var find = _messages.Find(message => message.Text == text);
        if (find.CountDocuments() > 0) {
            return await find.FirstAsync();
        } else {
            return null;
        }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    async Task<Message?> getMessage(string id) {
        if (!ObjectId.TryParse(id, out _)) {
            return null;
        }

        var find =  _messages.Find(message => message.Id == id);
        if (find.CountDocuments() > 0) {
            return await find.FirstAsync();
        } else {
            return null;
        }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    string toJson(object obj) {
        return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
    }
}