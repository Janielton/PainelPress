using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PainelPress.Elementos
{
    public class Botao : Button
    {
    
        public Botao() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Botao),
                new FrameworkPropertyMetadata(typeof(Botao)));
        }

        #region DependencyProperty : CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius),
                typeof(Botao), new PropertyMetadata(default));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion
    }
}
