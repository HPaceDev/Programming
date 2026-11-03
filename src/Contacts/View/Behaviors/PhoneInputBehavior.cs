using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace View.Behaviors;

public static class PhoneInputBehavior
{
    public static readonly DependencyProperty EnabledProperty =
        DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(PhoneInputBehavior),
            new PropertyMetadata(false, ChangeHandlers));

    public static bool GetEnabled(DependencyObject target) => (bool)target.GetValue(EnabledProperty);
    public static void SetEnabled(DependencyObject target, bool on) => target.SetValue(EnabledProperty, on);

    private static bool IsAllowed(string text) =>
        text.All(c => char.IsAsciiDigit(c) || " +()-".Contains(c));

    private static void ChangeHandlers(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        if (target is not TextBox box) return;
        if ((bool)e.NewValue)
        {
            box.PreviewTextInput += OnTextInput;
            DataObject.AddPastingHandler(box, OnPaste);
        }
        else
        {
            box.PreviewTextInput -= OnTextInput;
            DataObject.RemovePastingHandler(box, OnPaste);
        }
    }

    private static void OnTextInput(object sender, TextCompositionEventArgs e) =>
        e.Handled = !IsAllowed(e.Text);

    private static void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        string? value = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string
                        ?? e.SourceDataObject.GetData(DataFormats.Text) as string;
        if (value == null || !IsAllowed(value)) e.CancelCommand();
    }
}
