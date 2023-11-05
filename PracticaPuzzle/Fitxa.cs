using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace PracticaPuzzle
{
    class Fitxa : Button
    {
        public Fitxa()
        {
         
            Viewbox viewbox = new Viewbox();
            TextBlock textBlock = new TextBlock();
            viewbox.Child = textBlock;
            Content = viewbox;
            Numero = -1; 
           
        }
     
        public int Numero { get; set; }
        public string DisplayText { get; set; }
        public string Text
        {
            get { return ((Viewbox)Content).Child.ToString(); }
            set
            {
                ((Viewbox)Content).Child = new TextBlock() { Text = value };
            }
        }

        public bool estaAmagat
        {
            get { return Visibility == System.Windows.Visibility.Hidden; }
            set
            {
                if (value)
                {
                    Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        public new string Tag
        {
            get { return (string)GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }
    }
}
