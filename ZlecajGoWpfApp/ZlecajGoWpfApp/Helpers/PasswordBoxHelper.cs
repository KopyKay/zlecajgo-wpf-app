using System.Windows;
using System.Windows.Controls;

namespace ZlecajGoWpfApp.Helpers;

public static class PasswordBoxHelper
{
    public static readonly DependencyProperty BoundPassword =
        DependencyProperty.RegisterAttached(nameof(BoundPassword), typeof(string), typeof(PasswordBoxHelper), 
            new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

    public static readonly DependencyProperty BindPassword =
        DependencyProperty.RegisterAttached(nameof(BindPassword), typeof(bool), typeof(PasswordBoxHelper), 
            new PropertyMetadata(false, OnBindPasswordChanged));

    private static readonly DependencyProperty UpdatingPassword =
        DependencyProperty.RegisterAttached(nameof(UpdatingPassword), typeof(bool), typeof(PasswordBoxHelper), 
            new PropertyMetadata(false));

    public static string GetBoundPassword(DependencyObject dp) => (string)dp.GetValue(BoundPassword);

    public static void SetBoundPassword(DependencyObject dp, string value) => dp.SetValue(BoundPassword, value);

    public static bool GetBindPassword(DependencyObject dp) => (bool)dp.GetValue(BindPassword);

    public static void SetBindPassword(DependencyObject dp, bool value) => dp.SetValue(BindPassword, value);
    
    private static bool GetUpdatingPassword(DependencyObject dp) => (bool)dp.GetValue(UpdatingPassword);

    private static void SetUpdatingPassword(DependencyObject dp, bool value) => dp.SetValue(UpdatingPassword, value);

    private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        if (dp is not PasswordBox passwordBox || GetUpdatingPassword(passwordBox)) return;
        
        passwordBox.PasswordChanged -= HandlePasswordChanged;
        passwordBox.Password = (string)e.NewValue;
        passwordBox.PasswordChanged += HandlePasswordChanged;
    }

    private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        if (dp is not PasswordBox passwordBox) return;
        
        var wasBound = (bool)e.OldValue;
        var needToBind = (bool)e.NewValue;

        if (wasBound) passwordBox.PasswordChanged -= HandlePasswordChanged;

        if (needToBind) passwordBox.PasswordChanged += HandlePasswordChanged;
    }

    private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not PasswordBox passwordBox) return;
        
        SetUpdatingPassword(passwordBox, true);
        SetBoundPassword(passwordBox, passwordBox.Password);
        SetUpdatingPassword(passwordBox, false);
    }
}