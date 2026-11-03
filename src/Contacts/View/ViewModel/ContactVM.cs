using System.ComponentModel;
using System.Runtime.CompilerServices;
using View.Model;

namespace ViewModel;

public class ContactVM : INotifyPropertyChanged
{
    private string name, phoneNumber, email;

    public ContactVM(Contact source)
    {
        name = source.Name ?? "";
        phoneNumber = source.PhoneNumber ?? "";
        email = source.Email ?? "";
    }

    public string Name { get => name; set => Set(ref name, value); }
    public string PhoneNumber { get => phoneNumber; set => Set(ref phoneNumber, value); }
    public string Email { get => email; set => Set(ref email, value); }
    public Contact ToModel() => new Contact(Name, PhoneNumber, Email);

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Set(ref string field, string value, [CallerMemberName] string? property = null)
    {
        if (field == value) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
