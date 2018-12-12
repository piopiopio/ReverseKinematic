using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xaml;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace ReverseKinematic
{
    public class Scene : ViewModelBase
    {
        private Robot _robot1;//, _robot2;
        private bool _alternativeSolution = false;
        public bool AlternativeSolution
        {
            get
            {
                return _alternativeSolution;
            }
            set
            {
                _alternativeSolution = value;
                OnPropertyChanged(nameof(Robot));
            }
        }

        private bool _showFirst = true;
        private bool _showFirstPermission = true;

        public bool ShowFirst
        {
            get { return _showFirst && _showFirstPermission; }
            set
            {
                _showFirst = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Robot1));
            }
        }

        private bool _showSecond = true;
        private bool _showSecondPermission = true;

        public bool ShowSecond
        {
            get { return _showSecond && _showSecondPermission; }
            set
            {
                _showSecond = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Robot1));
            }
        }
        //public Robot Robot
        //{

        //    get
        //    {
        //        return _robot1;
        //    }

        //    set
        //    {

        //    }
        //}

        public Robot Robot1
        {
            get
            {

                return _robot1;

            }
            set
            {
                _robot1 = value;
                OnPropertyChanged();
            }
        }

        //public Robot Robot2
        //{
        //    get
        //    {

        //        return _robot2;

        //    }
        //    set
        //    {
        //        _robot2 = value;
        //        OnPropertyChanged();
        //    }
        //}
        public ObservableCollection<RectangleObstacle> ObstaclesCollection { get; private set; }


        private Point _startPosition;

        public Point StartPosition
        {
            get { return _startPosition; }
            set
            {
                _startPosition = value;


                var tempAngles = CalculateArmAnglesForPosition(_startPosition);

                if (double.IsNaN(tempAngles[0]) || double.IsNaN(tempAngles[1]))
                {
                    //MessageBox.Show("Config 1-> error");
                }
                else
                {
                    _robot1.Alpha0 = tempAngles[0];
                    _robot1.Alpha1 = tempAngles[1];
                }

                if (double.IsNaN(tempAngles[2]) || double.IsNaN(tempAngles[3]))
                {
                    // MessageBox.Show("Config 2-> error");
                }
                else
                {
                    _robot1.Alpha0bis = tempAngles[2];
                    _robot1.Alpha1bis = tempAngles[3];
                    OnPropertyChanged(nameof(Robot));
                }

                CollisionCheck();
                OnPropertyChanged();
                OnPropertyChanged(nameof(Robot));
            }
        }

        public void SelectObstacle(Point p)
        {
            foreach (var item in ObstaclesCollection)
            {
                item.Select(p);
            }
        }

        public Scene()
        {
            _robot1 = new Robot();
            _robot1.L0 = 200;
            _robot1.L1 = 200;
            _robot1.Alpha0 = Math.PI / 6;
            _robot1.Alpha1 = Math.PI / 3;
            _robot1.Alpha0bis = -Math.PI / 6;
            _robot1.Alpha1bis = -Math.PI / 3;
            //_robot2 = _robot1.Clone();

            ObstaclesCollection = new ObservableCollection<RectangleObstacle>();
            bitmapHelper.SetColor(240, 248, 255);
            bitmapHelper.SetColor(120, 124, 128);
            _startPosition = new Point(100, 100);
        }

        int[,] ConfigurationSpaceArray = new int[360, 360];
        //ObservableCollection<RectangleObstacle> _configurationSpace=new ObservableCollection<RectangleObstacle>();

        //public ObservableCollection<RectangleObstacle> ConfigurationSpace
        //{//TODO: Przerobić na pisanie po bitmapie
        //    get { return _configurationSpace; }
        //    set
        //    {
        //        _configurationSpace = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public WriteableBitmap _configurationSpaceBitmap = new WriteableBitmap(
        //    360, 360,
        //    96,
        //    96,
        //    PixelFormats.Bgr32,
        //    null);
        public void MoveSelectedObstacles(Point delta)
        {
            foreach (var item in ObstaclesCollection.Where(x => x.Selected == true))
            {
                item.MoveObstacle(delta);
            }
        }

        public void RemoveSelectedObstacles()
        {
            List<RectangleObstacle> tempList = new List<RectangleObstacle>();
            foreach (var item in ObstaclesCollection.Where(x => x.Selected == true))
            {
                tempList.Add(item);
            }

            foreach (var item in tempList)
            {
                ObstaclesCollection.Remove(item);
            }
        }
        public WriteableBitmap ConfigurationSpaceBitmap
        {
            get { return bitmapHelper.MakeBitmap(96, 96); }
            set
            {
                // _configurationSpaceBitmap = value;
                //OnPropertyChanged();
            }
        }

        private Point _endPosition;
        public Point EndPosition
        {
            get
            {
                return _endPosition;

            }
            set
            {
                _endPosition = value;
                OnPropertyChanged();
            }
        }

        BitmapHelper bitmapHelper = new BitmapHelper(360, 360);
        void GetObstaclesInConfigurationSpace()
        {
            // _configurationSpace.Clear();
            Robot tempRobot = Robot1.Clone();
            bitmapHelper.SetColor(240, 248, 255);

            for (int i = 0; i < 360; i++)
            {
                for (int j = 0; j < 360; j++)
                {
                    ConfigurationSpaceArray[i, j] = 0;
                    // ConfigurationSpaceArray[i+ j*360] = 0;
                    foreach (var item in ObstaclesCollection)
                    {
                        tempRobot.Alpha0 = Math.PI * i / 180;
                        tempRobot.Alpha1 = Math.PI * j / 180;

                        if (item.CollisionCheck(tempRobot.Point0, tempRobot.Point1) ||
                            item.CollisionCheck(tempRobot.Point1, tempRobot.Point2))
                        {
                            ConfigurationSpaceArray[i, j] = -1;
                            //ConfigurationSpaceBitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(0, 0, 0));
                            //bitmapHelper.SetPixel(i,j,255,0,0);

                        }
                    }
                }
            }

            int a0 = (int)(Robot1.Alpha0 * 180 / Math.PI);
            if (a0 < 0)
            {
                a0 = (int)((2 * Math.PI + Robot1.Alpha0) * 180 / Math.PI);
            }
            int a1 = (int)(Robot1.Alpha1 * 180 / Math.PI);
            if (a1 < 0)
            {
                a1 = (int)((2 * Math.PI + Robot1.Alpha1) * 180 / Math.PI);
            }
            arrayFloodFill(ConfigurationSpaceArray, a0, a1);

            for (int i = 0; i < 360; i++)
            {
                for (int j = 0; j < 360; j++)
                {
                    if (ConfigurationSpaceArray[i, j] != -1)
                    {
                        bitmapHelper.SetPixel(i, j, 0, (byte)(ConfigurationSpaceArray[i, j] * 255 / 720), 0);
                    }
                    else
                    {
                        bitmapHelper.SetPixel(i, j, 255, 0, 0);
                        ConfigurationSpaceArray[i, j] = -1;
                    }
                }
            }

            double[] startAngles = CalculateArmAnglesForPosition(StartPosition);
            double[] endAngles = CalculateArmAnglesForPosition(EndPosition);

            //bitmapHelper.SetPixel((int)(startAngles[0] *180/Math.PI), (int)(startAngles[1] * 180 / Math.PI), 255, 255, 255);
            //bitmapHelper.SetPixel((int)(startAngles[2] * 180 / Math.PI), (int)(startAngles[3] * 180 / Math.PI), 255, 255, 255);
            //bitmapHelper.SetPixel((int)(endAngles[0] * 180 / Math.PI), (int)(endAngles[1] * 180 / Math.PI), 255, 255, 255);
            //bitmapHelper.SetPixel((int)(endAngles[2] * 180 / Math.PI), (int)(endAngles[3] * 180 / Math.PI), 255, 255, 255);

            List<int[]> Path = new List<int[]>();
            Path.Add(new int[3] { (int)(endAngles[0]*180/Math.PI), (int)(endAngles[1] * 180 / Math.PI), ConfigurationSpaceArray[(int)(endAngles[0] * 180 / Math.PI), (int)(endAngles[1] * 180 / Math.PI)] });
            int value = ConfigurationSpaceArray[Path.Last()[0], Path.Last()[1]];

            while (value>1)
            {
                 

               

                if (ConfigurationSpaceArray[wrapAngles(Path.Last()[0] + 1), wrapAngles(Path.Last()[1])] == (value - 1))
                {
                    Path.Add(new int[3] { wrapAngles(Path.Last()[0] + 1), wrapAngles(Path.Last()[1] ), wrapAngles(ConfigurationSpaceArray[Path.Last()[0]+1, Path.Last()[1]]) });
                }

                else if (ConfigurationSpaceArray[wrapAngles(Path.Last()[0] - 1), wrapAngles(Path.Last()[1])] == (value - 1))
                {
                    {
                        Path.Add(new int[3] { wrapAngles(Path.Last()[0] - 1), wrapAngles(Path.Last()[1] ), wrapAngles(ConfigurationSpaceArray[Path.Last()[0]-1, Path.Last()[1]]) });
                    }
                }

                else if (ConfigurationSpaceArray[wrapAngles(Path.Last()[0]), wrapAngles(Path.Last()[1]+1)] == (value - 1))
                {
                    {
                        Path.Add(new int[3] { wrapAngles(Path.Last()[0]), wrapAngles(Path.Last()[1]+1 ), wrapAngles(ConfigurationSpaceArray[Path.Last()[0], Path.Last()[1]+1]) });
                    }
                }
                else if (ConfigurationSpaceArray[wrapAngles(Path.Last()[0]), wrapAngles(Path.Last()[1] - 1)] == (value - 1))
                {
                    {
                        Path.Add(new int[3] { wrapAngles(Path.Last()[0]), wrapAngles(Path.Last()[1] - 1 ), wrapAngles(ConfigurationSpaceArray[Path.Last()[0], Path.Last()[1]-1]) });
                    }
                }

                else
                {
                    MessageBox.Show("Path not found");
                }

                value = ConfigurationSpaceArray[wrapAngles(Path.Last()[0]), wrapAngles(Path.Last()[1])];

            }

            foreach (var item in Path)
            {
                bitmapHelper.SetPixel(item[0],item[1],255,255,255);
            }
            OnPropertyChanged(nameof(ConfigurationSpaceBitmap));
        }

        public int wrapAngles(int input)
        {
            int temp = input;
            while (true)
            {
                if (temp >= 360)
                {
                    temp -= 360;
                }
                else if (temp < 0)
                {
                    temp += 360;
                }
                else
                {
                    return temp;
                }
            }
        }
        public void GenerateConfigurationSpaceMap()
        {
            GetObstaclesInConfigurationSpace();
            CollisionCheck();
        }

        void arrayFloodFill(int[,] ConfigurationSpaceArray, int positionX, int positionY, int colorToChange = 0)
        {


            if (ConfigurationSpaceArray[positionX, positionY] != colorToChange)
            {
                return;
            }

            // int i = 0;
            var toFill = new List<int[]>();

            toFill.Add(new int[3] { positionX, positionY, 1 });

            while (toFill.Any())
            {
                var p = toFill[0];
                toFill.RemoveAt(0);

                ConfigurationSpaceArray[p[0], p[1]] = p[2];



                if ((p[0] + 1) < ConfigurationSpaceArray.GetLength(0))
                {
                    if ((ConfigurationSpaceArray[p[0] + 1, p[1]] == colorToChange) &&
                        !toFill.Any(t => (t[0] == (p[0] + 1)) && (t[1] == p[1])))
                    {
                        toFill.Add(new int[3] { p[0] + 1, p[1], p[2] + 1 });
                    }
                }

                if ((p[0] - 1) >= 0)
                {
                    if ((ConfigurationSpaceArray[p[0] - 1, p[1]] == colorToChange) &&
                        !toFill.Any(t => (t[0] == (p[0] - 1)) && (t[1] == p[1])))
                    {
                        toFill.Add(new int[3] { p[0] - 1, p[1], p[2] + 1 });
                    }
                }

                if ((p[1] + 1) < ConfigurationSpaceArray.GetLength(1))
                {
                    if ((ConfigurationSpaceArray[p[0], p[1] + 1] == colorToChange) &&
                        !toFill.Any(t => (t[0] == p[0]) && (t[1] == (p[1] + 1))))
                    {
                        toFill.Add(new int[3] { p[0], p[1] + 1, p[2] + 1 });
                    }
                }

                if ((p[1] - 1) >= 0)
                {

                    if ((ConfigurationSpaceArray[p[0], p[1] - 1] == colorToChange) &&
                                            !toFill.Any(t => (t[0] == p[0]) && (t[1] == (p[1] - 1))))
                    {
                        toFill.Add(new int[3] { p[0], p[1] - 1, p[2] + 1 });
                    }
                }

                // i++;


            }





        }

        public double[] CalculateArmAnglesForPosition(Point position)
        {


            var P = position - new Point(500, 500);
            var L0 = Robot1.L0;
            var L1 = Robot1.L1;

            var t = ((P.X * P.X + P.Y * P.Y - L0 * L0 - L1 * L1) / (2 * L0 * L1));
            var beta1 = Math.Atan2(Math.Sqrt(1 - t * t), t);
            var beta2 = Math.Atan2(-Math.Sqrt(1 - t * t), t);

            var k1 = (P.X * (L0 + L1 * Math.Cos(beta2)) + P.Y * L1 * Math.Sin(beta2)) / (P.X * P.X + P.Y * P.Y);
            var k2 = (P.X * (L0 + L1 * Math.Cos(beta1)) + P.Y * L1 * Math.Sin(beta1)) / (P.X * P.X + P.Y * P.Y);

            var tempAlpha1 = Math.Atan2(Math.Sqrt(1 - k1 * k1), k1);
            var tempAlpha2 = Math.Atan2(-Math.Sqrt(1 - k1 * k1), k1);

            var tempAlpha3 = Math.Atan2(Math.Sqrt(1 - k2 * k2), k2);
            var tempAlpha4 = Math.Atan2(-Math.Sqrt(1 - k2 * k2), k2);
            var alpha1 = 0.0;
            Robot tempRobot1 = _robot1.Clone();
            tempRobot1.Alpha0 = tempAlpha1;
            tempRobot1.Alpha1 = beta1;
            double epsilon = 0.1;

            var d1 = Length(tempRobot1.Point2, position);
            if (d1 < epsilon)
            {
                alpha1 = tempRobot1.Alpha0;
                _showFirstPermission = true;
                OnPropertyChanged(nameof(ShowFirst));
            }
            else
            {
                tempRobot1.Alpha0 = tempAlpha2;
                d1 = Length(tempRobot1.Point2, position);
                if (d1 < epsilon)
                {
                    alpha1 = tempRobot1.Alpha0;
                    _showFirstPermission = true;
                    OnPropertyChanged(nameof(ShowFirst));
                }
                else
                {
                    MessageBox.Show("Solution 1 not exist");
                    _showFirstPermission = false;
                    OnPropertyChanged(nameof(ShowFirst));
                }
            }

            var alpha2 = 0.0;
            Robot tempRobot2 = _robot1.Clone();
            tempRobot2.Alpha0 = tempAlpha3;
            tempRobot2.Alpha1 = beta2;
            var d2 = Length(tempRobot2.Point2, position);
            if (d2 < epsilon)
            {
                alpha2 = tempRobot2.Alpha0;
                _showSecondPermission = true;
                OnPropertyChanged(nameof(ShowSecond));
            }
            else
            {
                tempRobot2.Alpha0 = tempAlpha4;
                d2 = Length(tempRobot2.Point2, position);
                if (d2 < epsilon)
                {
                    alpha2 = tempRobot2.Alpha0;
                    _showSecondPermission = true;
                    OnPropertyChanged(nameof(ShowSecond));
                }
                else
                {
                    MessageBox.Show("Solution 2 not exist");
                    _showSecondPermission = false;
                    OnPropertyChanged(nameof(ShowSecond));
                }
            }

            var output = new double[4] { alpha1, beta1, alpha2, beta2 };
            for (int i = 0; i < 4; i++)
            {
                if (output[i] < 0) output[i] += (Math.PI * 2);
            }


            return output;
        }

        double Length(Point p1, Point p2)
        {
            return (new Vector(p2.X, p2.Y) - new Vector(p1.X, p1.Y)).Length;
        }
        //public double[] CalculateArmAnglesForPosition(Point position)
        //{

        //}
        public void CollisionCheck()
        {

            if (_showFirstPermission)
            {
                foreach (var item in ObstaclesCollection)
                {
                    if (item.CollisionCheck(Robot1.Point0, Robot1.Point1) ||
                        item.CollisionCheck(Robot1.Point1, Robot1.Point2))
                    {
                        _showFirstPermission = false;
                        OnPropertyChanged(nameof(ShowFirst));
                        MessageBox.Show("Solution 1 obstacle");
                        break;
                    };
                }


            }

            if (_showSecondPermission)
            {
                foreach (var item in ObstaclesCollection)
                {
                    if (item.CollisionCheck(Robot1.Point0, Robot1.Point1bis) ||
                        item.CollisionCheck(Robot1.Point1bis, Robot1.Point2bis))
                    {
                        _showSecondPermission = false;
                        OnPropertyChanged(nameof(ShowSecond));
                        MessageBox.Show("Solution 2 obstacle");
                        break;
                    }
                }
            }
        }
        public void RefreshRobots()
        {
            ///OnPropertyChanged(nameof(Robot2));
            OnPropertyChanged(nameof(Robot1));
        }
    }
}
