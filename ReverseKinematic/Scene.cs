using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReverseKinematic
{
    public class Scene : ViewModelBase
    {
        public Robot Robot { get; private set; }
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
            Robot = new Robot();
            ObstaclesCollection = new ObservableCollection<RectangleObstacle>();
            Robot.L0 = 200;
            Robot.L1 = 300;
            Robot.Alpha0 = Math.PI / 6;
            Robot.Alpha1 = Math.PI / 6;
        }

        int[,] ConfigurationSpaceArray = new int[360, 360];
        ObservableCollection<RectangleObstacle> _configurationSpace=new ObservableCollection<RectangleObstacle>();

        public ObservableCollection<RectangleObstacle> ConfigurationSpace
        {//TODO: Przerobić na pisanie po bitmapie
            get { return _configurationSpace; }
            set
            {
                _configurationSpace = value;
                OnPropertyChanged();
            }
        }

        void GetObstaclesInConfigurationSpace()
        {
            _configurationSpace.Clear();
            Robot tempRobot = Robot.Clone();
            for (int i = 0; i < 360; i++)
            {
                for (int j = 0; j < 360; j++)
                {
                    foreach (var item in ObstaclesCollection)
                    {
                        tempRobot.Alpha0 = Math.PI * i / 180;
                        tempRobot.Alpha1 = Math.PI * j / 180;
                        if (item.CollisionCheck(tempRobot.Point0, tempRobot.Point1) || item.CollisionCheck(tempRobot.Point1, tempRobot.Point2))
                        {
                            ConfigurationSpaceArray[i, j] = -1;
                            _configurationSpace.Add(new RectangleObstacle(i,j,1,1,1,0));
                        }
                    }
                }
            }
        }

        public void GeneratePath()
        {
            GetObstaclesInConfigurationSpace();
        }
    }
}
