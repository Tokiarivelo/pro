using System.Windows;
using System.Windows.Controls;

namespace CustomWindow.AttachProperties
{
    class PasswordBoxProperties
    {
        public static readonly DependencyProperty HasTextProperty = DependencyProperty.RegisterAttached("HasTExt",typeof(bool), typeof(PasswordBoxProperties),new PropertyMetadata(false));

        private static void SetHasText(PasswordBox element)
        {
            element.SetValue(HasTextProperty, element.SecurePassword.Length > 0);
        }

        public static bool GetHasTExt(PasswordBox element)
        {
            return (bool) element.GetValue(HasTextProperty);
        }
    }
}
