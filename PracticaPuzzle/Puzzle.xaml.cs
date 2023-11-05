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

        public Color DefaultButtonColor;

        private DispatcherTimer timer;

        private TimeSpan tempsTranscorregut;

        private bool isTimerRunning;
        private bool jocPausat;
        private const int  segons = 1;
        int clicks = 0;
        int completat = 0;
        private Fitxa?[,] fitxes; 
        private Grid? graella;
        public Puzzle(int columns, int rows) 
        {
            InitializeComponent();
            columnes = columns; 
            files = rows;                  
            DefaultButtonColor = Colors.IndianRed;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); 
            timer.Tick += Timer_Tick;
            tempsTranscorregut = TimeSpan.Zero;
            isTimerRunning = false;
            this.KeyDown += Fitxa_KeyDown;
            CreaControlsDinamics();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (isTimerRunning && !jocPausat)
            {
                tempsTranscorregut = tempsTranscorregut.Add(TimeSpan.FromSeconds(segons));
                StatusText.Text = tempsTranscorregut.ToString(@"hh\:mm\:ss");
               
            }
        }

        private void Pausa()
        {
            jocPausat = !jocPausat;
            if (jocPausat)
            {
                // Pausem
                timer.Stop();
                foreach (Fitxa button in graella.Children)
                {
                    button.IsEnabled = false;
                }              
                StatusText.Text = "Pausa";               
            }
            else
            {
                // Resume
                timer.Start();
                foreach (Fitxa button in graella.Children)
                {
                    button.IsEnabled = true;
                }
                StatusText.Text = tempsTranscorregut.ToString(@"hh\:mm\:ss");              
            }
        }
        private void CreaControlsDinamics()
        {
            graella = new Grid();          
           

            // Crear files i columnes
            
            CreaFiles(graella, files);
            CreaColumnes(graella, columnes);

            // Afegir la graella al grid

            baseGrid.Children.Add(graella);
            Grid.SetRow(graella, 0);
            Grid.SetColumn(graella, 0);

            //Inicialitzar array de Fitxa
            
            fitxes = new Fitxa[files, columnes]; 

            List<int> numbers = Enumerable.Range(1, files * columnes - 1).ToList(); //Per crear una llista consecutiva de numeros

            //Shuffle fins que sigui resoluble, n2 cops

            int n2 = numbers.Count * numbers.Count;

            do
            {
                Shuffle(numbers,n2);
            } while (!esResoluble(numbers));

            numbers.Add(-1); // -1 representara el l'espai buit
            int index = 0; // Index per mourens per l'array de Fitxa

            for (int fila = 0; fila < files; fila++)
            {
                for (int columna = 0; columna < columnes; columna++)    //Per cada posicio de la graella:
                {
                    Fitxa fitxa = new Fitxa();                
                    int numero = numbers[index]; // Agafem un numero
                    fitxa.Text = numero == -1 ? "" : numero.ToString(); // Si es el -1 , no posem text
                    graella.Margin = new Thickness(10);
                    graella.Background = new SolidColorBrush(Colors.Aquamarine);
                    graella.Children.Add(fitxa);

                    // Calculem quina seria la posició correcte del numero
                    int filaCorrecte = (numero - 1) / columnes;
                    int columnaCorrecte = (numero - 1) % columnes;

                    // Posem un tag per mes endavant cambiar el color si es correcte
                    fitxa.Tag = $"{filaCorrecte},{columnaCorrecte}";
                    fitxa.Margin = new Thickness(6);

                    // Coloquem la fitxa
                    Grid.SetRow(fitxa, fila);
                    Grid.SetColumn(fitxa, columna);                    

                    fitxes[fila, columna] = fitxa; // La posem a l'array
                    index++;

                    if (fila == files - 1 && columna == columnes - 1) // si es l'ultima posicio, amagem el numero 
                    {
                        fitxa.estaAmagat = true;
                        fitxa.Text = ""; 
                    }

                    if (fitxa.Tag.ToString() == $"{fila},{columna}") // Si la posicio coincideix amb el tag, posem el color verd
                    {
                        fitxa.Background = new SolidColorBrush(Colors.Green);
                       
                    }
                    else fitxa.Background = new SolidColorBrush(DefaultButtonColor);
                    fitxa.Click += Button_Click; 
                }
            }


            //completat

            int botonsCorrectes = 0;

            for (int fila = 0; fila < files; fila++)
            {
                for (int columna = 0; columna < columnes; columna++)
                {
                    string[] tagParts = fitxes[fila, columna].Tag.ToString().Split(',');
                    int filaCorrecte = int.Parse(tagParts[0]);
                    int columnaCorrecte = int.Parse(tagParts[1]);

                    if (fila == filaCorrecte && columna == columnaCorrecte)
                    {
                        botonsCorrectes++;
                    }
                }
            }

            // Actualitzem el percentatge de completat
            int totalButtons = columnes * files-1;
            sbCompletat.Text = (botonsCorrectes * 100 / totalButtons) + "%";


        }

        private void Fitxa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {               
                Button_Click(sender, new RoutedEventArgs());
            }

            else if (e.Key == Key.Escape)
            {
                Pausa();
                e.Handled = true;
            }

           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Fitxa botoClickat = (Fitxa)sender;

         
            //cronometre

            if (!isTimerRunning)
            {
                timer.Start();
                isTimerRunning = true;
            }
            // Aconseguim la fila i columna de la fitxa
            int filaClick = Grid.GetRow(botoClickat);
            int columnaClick = Grid.GetColumn(botoClickat);



            int filaAmbForat=-1;
            int columnaAmbForat=-1; 
            //Busquem l'espai buit
            bool trobat = false;
            while (!trobat) { 
            for (int fila = 0; fila < files; fila++)
            {
                for (int columna = 0; columna < columnes; columna++)
                {
                    if (fitxes[fila, columna].estaAmagat)
                    {
                        filaAmbForat = fila;
                        columnaAmbForat = columna;
                        trobat = true;
                    }
                }
            }

        }

            // Comprovem que la fitxa o columna clickada estigui al costat de l'espai buit
            if (filaClick == filaAmbForat || columnaClick == columnaAmbForat)
            {
                int diferenciaFila = Math.Abs(filaClick - filaAmbForat);
                int diferenciaColumna = Math.Abs(columnaClick - columnaAmbForat);

                if ((diferenciaFila == 1 && diferenciaColumna == 0) || (diferenciaFila == 0 && diferenciaColumna == 1)) // Si la diferencia es 1, estan al costat, ja sigui fila o columna
                {
                   
                    CambiaPosicio(filaClick, columnaClick, filaAmbForat, columnaAmbForat);
                    clicks++;
                  
                }
                else
                {
                    if (filaClick == filaAmbForat)
                    {
                        // Si el boto buit esta a la primera fila movem tots els botons
                        if (columnaClick < columnaAmbForat)
                        {
                            // cap a la dreta
                            for (int columna = columnaAmbForat - 1; columna >= columnaClick; columna--)
                            {
                                CambiaPosicio(filaClick, columna + 1, filaClick, columna);
                               
                                clicks++;
                            }
                        }
                        else if (columnaClick > columnaAmbForat)
                        {
                            // esquerra
                            for (int columna = columnaAmbForat + 1; columna <= columnaClick; columna++)
                            {
                                CambiaPosicio(filaClick, columna - 1, filaClick, columna);
                                
                                clicks++;
                            }
                        }
                    }
                    else if (columnaClick == columnaAmbForat)
                    {
                        // el mateix amb les columnes
                        if (filaClick < filaAmbForat)
                        {
                            // cap avalla
                            for (int fila = filaAmbForat - 1; fila >= filaClick; fila--)
                            {
                                CambiaPosicio(fila + 1, columnaClick, fila, columnaClick);
                               
                                clicks++;
                            }
                        }
                        else if (filaClick > filaAmbForat)
                        {
                            // cap amunt
                            for (int fila = filaAmbForat + 1; fila <= filaClick; fila++)
                            {
                                CambiaPosicio(fila - 1, columnaClick, fila, columnaClick);
                             
                                clicks++;
                            }
                        }
                    }
                }

                CambiarColor();

            }
            sbClicks.Text = clicks.ToString();

            //completat

            int botonsCorrectes = 0;

            for (int fila = 0; fila < files; fila++)
            {
                for (int columna = 0; columna < columnes; columna++)
                {
                    string[] tagParts = fitxes[fila, columna].Tag.ToString().Split(',');
                    int filaCorrecte = int.Parse(tagParts[0]);
                    int columnaCorrecte = int.Parse(tagParts[1]);

                    if (fila == filaCorrecte && columna == columnaCorrecte)
                    {
                        botonsCorrectes++;
                    }
                }
            }

            
            int totalBotons = columnes * files - 1;
            sbCompletat.Text = (botonsCorrectes * 100 / totalBotons) + "%";

            if (sbCompletat.Text == "100%")
            {
                MessageBoxResult result = MessageBox.Show("Has guanyat!", "Felicitats", MessageBoxButton.OK);
                if (result == MessageBoxResult.OK)
                {
                    
                    this.Close(); 
                }
            }
        }

        private void CambiarColor()
        {
            for (int fila = 0; fila < files; fila++)
            {
                for (int columna = 0; columna < columnes; columna++)
                {
                    Fitxa fitxa = fitxes[fila, columna];
                    string[] posicioTag = fitxa.Tag.ToString().Split(',');
                    int filaCorrecte = int.Parse(posicioTag[0]);
                    int columnaCorrecte = int.Parse(posicioTag[1]);

                    if (fila == filaCorrecte && columna == columnaCorrecte)
                    {
                        fitxa.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        fitxa.Background = new SolidColorBrush(DefaultButtonColor);
                    }
                }
            }
        }


        private void CambiaPosicio(int row1, int col1, int row2, int col2)
        {
            Fitxa tempFitxa = fitxes[row1, col1];
            fitxes[row1, col1] = fitxes[row2, col2];
            fitxes[row2, col2] = tempFitxa;
            
        
            Grid.SetRow(fitxes[row1, col1], row1);
            Grid.SetColumn(fitxes[row1, col1], col1);
            Grid.SetRow(fitxes[row2, col2], row2);
            Grid.SetColumn(fitxes[row2, col2], col2);

            
        }

        private static void Shuffle<T>(IList<T> list, int numberOfShuffles)
        {
            Random rnd = new Random();

            for (int shuffleCount = 0; shuffleCount < numberOfShuffles; shuffleCount++)
            {
                int n = list.Count;

                while (n > 1)
                {
                    n--;
                    int k = rnd.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }
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

        private bool esResoluble(List<int> numbers)
        {
            int inversions = 0;
            bool resoluble = false;
            for (int i = 0; i < numbers.Count; i++)
            {
                for (int j = i + 1; j < numbers.Count; j++)
                {
                    if (numbers[i] > numbers[j])
                    {
                        inversions++;
                    }
                }
            }

            // Si es parell es resoluble
            if (inversions % 2 == 0)
            {
                resoluble = true;
            }
            else
            {
                // si es imparell, swap els dos ultims
                int lastIndex = numbers.Count - 1;

                int temp = numbers[lastIndex];
                numbers[lastIndex] = numbers[lastIndex - 1];
                numbers[lastIndex - 1] = temp;

                resoluble = true;
            }

            return resoluble;
        }


    }
}
