using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SGSMTube.Pages
{
    /// <summary>
    /// Interaction logic for SelectionViewerDialog.xaml
    /// </summary>
    public partial class SelectionViewerDialog : Window
    {
        public SelectionViewerDialog(Window owner)
        {
            InitializeComponent();
            this.Owner = owner;
            this.Topmost = false;
            ShowInTaskbar = false;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
