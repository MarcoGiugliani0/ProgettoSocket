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

namespace Host2
{
    /// <summary>
    /// Logica di interazione per NomeUtente.xaml
    /// </summary>
    public partial class NomeUtente : Window
    {
        public NomeUtente()
        {
            InitializeComponent();
        }

        public string Nome;

        private void btnInizia_Click(object sender, RoutedEventArgs e)
        {
            if (txtNome.Text == "")
            {
                MessageBox.Show("Devi inserire il tuo nome!", "ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                Nome = txtNome.Text;
                Close();
            }
        }
    }
}
