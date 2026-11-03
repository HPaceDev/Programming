using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Model;

namespace Model.Services;

public sealed class ContactSerializer
{
    public string FilePath { get; }

    public ContactSerializer(string? filePath = null)
    {
        string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        FilePath = filePath ?? Path.Combine(documents, "Contacts", "contacts.json");
    }

    public List<Contact> Load()
    {
        if (!File.Exists(FilePath)) return new List<Contact>();

        var document = JToken.Parse(File.ReadAllText(FilePath));
        // Ранее один контакт сохранялся в том же файле отдельным объектом.
        if (document is JObject single)
            return new List<Contact> { single.ToObject<Contact>()! };
        if (document is not JArray array || array.Any(row => row is not JObject))
            throw new JsonSerializationException("В файле ожидался список контактов.");
        return array.ToObject<List<Contact>>()!;
    }

    public void Save(IEnumerable<Contact> contacts)
    {
        var fullPath = Path.GetFullPath(FilePath);
        var dir = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(dir);
        string temp = Path.Combine(dir, Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            string json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
            File.WriteAllText(temp, json);
            File.Move(temp, fullPath, true);
        }
        finally
        {
            if (File.Exists(temp)) File.Delete(temp);
        }
    }
}
