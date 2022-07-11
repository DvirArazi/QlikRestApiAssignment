# QlikRestApiAssignment
The app contains a REST API paired with a Client Web UI for creating, retrieving, updating and deleting messages and their traits - such as whether they are palindromes, character count, etc.

The API is made with C# .Net6.0 with the ASP.NET Framework, it stores and retrieves information using a MongoDB database. The Client App was made with plain HTML and JS.
The app was contained using Docker and deployed on Heroku at https://qlik-assignment.herokuapp.com/.

# Architecture
![alt text](https://imgur.com/JubDLrN.png)

The REST API was made in a modular fashion, allowing adding new traits to be stored and displayed by simply adding new properties to the TextTraits class and assigning them a value that fits their logic:
```C#
public class TextTraits {
    public bool IsPalindrome {get;}
    public int CharCount {get;}
    public string Organized {get;}

    public TextTraits(string text) {
        IsPalindrome = Utils.IsPalindrome(text);
        CharCount = Utils.CharCount(text);
        Organized = Utils.Organize(text);
    }
}
```
# Usage
## Run the app
To run the app on your own machine first clone the repository,
then open the `Program.cs` file and replace the line
`app.Urls.Add("http://*:"+Environment.GetEnvironmentVariable("PORT"));`
with `app.Urls.Add("http://*:<port>");` with `<port>` being a port of your choosing.<br />
Then simply go to the project's root directory and execute `dotnet run`.<br />
The server will run on your machine and you may access the web interface by going to `http://localhost:<port>`.

## Other client options
There are other ways to communicate with the service beside the web interface.
You could clone and use the CLI application from the repository [here](https://github.com/DvirArazi/CLI), or you could use the app's Swagger page [here](https://qlik-assignment.herokuapp.com/swagger/index.html).

## REST API Reference
| Action                             | Method | Endpoint                                | Parameters                      |
|------------------------------------|--------|-----------------------------------------|---------------------------------|
| Retrieve all messages              | GET    | api/messages                            |                                 |
| Retrieve a single message          | GET    | api/messages/{id}                       |                                 |
| Create message                     | POST   | api/messages                            | text                            |
| Delete message                     | DELETE | api/messages/{id}                       |                                 |
| Update message                     | PATCH  | api/messages/{id}                       | newText                         |
