using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ReverseKinematic
{
    public class RectangleObstacle : ViewModelBase
    {
        public RectangleObstacle(double fromLeft = 0, double fromTop = 0, double width = 0, double height = 0)
        {
            From = new Point(fromLeft, fromTop);
            Size = new Point(width, height);
            Color= new SolidColorBrush(System.Windows.Media.Colors.Indigo);
        }

        public void MoveObstacle(Point delta)
        {
            From=new Point(_from.X+delta.X, _from.Y+delta.Y);
        }
        private bool _selected = false;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;

            }
        }
        public void Select(Point p)
        {
            if (p.X > _from.X && p.X < (_from.X + _size.X) && p.Y > _from.Y && p.Y < (_from.Y + _size.Y))
            {
                _selected = !_selected;
                if (_selected)
                {
                    Color = new SolidColorBrush(System.Windows.Media.Colors.Brown);
                }
                else
                {
                    Color = new SolidColorBrush(System.Windows.Media.Colors.Indigo);
                }
            }
        }
        public RectangleObstacle(double fromLeft, double fromTop, double width, double height, float redRedSaturation, float greenColorSaturation)
        {
            From = new Point(fromLeft, fromTop);
            Size = new Point(width, height);
            _redSaturation = redRedSaturation;
            _greenSaturation = greenColorSaturation;
            _color = new SolidColorBrush(System.Windows.Media.Color.FromScRgb(1, _greenSaturation, _redSaturation, 0));
        }

        private float _redSaturation = 0;
        private float _greenSaturation = 0;
        private SolidColorBrush _color;

        public SolidColorBrush Color
        {
            get { return _color; }
            set
            {
                _color = value; 
                OnPropertyChanged();
            }
        }

        private Point _from;
        public Point From
        {
            get => _from;
            set
            {
                _from = value;
                OnPropertyChanged();
            }
        }

        private Point _size;
        public Point Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged();
            }
        } //Width, height

        public RectangleObstacle Clone()
        {
            return new RectangleObstacle(From.X, From.Y, Size.X, Size.Y);
        }

        public bool CollisionCheck(Point p0, Point p1)
        {
            return CollisionCheck(p0.X, p0.Y, p1.X, p1.Y);
        }

        public bool CollisionCheck(double x1, double y1, double x2, double y2)
        {


            //bool left = lineLine(x1, y1, x2, y2, _from.X, _from.Y, _from.X, _from.Y + _size.Y);
            //bool right = lineLine(x1, y1, x2, y2, _from.X + _size.X, _from.Y, _from.X + _size.X, _from.Y + _size.Y);
            //bool top = lineLine(x1, y1, x2, y2, _from.X, _from.Y, _from.X + _size.X, _from.Y);
            //bool bottom = lineLine(x1, y1, x2, y2, _from.X, _from.Y + _size.Y, _from.X + _size.X, _from.Y + _size.Y);

            if (pointRectangle(x1, y1, _from.X, _from.Y, _from.X + _size.X, _from.Y + _size.Y))
            {
                return true;
            }

            if (pointRectangle(x2, y2, _from.X, _from.Y, _from.X + _size.X, _from.Y + _size.Y))
            {
                return true;
            }
            if (lineLine(x1, y1, x2, y2, _from.X, _from.Y, _from.X, _from.Y + _size.Y)) return true;
            if (lineLine(x1, y1, x2, y2, _from.X + _size.X, _from.Y, _from.X + _size.X, _from.Y + _size.Y)) return true; ;
            if (lineLine(x1, y1, x2, y2, _from.X, _from.Y, _from.X + _size.X, _from.Y)) return true; ;
            if (lineLine(x1, y1, x2, y2, _from.X, _from.Y + _size.Y, _from.X + _size.X, _from.Y + _size.Y)) return true; ;



            return false;
        }


        bool lineLine(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {


            double uA = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
            double uB = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                return true;
            }
            return false;
        }

        bool pointRectangle(double Px, double Py, double x1, double y1, double x2, double y2)
        {
            //(x1,y1) up, left corner and (x2,y2) down, right corner .
            if ( (Px >= x1) && (Px <= x2) && (Py >=y1) && (Py<=y2) )
            {
                return true;
            }

            return false;
        }
    }
}
