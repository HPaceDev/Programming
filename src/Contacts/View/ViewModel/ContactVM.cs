using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using View.Model;

namespace ViewModel;

/// <summary>Проверяет редактируемую копию контакта.</summary>
public class ContactVM : ObservableValidator
{
    private string name, phoneNumber, email;
    private bool isEditing;

    public bool IsEditing
    {
        get => isEditing;
        internal set
        {
            if (SetProperty(ref isEditing, value)) OnPropertyChanged(nameof(IsReadOnly));
        }
    }

    public bool IsReadOnly => !IsEditing;

    public ContactVM(Contact source)
    {
        name = source.Name ?? "";
        phoneNumber = source.PhoneNumber ?? "";
        email = source.Email ?? "";
        ValidateAllProperties();
    }

    [Required(ErrorMessage = "Введите имя.")]
    [StringLength(100, ErrorMessage = "Имя не должно быть длиннее 100 символов.")]
    public string Name
    {
        get => name;
        set => SetProperty(ref name, value, true);
    }

    [Required(ErrorMessage = "Введите номер телефона.")]
    [StringLength(100, ErrorMessage = "Номер не должен быть длиннее 100 символов.")]
    [RegularExpression(@"[0-9 +()\-]*", ErrorMessage = "Допустимы цифры, пробел, +, -, ( и ).")]
    public string PhoneNumber
    {
        get => phoneNumber;
        set => SetProperty(ref phoneNumber, value, true);
    }

    [Required(ErrorMessage = "Введите почту.")]
    [StringLength(100, ErrorMessage = "Адрес не должен быть длиннее 100 символов.")]
    [RegularExpression(@".*@.*", ErrorMessage = "В адресе должен быть символ @.")]
    public string Email
    {
        get => email;
        set => SetProperty(ref email, value, true);
    }

    public Contact ToModel() => new Contact(Name, PhoneNumber, Email);
}
