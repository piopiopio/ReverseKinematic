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
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace ReverseKinematic
{
    public class Scene : ViewModelBase
    {
        private Robot _robot1, _robot2;
        private bool _alternativeSolution = false;
        public bool AlternativeSolution {
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
        public Robot Robot {
            get
            {
                if (AlternativeSolution)
                {
                    return _robot1;
                }
                else
                {
                    return _robot2;
                }
            }
            set
            {

            }
        }
        public ObservableCollection<RectangleObstacle> ObstaclesCollection { get; private set; }
        private Point _target;

        public Point Target
        {
            get { return _target; }
            set
            {
                _target = value;
                OnPropertyChanged();
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
            _robot1.L1 = 300;
            _robot1.Alpha0 = Math.PI / 6;
            _robot1.Alpha1 = Math.PI / 6;

            _robot2 = _robot1.Clone();

            ObstaclesCollection = new ObservableCollection<RectangleObstacle>();
            bitmapHelper.SetColor(240, 248, 255);
            _target = new Point(100, 100);
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
        BitmapHelper bitmapHelper = new BitmapHelper(360, 360);
        void GetObstaclesInConfigurationSpace()
        {
            // _configurationSpace.Clear();
            Robot tempRobot = Robot.Clone();
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

            arrayFloodFill(ConfigurationSpaceArray, 180, 180);

            for (int i = 0; i < 360; i++)
            {
                for (int j = 0; j < 360; j++)
                {
                    if (ConfigurationSpaceArray[i, j] != -1)
                    {
                        bitmapHelper.SetPixel(i, j, 0, (byte)(ConfigurationSpaceArray[i, j] * 255 / 510), 0);
                    }
                    else
                    {
                        bitmapHelper.SetPixel(i, j, 255, 0, 0);
                    }
                }
            }

            OnPropertyChanged(nameof(ConfigurationSpaceBitmap));
        }

        public void GeneratePath()
        {
            GetObstaclesInConfigurationSpace();
            var tempAngles=CalculateArmAnglesForPosition(_target);
            _robot1.Alpha0 = tempAngles[0];
            _robot1.Alpha1 = tempAngles[1];
            _robot2.Alpha0 = tempAngles[2];
            _robot2.Alpha1 = tempAngles[3];
            OnPropertyChanged(nameof(Robot));
        }

        void arrayFloodFill(int[,] ConfigurationSpaceArray, int positionX, int positionY, int colorToChange = 0)
        {
            if (ConfigurationSpaceArray[positionX, positionY] != colorToChange)
            {
                return;
            }

           // int i = 0;
            var toFill = new List<int[]>();

            toFill.Add(new int[3] { positionX, positionY,0 });

            while (toFill.Any())
            {
                var p = toFill[0];
                toFill.RemoveAt(0);
                //ConfigurationSpaceArray[p[0], p[1]] = i;
                //ConfigurationSpaceArray[p[0], p[1]] = (int)Math.Sqrt((p[0]-positionX)* (p[0] - positionX) + (p[1] - positionY)* (p[1] - positionY));
                ConfigurationSpaceArray[p[0], p[1]] = p[2];

                //if (getPixelFromLimitedBitmab(Bitmap2, p[0] + 1, p[1], colorToChange) && !toFill.Any(t => (t[0] == (p[0] + 1)) && (t[1] == p[1])))

                if ((p[0] + 1) < ConfigurationSpaceArray.GetLength(0))
                {
                    if ((ConfigurationSpaceArray[p[0] + 1, p[1]] == colorToChange) &&
                        !toFill.Any(t => (t[0] == (p[0] + 1)) && (t[1] == p[1])))
                    {
                        toFill.Add(new int[3] { p[0] + 1, p[1], p[2]+1 });
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
          
            var destination = position - new Point(500,500);
            var x = destination.X;
            var y = destination.Y;
            var beta = -Math.Acos((x * x + y * y - Robot.L0 * Robot.L0 - Robot.L1 * Robot.L1) / (2 * Robot.L0 * Robot.L1));
            var alpha = Math.Asin((Robot.L1 * Math.Sin(beta)) / Math.Sqrt(x * x + y * y)) + Math.Atan2(y, x);
            var beta2 = -beta;
            var alpha2 = -Math.Asin((Robot.L1 * Math.Sin(-beta2)) / Math.Sqrt(x * x + y * y)) + Math.Atan2(y, x);

            return new double[4] { alpha, beta, alpha2, beta2 };
        }
    }
}
