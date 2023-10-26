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
using System.Windows.Threading;

namespace PracticaPuzzle
{
    /// <summary>
    /// Lógica de interacción para Puzzle.xaml
    /// </summary>
    public partial class Puzzle : Window
    {
        public int columnes { get; set; }
        public int files { get; set; }
        public Color DefaultButtonColor { get; private set; }

        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        private bool isTimerRunning;
        int clicks = 0;
        private Fitxa?[,] fitxes; // Declare an array of Fitxa
        private Grid? graella;
        public Puzzle(int columns, int rows) // Pass the values as constructor parameters
        {
            InitializeComponent();
            columnes = columns; // Set the properties here
            files = rows;       // Set the properties here
            
            DefaultButtonColor = Colors.Red;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Update every second
            timer.Tick += Timer_Tick;

            // Additional initialization
            elapsedTime = TimeSpan.Zero;
            isTimerRunning = false;

            CreaControlsDinamics();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (isTimerRunning)
            {
                elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
                StatusText.Text = elapsedTime.ToString(@"hh\:mm\:ss");
            }
        }

      
        private void CreaControlsDinamics()
        {
            graella = new Grid();          
            graella.ShowGridLines = true;
            
            CreaFiles(graella, files);
            CreaColumnes(graella, columnes);

            baseGrid.Children.Add(graella);
            Grid.SetRow(graella, 0);
            Grid.SetColumn(graella, 0);


            fitxes = new Fitxa[files, columnes]; // Initialize the array

            List<int> numbers = Enumerable.Range(1, files * columnes - 1).ToList(); // Create a list of consecutive numbers (one less)
            int shuffles= Shuffle(numbers); // Shuffle the list
            numbers.Add(-1); // Add -1 to the end to represent the last hidden button

            int index = 0; // Index to iterate through the shuffled numbers

            for (int fila = 0; fila < files; fila++)
            {
                for (int columna = 0; columna < columnes; columna++)
                {
                    Fitxa fitxa = new Fitxa();
                    fitxa.Tag = $"{fila + 1},{columna + 1}"; // Set the tag to the row and column
                    int number = numbers[index]; // Get the number
                    fitxa.Text = number == -1 ? "" : number.ToString(); // Set the text (or empty for -1)
                    graella.Children.Add(fitxa);

                    // Calculate the row and column based on the number
                    int correctRow = (number - 1) / columnes;
                    int correctColumn = (number - 1) % columnes;

                    // Set the Tag property with the correct position
                    fitxa.Tag = $"{correctRow},{correctColumn}";

                    // Set row and column for the Fitxa
                    Grid.SetRow(fitxa, fila);
                    Grid.SetColumn(fitxa, columna);                    
                    fitxes[fila, columna] = fitxa; // Add the Fitxa to the array
                    index++;
                    if (fila == files - 1 && columna == columnes - 1)
                    {
                        fitxa.estaAmagat = true;
                        fitxa.Text = ""; // Clear the text for the last button
                    }

                    if (fitxa.Tag.ToString() == $"{fila},{columna}")
                    {
                        fitxa.Background = new SolidColorBrush(Colors.Green);
                    }


                    
                    if (fitxa.Tag.ToString() == $"{fila},{columna}")
                    {
                        fitxa.Background = new SolidColorBrush(Colors.Green);
                    }

                    else fitxa.Background = new SolidColorBrush(Colors.Red);
                    fitxa.Click += Button_Click; // Add the click event handler
                    fitxa.KeyDown += Fitxa_KeyDown; // Add the key down event handler
                }
            }

           
        }

        private void Fitxa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {               
                Button_Click(sender, new RoutedEventArgs());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Fitxa clickedButton = (Fitxa)sender;
           

            

            if (!isTimerRunning)
            {
                timer.Start();
                isTimerRunning = true;
            }
            // Get the row and column of the clicked button
            int clickedRow = Grid.GetRow(clickedButton);
            int clickedColumn = Grid.GetColumn(clickedButton);

            
            //ChangeColor(clickedRow,clickedButton,clickedColumn);
            // Get the row and column of the hidden space
            int hiddenRow = -1;
            int hiddenColumn = -1;

            // Find the hidden space
            for (int fila = 0; fila < files; fila++)
            {
                for (int columna = 0; columna < columnes; columna++)
                {
                    if (fitxes[fila, columna].estaAmagat)
                    {
                        hiddenRow = fila;
                        hiddenColumn = columna;
                        break;
                    }
                }
            }

            // Check if the clicked button is in the same row or column as the hidden space
            if (clickedRow == hiddenRow || clickedColumn == hiddenColumn)
            {
                int rowDifference = Math.Abs(clickedRow - hiddenRow);
                int columnDifference = Math.Abs(clickedColumn - hiddenColumn);

                if ((rowDifference == 1 && columnDifference == 0) || (rowDifference == 0 && columnDifference == 1))
                {
                    // If the clicked button is adjacent to the hidden space, swap positions
                    CambiaPosicio(clickedRow, clickedColumn, hiddenRow, hiddenColumn);
                    clicks++;
                    CambiarColor(clickedRow, clickedButton, clickedColumn); // Update the color after the swap
                    CambiarColor(hiddenRow, fitxes[hiddenRow, hiddenColumn], hiddenColumn); // Update the color of the hidden button
                }
                else
                {
                    if (clickedRow == hiddenRow)
                    {
                        // If the clicked button is in the same row, move all buttons in the row closer to the hidden space
                        if (clickedColumn < hiddenColumn)
                        {
                            // Move buttons to the right
                            for (int columna = hiddenColumn - 1; columna >= clickedColumn; columna--)
                            {
                                CambiaPosicio(clickedRow, columna + 1, clickedRow, columna);
                                clicks++;
                            }
                        }
                        else if (clickedColumn > hiddenColumn)
                        {
                            // Move buttons to the left
                            for (int columna = hiddenColumn + 1; columna <= clickedColumn; columna++)
                            {
                                CambiaPosicio(clickedRow, columna - 1, clickedRow, columna);
                                clicks++;
                            }
                        }
                    }
                    else if (clickedColumn == hiddenColumn)
                    {
                        // If the clicked button is in the same column, move all buttons in the column closer to the hidden space
                        if (clickedRow < hiddenRow)
                        {
                            // Move buttons down
                            for (int fila = hiddenRow - 1; fila >= clickedRow; fila--)
                            {
                                CambiaPosicio(fila + 1, clickedColumn, fila, clickedColumn);
                               clicks++;
                            }
                        }
                        else if (clickedRow > hiddenRow)
                        {
                            // Move buttons up
                            for (int fila = hiddenRow + 1; fila <= clickedRow; fila++)
                            {
                                CambiaPosicio(fila - 1, clickedColumn, fila, clickedColumn);
                                clicks++;
                            }
                        }
                    }


                }

              

            }
            sbClicks.Text = clicks.ToString();

        }

        private void CambiarColor(int clickedRow, Fitxa clickedButton, int clickedColumn )
        {
            string[] tagParts = clickedButton.Tag.ToString().Split(',');
            int expectedRow = int.Parse(tagParts[0]);
            int expectedColumn = int.Parse(tagParts[1]);

            // Check if the clicked button is in the correct position
            if (clickedRow == expectedRow && clickedColumn == expectedColumn)
            {
                // Set the background color of the clicked button to green
                clickedButton.Background = new SolidColorBrush(Colors.Green);
            }
            else
            {
                // Set the background color of the clicked button to another color to indicate it's been moved
                clickedButton.Background = new SolidColorBrush(Colors.Red); // Change to your desired color
            }


        }

        private void CambiaPosicio(int row1, int col1, int row2, int col2)
        {
            Fitxa tempFitxa = fitxes[row1, col1];
            fitxes[row1, col1] = fitxes[row2, col2];
            fitxes[row2, col2] = tempFitxa;
            
            // Update the row and column properties of the Fitxa buttons
            Grid.SetRow(fitxes[row1, col1], row1);
            Grid.SetColumn(fitxes[row1, col1], col1);
            Grid.SetRow(fitxes[row2, col2], row2);
            Grid.SetColumn(fitxes[row2, col2], col2);

            
        }

        private static int Shuffle<T>(IList<T> list)
        {
            Random rnd = new Random();
            int n = list.Count;
            int shuffles = 0;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
                shuffles++;
            }

            return shuffles;
        }

        private void CreaFiles(Grid graella, int numFiles)
        {
            for (int fila = 0; fila < numFiles; fila++)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(1, GridUnitType.Star); // per pasarli parametres la fila, en aquest cas 1, i el tipus de mesura, en aquest cas estrelles
                graella.RowDefinitions.Add(rd);


            }
        }

        public void CreaColumnes( Grid graella, int numColumnes) //Important el this 
        {
            for (int columna = 0; columna < numColumnes; columna++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star); // per pasarli parametres la fila, en aquest cas 1, i el tipus de mesura, en aquest cas estrelles
                graella.ColumnDefinitions.Add(cd);
            }
        }
    }
}
