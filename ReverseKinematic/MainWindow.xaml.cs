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
using System.Windows.Markup;
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
        private Point moveVector = new Point();
        private MainViewModel _mainViewModel = new MainViewModel();
        RectangleObstacle tempRectangle = new RectangleObstacle();
        public MainWindow()
        {
            InitializeComponent();

            DataContext = _mainViewModel;
            var line = new Line();
            line.Stroke = Brushes.Black;
            line.X1 = 0;
            line.Y1 = 0;
            line.X2 = 100;
            line.Y2 = 100;
            line.StrokeThickness = 2;
            // MainCanvas.Children.Add(line);
            //_mainViewModel.Scene.ObstaclesCollection.Add(new RectangleObstacle(500,500,500,500));

        }



        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //TODO: Zrobić automatyczne skalowanie.
            // MainWindow1.Width = MainWindow1.Height * 16 / 9-23;
            MainWindow1.Height = MainWindow1.Width * 9 / 16 + 23;




        }

        private void MainCanvas_OnLeftMouseDown(object sender, MouseButtonEventArgs e)
        {


            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                _mainViewModel.Scene.SelectObstacle(rescalePoint(e.GetPosition(this)));
                moveVector = rescalePoint(e.GetPosition(this));
            }
            else
            {
                position = rescalePoint(e.GetPosition(this));
                _mainViewModel.Scene.ObstaclesCollection.Remove(tempRectangle);

                ////tempRectangle.Fill=new SolidColorBrush(System.Windows.Media.Colors.Black);  
                ////Canvas.SetTop(tempRectangle, position.Y);
                ////Canvas.SetLeft(tempRectangle, position.X);
                tempRectangle.From = new Point(position.X, position.Y);
                tempRectangle.Size = new Point(0, 0);
                _mainViewModel.Scene.ObstaclesCollection.Add(tempRectangle);
            }

        }

        private void MainCanvas_OnLeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                //  _mainViewModel.Scene.SelectObstacle(rescalePoint(e.GetPosition(this)));
            }
            else
            {
                _mainViewModel.Scene.ObstaclesCollection.Add(tempRectangle.Clone());
                _mainViewModel.Scene.ObstaclesCollection.Remove(tempRectangle);
            }
        }


        private void MainCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            var currentPosition = rescalePoint(e.GetPosition(this));

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {


                _mainViewModel.Scene.MoveSelectedObstacles(new Point(-moveVector.X + currentPosition.X, -moveVector.Y + currentPosition.Y));

            }
            else
            {

                var newPosition = rescalePoint(e.GetPosition(this));
                var Width = newPosition.X - position.X;
                var Height = newPosition.Y - position.Y;
                if (Width < 0) tempRectangle.From = new Point(newPosition.X, tempRectangle.From.Y);
                if (Height < 0) tempRectangle.From = new Point(tempRectangle.From.X, newPosition.Y);
                tempRectangle.Size = new Point(Math.Abs(Width), Math.Abs(Height));
            }
            moveVector = currentPosition;
        }

        public Point rescalePoint(Point p1)
        {
            // Convert from canvas scale to backend scale
            //var a = MainCanvas.Width;
            double a = 1000;
            var b = MainViewbox.ActualWidth;
            var scale = b / a;
            // return p1;
            return new Point(p1.X / scale, p1.Y / scale);

        }


        private void MainCanvas_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TargetEllipse.Visibility = Visibility.Visible;
            _mainViewModel.Scene.Target = rescalePoint(e.GetPosition(this));


        }


        private void Calculate(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Scene.GeneratePath();
        }

        private void ClearScene(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Scene.ObstaclesCollection.Clear();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _mainViewModel.Scene.RemoveSelectedObstacles();
            }
        }
    }
}
