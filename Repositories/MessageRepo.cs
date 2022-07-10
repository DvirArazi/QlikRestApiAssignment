using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

public class Message {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id {get; set;}
    
    public DateTime CreationDate {get; set;}

    string _text;
    public TextTraits Traits {get; private set;}

    public Message(string text) {
        Id = "";
        
        _text = text;
        Traits = new TextTraits(text);
        CreationDate = DateTime.Now;
    }

    public string Text {
        get => _text;
        set {
            _text = value;
            Traits = new TextTraits(value);
        }
    }
}