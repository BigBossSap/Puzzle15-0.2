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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PracticaPuzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        
        public MainWindow()
        {
            InitializeComponent();
           
        }


        public void btnContinuar_Click(object sender, RoutedEventArgs e)
        {
            int columnes = (int)this.columnes.Value;
            int files = (int)this.files.Value;

            if (columnes > 1 && files > 0 || columnes > 0 && files > 1)
            {
                Puzzle puzzle = new Puzzle(columnes, files);
                puzzle.ShowDialog();
            }
            else
            {
                
                MessageBox.Show("Columnes i files no pot ser 1");
            }
        }

        private void NomesNums(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true; 
            }
        }

    }
}
