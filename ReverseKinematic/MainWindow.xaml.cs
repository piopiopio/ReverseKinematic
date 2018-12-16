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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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
            //   this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            // MainCanvas.Children.Add(line);
            //_mainViewModel.Scene.ObstaclesCollection.Add(new RectangleObstacle(500,500,500,500));
            _mainViewModel.Scene.TurnOnAnimationModeReverseKinematic += TurnOnAnimMode;
            _mainViewModel.Scene.TurnOffAnimationModeReverseKinematic += TurnOffAnimMode;

       
        }


        private void TurnOffAnimMode(object sender, PropertyChangedEventArgs e)
        {
            TurnOffAnimtionMode(true);
        }


        void TurnOnAnimMode(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            TurnOffAnimtionMode(false);

            //glControl.Invalidate();
        }
        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //TODO: Zrobić automatyczne skalowanie.

            MainWindow1.Height = MainWindow1.Width * 9 / 16 + 24;
            //MainWindow1.Height = MainWindow1.Width * 9 / 16-24;





        }

        private void MainCanvas_OnRightMouseDown(object sender, MouseButtonEventArgs e)
        {


      

                position = rescalePoint(e.GetPosition(MainViewbox));
                _mainViewModel.Scene.ObstaclesCollection.Remove(tempRectangle);

                ////tempRectangle.Fill=new SolidColorBrush(System.Windows.Media.Colors.Black);  
                ////Canvas.SetTop(tempRectangle, position.Y);
                ////Canvas.SetLeft(tempRectangle, position.X);
                tempRectangle.From = new Point(position.X, position.Y);
                tempRectangle.Size = new Point(0, 0);
                _mainViewModel.Scene.ObstaclesCollection.Add(tempRectangle);
            

        }

        private void MainCanvas_OnLeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                _mainViewModel.Scene.StartPosition = rescalePoint(e.GetPosition(MainViewbox));
            }

            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                _mainViewModel.Scene.SelectObstacle(rescalePoint(e.GetPosition(MainViewbox)));
                moveVector = rescalePoint(e.GetPosition(MainViewbox));
            }
            else
            {
                _mainViewModel.Scene.EndPosition = rescalePoint(e.GetPosition(MainViewbox));
            }



        }


        private void MainCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            var currentPosition = rescalePoint(e.GetPosition(MainViewbox));

            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {


                _mainViewModel.Scene.MoveSelectedObstacles(new Point(-moveVector.X + currentPosition.X, -moveVector.Y + currentPosition.Y));

            }
            else
            {

                var newPosition = rescalePoint(e.GetPosition(MainViewbox));

                var Width = newPosition.X - position.X;
                var Height = newPosition.Y - position.Y;
                if (Width < 0) tempRectangle.From = new Point(newPosition.X, tempRectangle.From.Y);
                if (Height < 0) tempRectangle.From = new Point(tempRectangle.From.X, newPosition.Y);
                tempRectangle.Size = new Point(Math.Max(Math.Abs(Width),10), Math.Max(Math.Abs(Height),10));
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


        private void Calculate(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Scene.GenerateConfigurationSpaceMap();
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

        //private void L0_OnLostFocus(object sender, RoutedEventArgs e)
        //{

        //    //_mainViewModel.Scene.Robot1.L0 = _mainViewModel.Scene.Robot2.L0;
        //    //_mainViewModel.Scene.Robot1.L1 = _mainViewModel.Scene.Robot2.L1;
        //    _mainViewModel.Scene.RefreshRobots();
        //}


        private void StartAnimation_OnClick(object sender, RoutedEventArgs e)
        { 
           // AlertPopup.IsOpen = true;
            TurnOffAnimtionMode(false);
            _mainViewModel.Scene.StartSimulation();

        }

        private void TurnOffAnimtionMode(bool input)
        {
            //AlertPopup.IsOpen = false;
            StopSimulation.IsEnabled = input;
            ClearSceneButton.IsEnabled = input;
            StartAnimation.IsEnabled = input;
            L0.IsEnabled = input;
            L1.IsEnabled = input;
            AnimationLength.IsEnabled = input;
            R1.IsEnabled = input;
            R2.IsEnabled = input;
            R3.IsEnabled = input;
            R4.IsEnabled = input;
            StopAnimation.IsEnabled = !input;
        }

        private void StopAnimation_OnClick(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Scene.StopSimulation();
        }
    }
}
