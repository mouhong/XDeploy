using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace XDeploy.Wpf.Framework
{
    public static class PasswordBinding
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBinding),
            new FrameworkPropertyMetadata(String.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PasswordBinding), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordBinding));

        public static string GetPassword(DependencyObject d)
        {
            return (string)d.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject d, object value)
        {
            d.SetValue(PasswordProperty, value);
        }

        public static bool GetAttach(DependencyObject d)
        {
            return (bool)d.GetValue(AttachProperty);
        }

        public static void SetAttach(DependencyObject d, object value)
        {
            d.SetValue(AttachProperty, value);
        }

        static bool GetIsUpdating(DependencyObject d)
        {
            return (bool)d.GetValue(IsUpdatingProperty);
        }

        static void SetIsUpdating(DependencyObject d, object value)
        {
            d.SetValue(IsUpdatingProperty, value);
        }

        static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;

            if (passwordBox == null)
                throw new InvalidOperationException("PasswordBinding cannot be applied to PasswordBox.");

            if (GetIsUpdating(passwordBox) == true)
            {
                return;
            }

            passwordBox.PasswordChanged -= OnPasswordChanged;
            passwordBox.Password = (string)e.NewValue;
            passwordBox.PasswordChanged += OnPasswordChanged;
        }

        static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox == null)
                return;

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= OnPasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += OnPasswordChanged;
            }
        }

        static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}
