using System.Windows;
using ViewModel;

namespace View;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainVM(Environment.GetEnvironmentVariable("CONTACTS_JSON_PATH"));
    }
}
