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

namespace ReverseKinematic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point position = new Point();
        private MainViewModel _mainViewModel;
        Rectangle tempRectangle = new Rectangle();
        public MainWindow()
        {
            InitializeComponent();
            var line = new Line();
            line.Stroke = Brushes.Black;
            line.X1 = 0;
            line.Y1 = 0;
            line.X2 = 100;
            line.Y2 = 100;
            line.StrokeThickness = 2;
            MainCanvas.Children.Add(line);

        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainWindow1.Width = MainWindow1.Height * 5 / 4;
        }

        private void MainCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.Children.Remove(tempRectangle);
            position = rescalePoint(e.GetPosition(this));
            tempRectangle.Fill=new SolidColorBrush(System.Windows.Media.Colors.Black);  
            Canvas.SetTop(tempRectangle, position.Y);
            Canvas.SetLeft(tempRectangle, position.X);
            tempRectangle.Width = 0;
            tempRectangle.Height = 0;
            MainCanvas.Children.Add(tempRectangle);  
          
        }

        private void MainCanvas_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.Children.Remove(tempRectangle);
        }

        private void MainCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            var newPosition = rescalePoint(e.GetPosition(this));
            var Width = newPosition.X - position.X;
            var Height = newPosition.Y - position.Y;
            if (Width<0) Canvas.SetLeft(tempRectangle, newPosition.X);
            if (Height<0) Canvas.SetTop(tempRectangle, newPosition.Y);
            tempRectangle.Width=Math.Abs(Width);
            tempRectangle.Height= Math.Abs(Height);
        }

        public Point rescalePoint(Point p1)
        {
            //Convert from canvas scale to backend scale
            var a=MainCanvas.Width;
            var b= MainViewbox.ActualWidth;
            var scale = b / a;
            return new Point(p1.X / scale, p1.Y / scale);
        }
    }
}
