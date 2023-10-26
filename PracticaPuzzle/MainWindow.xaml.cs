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
            int columnsValue = int.Parse(columnes.Text);
            int filesValue = int.Parse(files.Text);

            Puzzle puzzle = new Puzzle(columnsValue,filesValue);
            puzzle.columnes = columnsValue;
            puzzle.files = filesValue;

            puzzle.ShowDialog();
        }

    }
}
