using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Model;

namespace ViewModel;

public partial class ContactVM : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Введите имя.")]
    [StringLength(100, ErrorMessage = "Имя не должно быть длиннее 100 символов.")]
    private string name;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Введите номер телефона.")]
    [StringLength(100, ErrorMessage = "Номер не должен быть длиннее 100 символов.")]
    [RegularExpression(@"[0-9 +()\-]*", ErrorMessage = "Допустимы цифры, пробел, +, -, ( и ).")]
    private string phoneNumber;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Введите почту.")]
    [StringLength(100, ErrorMessage = "Адрес не должен быть длиннее 100 символов.")]
    [RegularExpression(@".*@.*", ErrorMessage = "В адресе должен быть символ @.")]
    private string email;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReadOnly))]
    private bool isEditing;

    public bool IsReadOnly => !IsEditing;

    public ContactVM(Contact source)
    {
        name = source.Name ?? "";
        phoneNumber = source.PhoneNumber ?? "";
        email = source.Email ?? "";
        ValidateAllProperties();
    }

    public Contact ToModel() => new Contact(Name, PhoneNumber, Email);
}
