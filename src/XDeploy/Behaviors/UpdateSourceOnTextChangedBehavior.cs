using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace XDeploy.Behaviors
{
    public class UpdateSourceOnTextChangedBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += OnTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            var textBox = (sender as TextBox);
            if (textBox != null)
            {
                var binding = textBox.GetBindingExpression(TextBox.TextProperty);

                if (binding != null)
                    binding.UpdateSource();
            }
        }
    }
}
