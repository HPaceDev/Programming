using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Model.Services;
using View.Model;

namespace ViewModel;

/// <summary>Обе панели читают значения одного контакта через эти свойства.</summary>
public class MainVM : INotifyPropertyChanged
{
    public Contact Contact { get; private set; } = new();
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public MainVM(string? filePath = null)
    {
        var store = new ContactSerializer(filePath);
        SaveCommand = new SaveCommand(() => store.Save(Contact));
        LoadCommand = new LoadCommand(() => RefreshFromContact(store.Load()));
    }

    public string Name
    {
        get => Contact.Name;
        set { if (value != Contact.Name) { Contact.Name = value; Notify(); } }
    }

    public string PhoneNumber
    {
        get => Contact.PhoneNumber;
        set { if (value != Contact.PhoneNumber) { Contact.PhoneNumber = value; Notify(); } }
    }

    public string Email
    {
        get => Contact.Email;
        set { if (value != Contact.Email) { Contact.Email = value; Notify(); } }
    }

    private void RefreshFromContact(Contact loaded)
    {
        Contact = loaded;
        Notify(nameof(Contact));
        Notify(nameof(Name));
        Notify(nameof(PhoneNumber));
        Notify(nameof(Email));
    }

    private void Notify([CallerMemberName] string? property = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}
