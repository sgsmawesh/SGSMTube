using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SGSMTube.UserControls
{
    /// <summary>
    /// Interaction logic for HintTextBox.xaml
    /// </summary>
    public partial class HintTextBox : UserControl
    {
        public HintTextBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
               "Placeholder",
               typeof(string),
               typeof(HintTextBox),
               new FrameworkPropertyMetadata("Input here"));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
               "Text",
               typeof(string),
               typeof(HintTextBox),
               new FrameworkPropertyMetadata(string.Empty));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Clear();
            txtInput.Focus();
        }

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblPlaceholder.Visibility = string.IsNullOrEmpty(txtInput.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
