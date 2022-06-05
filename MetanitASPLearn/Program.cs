using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Run(async (context) => 
{
    
    var inputPath = context.Request.Path;
    var fullPath = $"html/{inputPath}.html";

    context.Response.ContentType = "text/html; charset=utf-8";
    if (context.Request.Path == "/old")
    {
        context.Response.Redirect("html/index.html");
    }
    if (context.Request.Path == "/nameAndAge")
    {
        var form = context.Request.Form;

        string name = form["userName"];
        string age = form["userAge"];
        string[] languages = form["languages"];
        string allLang = "C#";
        foreach (var language in languages) allLang += ", " + language;

        await context.Response.WriteAsync($"<div><p>Name: {name}</p><p>Age: {age}</p><p>Languages: {allLang}</p></div>");
    }
    else
    {
        await context.Response.SendFileAsync("html/index.html");
    }

    if (File.Exists(fullPath))
    {
        await context.Response.SendFileAsync(fullPath);
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("None");
    }
});

app.Run();

public record Person(string Name, int Age);
public class PersonConverter : JsonConverter<Person>
{
    public override Person Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var personName = "Undefined";
        var personAge = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    // если свойство Age/age и оно содержит число
                    case "age" or "Age" when reader.TokenType == JsonTokenType.Number:
                        personAge = reader.GetInt32();  // считываем число из json
                        break;
                    // если свойство Age/age и оно содержит строку
                    case "age" or "Age" when reader.TokenType == JsonTokenType.String:
                        string? stringValue = reader.GetString();
                        // пытаемся конвертировать строку в число
                        if (int.TryParse(stringValue, out int value))
                        {
                            personAge = value;
                        }
                        break;
                    case "Name" or "name":  // если свойство Name/name
                        string? name = reader.GetString();
                        if (name != null)
                            personName = name;
                        break;
                }
            }
        }
        return new Person(personName, personAge);

    }
    // Serialize object to Json
    public override void Write(Utf8JsonWriter writer, Person person, JsonSerializerOptions options)
    {

        writer.WriteStartObject();
        writer.WriteString("name", person.Name);
        writer.WriteNumber("age", person.Age);

        writer.WriteEndObject();
    }    
}