using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

public class MessageRepo {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id {get; set;}
    
    public string Text;
    public bool IsPalindrome;

    public MessageRepo(string text, bool isPalindrome) {
        Id = "";
        
        Text = text;
        IsPalindrome = isPalindrome;
    }
}