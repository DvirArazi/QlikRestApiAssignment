using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

public class MessageRepo {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id {get; set;}
    
    string _text;
    public TextTraits Traits {get; private set;}

    public MessageRepo(string text) {
        Id = "";
        
        _text = text;
        Traits = new TextTraits(text);
    }

    public string Text {
        get => _text;
        set {
            _text = value;
            Traits = new TextTraits(value);
        }
    }
}