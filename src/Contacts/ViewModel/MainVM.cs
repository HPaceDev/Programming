using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model;
using Model.Services;

namespace ViewModel;

public partial class MainVM : ObservableObject
{
    private readonly ContactSerializer store;
    private bool loadFailed;
    public ObservableCollection<ContactVM> Contacts { get; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditContactCommand), nameof(RemoveContactCommand))]
    private ContactVM? selectedListContact;

    [ObservableProperty]
    private ContactVM selectedContact = new ContactVM(new Contact());

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReadOnly))]
    [NotifyCanExecuteChangedFor(nameof(AddContactCommand), nameof(EditContactCommand),
        nameof(RemoveContactCommand), nameof(ApplyContactCommand))]
    private bool isEditing;

    [ObservableProperty]
    private string statusMessage = "";

    public bool IsReadOnly => !IsEditing;
    private bool CanAdd() => !IsEditing;

    partial void OnSelectedListContactChanged(ContactVM? value)
    {
        IsEditing = false;
        SelectedContact = CopyOf(value);
    }

    partial void OnIsEditingChanged(bool value) => SelectedContact.IsEditing = value;

    partial void OnSelectedContactChanged(ContactVM? oldValue, ContactVM newValue)
    {
        if (oldValue != null) oldValue.ErrorsChanged -= OnErrorsChanged;
        newValue.IsEditing = IsEditing;
        newValue.ErrorsChanged += OnErrorsChanged;
        ApplyContactCommand.NotifyCanExecuteChanged();
    }

    public MainVM(string? filePath = null)
    {
        store = new ContactSerializer(filePath);
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

    private static ContactVM CopyOf(ContactVM? contact) =>
        new ContactVM(contact?.ToModel() ?? new Contact());
    private bool CanUseSelection() => !IsEditing && SelectedListContact != null;
    private bool CanApply() => IsEditing && !SelectedContact.HasErrors;

    private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e) =>
        ApplyContactCommand.NotifyCanExecuteChanged();

    [RelayCommand(CanExecute = nameof(CanAdd))]
    private void AddContact()
    {
        SelectedListContact = null;
        SelectedContact = CopyOf(null);
        IsEditing = true;
    }

    [RelayCommand(CanExecute = nameof(CanUseSelection))]
    private void EditContact()
    {
        SelectedContact = CopyOf(SelectedListContact);
        IsEditing = true;
    }

    [RelayCommand(CanExecute = nameof(CanApply))]
    private void ApplyContact()
    {
        if (!CanApply()) return;
        int index = SelectedListContact == null ? -1 : Contacts.IndexOf(SelectedListContact);
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

    [RelayCommand(CanExecute = nameof(CanUseSelection))]
    private void RemoveContact()
    {
        if (!CanUseSelection()) return;
        int index = Contacts.IndexOf(SelectedListContact!);
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

}
