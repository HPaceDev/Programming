using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using Model.Services;
using View.Model;

namespace ViewModel;

public class MainVM : INotifyPropertyChanged
{
    private readonly ContactSerializer store;
    private bool loadFailed;
    private ContactVM? selection;
    private ContactVM editor = new ContactVM(new Contact());
    private bool editing;
    private string status = "";

    public ObservableCollection<ContactVM> Contacts { get; } = new();
    public RelayCommand AddContactCommand { get; }
    public RelayCommand EditContactCommand { get; }
    public RelayCommand RemoveContactCommand { get; }
    public RelayCommand ApplyContactCommand { get; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public MainVM(string? filePath = null)
    {
        store = new ContactSerializer(filePath);
        AddContactCommand = new RelayCommand(AddContact, () => !IsEditing);
        EditContactCommand = new RelayCommand(EditContact, CanUseSelection);
        RemoveContactCommand = new RelayCommand(RemoveContact, CanUseSelection);
        ApplyContactCommand = new RelayCommand(ApplyContact, CanApply);
        try
        {
            foreach (var contact in store.Load()) Contacts.Add(new ContactVM(contact));
            SelectedListContact = Contacts.FirstOrDefault();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or Newtonsoft.Json.JsonException)
        {
            loadFailed = true;
            StatusMessage = "Не удалось прочитать контакты. " + ex.Message;
        }
    }

    public ContactVM? SelectedListContact
    {
        get => selection;
        set
        {
            if (ReferenceEquals(selection, value)) return;
            selection = value;
            Notify();
            IsEditing = false;
            SelectedContact = CopyOf(value);
            UpdateCommands();
        }
    }

    public ContactVM SelectedContact
    {
        get => editor;
        private set { editor = value; Notify(); }
    }

    public bool IsEditing
    {
        get => editing;
        private set
        {
            if (editing == value) return;
            editing = value;
            Notify();
            Notify(nameof(IsReadOnly));
            UpdateCommands();
        }
    }

    public bool IsReadOnly => !IsEditing;

    public string StatusMessage
    {
        get => status;
        private set { status = value; Notify(); }
    }

    private static ContactVM CopyOf(ContactVM? contact) =>
        new ContactVM(contact?.ToModel() ?? new Contact());
    private bool CanUseSelection() => !IsEditing && selection != null;
    private bool CanApply() => IsEditing;

    private void AddContact()
    {
        SelectedListContact = null;
        SelectedContact = CopyOf(null);
        IsEditing = true;
    }

    private void EditContact()
    {
        SelectedContact = CopyOf(selection);
        IsEditing = true;
    }

    private void ApplyContact()
    {
        if (!CanApply()) return;
        int index = selection == null ? -1 : Contacts.IndexOf(selection);
        Contact updated = SelectedContact.ToModel();
        var rows = Contacts.Select(c => c.ToModel()).ToList();
        if (index < 0) rows.Add(updated);
        else rows[index] = updated;
        if (!TrySave(rows)) return;

        var committed = new ContactVM(updated);
        if (index < 0) Contacts.Add(committed);
        else Contacts[index] = committed;
        SelectedListContact = committed;
    }

    private void RemoveContact()
    {
        if (!CanUseSelection()) return;
        int index = Contacts.IndexOf(selection!);
        var rows = Contacts.Select(c => c.ToModel()).ToList();
        rows.RemoveAt(index);
        if (!TrySave(rows)) return;

        Contacts.RemoveAt(index);
        SelectedListContact = Contacts.Count == 0 ? null : Contacts[Math.Min(index, Contacts.Count - 1)];
    }

    private bool TrySave(List<Contact> rows)
    {
        if (loadFailed) return false;
        try
        {
            store.Save(rows);
            StatusMessage = "";
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            StatusMessage = "Не удалось сохранить контакты. " + ex.Message;
            return false;
        }
    }

    private void UpdateCommands()
    {
        AddContactCommand.NotifyCanExecuteChanged();
        EditContactCommand.NotifyCanExecuteChanged();
        RemoveContactCommand.NotifyCanExecuteChanged();
        ApplyContactCommand.NotifyCanExecuteChanged();
    }

    private void Notify([CallerMemberName] string? property = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}
