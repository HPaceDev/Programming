using System.IO;
using Newtonsoft.Json;
using View.Model;

namespace Model.Services;

public class ContactSerializer
{
    public string FilePath { get; }

    public ContactSerializer(string? filePath = null)
    {
        string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        FilePath = filePath ?? Path.Combine(documents, "Contacts", "contacts.json");
    }

    public void Save(Contact contact)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(FilePath))!);
        string json = JsonConvert.SerializeObject(contact, Formatting.Indented);
        File.WriteAllText(FilePath, json);
    }

    public Contact Load()
    {
        if (!File.Exists(FilePath)) return new Contact();
        return JsonConvert.DeserializeObject<Contact>(File.ReadAllText(FilePath)) ?? new Contact();
    }
}
