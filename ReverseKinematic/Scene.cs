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
using System.Windows.Threading;
using System.Xaml;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace ReverseKinematic
{
    public class Scene : ViewModelBase
    {
        private Robot _robot1, _robot2;
        //private bool _alternativeSolution = false;


        //public bool AlternativeSolution
        //{
        //    get
        //    {
        //        return _alternativeSolution;
        //    }
        //    set
        //    {
        //        _alternativeSolution = value;
        //        OnPropertyChanged(nameof(Robot));
        //    }
        //}
        private double _simulationTime = 5;
        public double SimulationTime
        {
            get { return _simulationTime; }
            set
            {
                _simulationTime = value;
                OnPropertyChanged();
            }
        }
            

        private bool _showFirst = true;
        private bool _showFirstPermission = true;

        public bool ShowFirst
        {
            get { return _showFirst&& _showFirstPermission; }
            set
            {
                _showFirst = true;
                // _showSecond = !_showFirst;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowSecond));
                //OnPropertyChanged(nameof(Robot1));
            }
        }

        //private bool _showSecond = true;
        private bool _showSecondPermission = true;

        public bool ShowSecond
        {
            get { return !_showFirst && _showFirstPermission; }
            set
            {
                // _showSecond = value;
                _showFirst = false;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowFirst));
                OnPropertyChanged(nameof(ShowSecondEnd));          
            }
        }

        private bool _showAlternativeEnd = false;

        public bool ShowFirstEnd
        {
            get { return !_showAlternativeEnd && _showSecondPermission; }
            set
            {
                _showAlternativeEnd = false;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowFirst));
                OnPropertyChanged(nameof(ShowSecondEnd));
            }
        }


        public bool ShowSecondEnd
        {
            get { return _showAlternativeEnd && _showSecondPermission; }
            set
            {
                _showAlternativeEnd = true;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowFirstEnd));
                OnPropertyChanged(nameof(ShowSecondEnd));
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


        public Robot Robot2
        {
            get
            {

                return _robot2;

            }
            set
            {
                _robot2 = value;
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
            
                var SolutionsFlags = Robot1.SetNewPositionWorldCoordintaes(_startPosition);

                if (SolutionsFlags[0])
                {
                    _showFirstPermission = true;
                 
                }
                else
                {
                    _showFirstPermission = false;
                   
                }

                if (SolutionsFlags[1])
                {
                    _showFirstPermission = true;
                  
                }
                else
                {
                    _showFirstPermission = false;
                }

                //TODO: Obsługa błędów
                OnPropertyChanged(nameof(ShowFirst));
                OnPropertyChanged(nameof(ShowSecond));
                OnPropertyChanged();
                OnPropertyChanged(nameof(Robot));

                CollisionCheck();

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
                Robot2 = Robot1.Clone();
                var SolutionsFlags = Robot2.SetNewPositionWorldCoordintaes(_endPosition);
                if (SolutionsFlags[0])
                {
                    _showSecondPermission = true;

                }
                else
                {
                    _showSecondPermission = false;

                }

                if (SolutionsFlags[1])
                {
                    _showSecondPermission = true;

                }
                else
                {
                    _showSecondPermission = false;
                }


                OnPropertyChanged(nameof(ShowFirstEnd));
                OnPropertyChanged(nameof(ShowSecondEnd));
                OnPropertyChanged();
                OnPropertyChanged(nameof(Robot));
                CollisionCheck();
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
            _startPosition = new Point(300, 700);
            _endPosition = new Point(800, 500);

            _robot1 = new Robot(200, 200, _startPosition);
            _robot2 = new Robot(200, 200, _endPosition);

            ObstaclesCollection = new ObservableCollection<RectangleObstacle>();
            bitmapHelper.SetColor(240, 248, 255);
            bitmapHelper.SetColor(120, 124, 128);

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

        List<int[]> Path = new List<int[]>();
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


            int a0;
            int a1;


            if (_showFirst)
            {
                a0 = wrapAngles((int)(Robot1.Alpha0 * 180 / Math.PI));
                a1 = wrapAngles((int)(Robot1.Alpha1 * 180 / Math.PI));
            }
            else
            {
                a0 = wrapAngles((int)(Robot1.Alpha0bis * 180 / Math.PI));
                a1 = wrapAngles((int)(Robot1.Alpha1bis * 180 / Math.PI));
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

            //double[] startAngles = Robot1.CalculateArmAnglesForPosition(StartPosition);

            //double[] endAngles = Robot1.CalculateArmAnglesForPosition(EndPosition);




            double[] endAngles;
            if (_showAlternativeEnd)
            {
                endAngles = new double[] { Robot2.Alpha0bis, Robot2.Alpha1bis };
            }
            else
            {
                endAngles = new double[] { Robot2.Alpha0, Robot2.Alpha1 };
            }



            Path.Clear();
            Path.Add(new int[3] { (int)(endAngles[0] * 180 / Math.PI), (int)(endAngles[1] * 180 / Math.PI), ConfigurationSpaceArray[(int)(endAngles[0] * 180 / Math.PI), (int)(endAngles[1] * 180 / Math.PI)] });
            int value = ConfigurationSpaceArray[Path.Last()[0], Path.Last()[1]];

            while (value > 1)
            {




                if (ConfigurationSpaceArray[wrapAngles(Path.Last()[0] + 1), wrapAngles(Path.Last()[1])] == (value - 1))
                {
                    Path.Add(new int[3] { wrapAngles(Path.Last()[0] + 1), wrapAngles(Path.Last()[1]), wrapAngles(ConfigurationSpaceArray[Path.Last()[0] + 1, Path.Last()[1]]) });
                }

                else if (ConfigurationSpaceArray[wrapAngles(Path.Last()[0] - 1), wrapAngles(Path.Last()[1])] == (value - 1))
                {
                    {
                        Path.Add(new int[3] { wrapAngles(Path.Last()[0] - 1), wrapAngles(Path.Last()[1]), wrapAngles(ConfigurationSpaceArray[Path.Last()[0] - 1, Path.Last()[1]]) });
                    }
                }

                else if (ConfigurationSpaceArray[wrapAngles(Path.Last()[0]), wrapAngles(Path.Last()[1] + 1)] == (value - 1))
                {
                    {
                        Path.Add(new int[3] { wrapAngles(Path.Last()[0]), wrapAngles(Path.Last()[1] + 1), wrapAngles(ConfigurationSpaceArray[Path.Last()[0], Path.Last()[1] + 1]) });
                    }
                }
                else if (ConfigurationSpaceArray[wrapAngles(Path.Last()[0]), wrapAngles(Path.Last()[1] - 1)] == (value - 1))
                {
                    {
                        Path.Add(new int[3] { wrapAngles(Path.Last()[0]), wrapAngles(Path.Last()[1] - 1), wrapAngles(ConfigurationSpaceArray[Path.Last()[0], Path.Last()[1] - 1]) });
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
                bitmapHelper.SetPixel(item[0], item[1], 255, 255, 255);
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
                        //TODO: obsługa bledu
                        //_showFirstPermission = false;
                        // OnPropertyChanged(nameof(ShowFirst));
                        MessageBox.Show("Solution 1 obstacle");
                        break;
                    };
                }


            }
            else
            {
                foreach (var item in ObstaclesCollection)
                {
                    if (item.CollisionCheck(Robot1.Point0, Robot1.Point1bis) ||
                        item.CollisionCheck(Robot1.Point1bis, Robot1.Point2bis))
                    {
                        //TODO: obsługa bledu
                        //_showFirstPermission = false;
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
        private DispatcherTimer timer;
        private Robot SavedRobotCopy;
        //private double _animationSpeed = 10;
        private int pathFrameNumber;

        private List<int[]> multiplicateFrames(List<int[]> L)
        {
            var steps = L.Count - 2;
            List < int[] > output=new List<int[]>();

            for (int i = 0; i < steps; i++)
            {
                int[] temp = new int[2];
                temp[0] = (L[i][0] + L[i+1][0]) / 2;
                temp[1] = (L[i][1] + L[i+1][1]) / 2;
                output.Add(L[i]);
                output.Add(temp);
            }
            output.Add(L.Last());
            return output;
        }

        public void StartSimulation()
        {
            const int MinFramesPerSeconds = 10;

            GetObstaclesInConfigurationSpace();
            SavedRobotCopy = Robot1.Clone();
            timer = new DispatcherTimer(DispatcherPriority.Render);


            //PathCopy =new List<int[]>();


            //foreach (var item in Path)
            //{
            //    PathCopy.Add(item);
            //}




            //while ((PathCopy.Count / SimulationTime) < MinFramesPerSeconds)
            //{
            //    PathCopy = multiplicateFrames(PathCopy);
            //}



            pathFrameNumber = Path.Count() - 1;


            timer.Interval = TimeSpan.FromMilliseconds(1000*SimulationTime/ Path.Count);
            timer.Tick += TimerOnTick;
            timer.Start();
        }

        //private List<int[]> PathCopy;
        private List<Point[]> PointsList;
        private void TimerOnTick(object sender, EventArgs e)
        {
            if (pathFrameNumber >= 0)
            {
                //Robot1._alpha0 = PathCopy[pathFrameNumber][0] * Math.PI / 180;
                //Robot1._alpha1 = PathCopy[pathFrameNumber][1] * Math.PI / 180;
                //Robot1._alpha0bis = PathCopy[pathFrameNumber][0] * Math.PI / 180;
                //Robot1._alpha1bis = PathCopy[pathFrameNumber][1] * Math.PI / 180;
                //Robot1.RefreshFast();
                //pathFrameNumber--;


                Robot1.Alpha0 = Path[pathFrameNumber][0] * Math.PI / 180;
                Robot1.Alpha1 = Path[pathFrameNumber][1] * Math.PI / 180;
                Robot1.Alpha0bis = Path[pathFrameNumber][0] * Math.PI / 180;
                Robot1.Alpha1bis = Path[pathFrameNumber][1] * Math.PI / 180;

                pathFrameNumber--;
            }
            else
            {
                timer.Stop();
                Robot1 = SavedRobotCopy.Clone();
            }
        }
    }
}
