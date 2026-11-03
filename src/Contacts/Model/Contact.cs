namespace Model;

/// <summary>Имя, телефон и почта одной записи адресной книги.</summary>
public class Contact
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public Contact() : this("", "", "") { }

    public Contact(string name, string phoneNumber, string email)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}
