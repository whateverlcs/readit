using System.Windows;
using System.Windows.Controls;

namespace Readit.Infra.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject obj) =>
            (string)obj.GetValue(BoundPasswordProperty);

        public static void SetBoundPassword(DependencyObject obj, string value) =>
            obj.SetValue(BoundPasswordProperty, value);

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                // Evitar sobrescrever a senha ao atualizar.
                if (passwordBox.Password != (string)e.NewValue)
                {
                    passwordBox.Password = (string)e.NewValue ?? string.Empty;
                }
            }
        }

        public static readonly DependencyProperty BindPasswordBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "BindPasswordBehavior",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnBindPasswordBehaviorChanged));

        public static bool GetBindPasswordBehavior(DependencyObject obj) =>
            (bool)obj.GetValue(BindPasswordBehaviorProperty);

        public static void SetBindPasswordBehavior(DependencyObject obj, bool value) =>
            obj.SetValue(BindPasswordBehaviorProperty, value);

        private static void OnBindPasswordBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
                else
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // Atualizar a propriedade BoundPassword somente se necessário.
                string currentBoundPassword = GetBoundPassword(passwordBox);
                if (passwordBox.Password != currentBoundPassword)
                {
                    SetBoundPassword(passwordBox, passwordBox.Password);
                }
            }
        }
    }
}