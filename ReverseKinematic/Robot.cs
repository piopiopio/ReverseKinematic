using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace ReverseKinematic
{
    public class Robot : ViewModelBase
    {
        public Robot()
        {

        }
        public Robot(double l0, double l1, double alpha0, double alpha1)
        {
            _l0 = l0;
            _l1 = l1;
            _alpha0 = alpha0;
            _alpha1 = alpha1;
        }

        public Robot Clone()
        {
            return new Robot(_l0, _l1,_alpha0, _alpha1);
        }
        private void Refresh()
        {
            OnPropertyChanged(nameof(L0));
            OnPropertyChanged(nameof(L1));
            OnPropertyChanged(nameof(Point1));
            OnPropertyChanged(nameof(Point2));
            OnPropertyChanged(nameof(Point1bis));
            OnPropertyChanged(nameof(Point2bis));
        }
        private double _l0 = 0;
        public double L0
        {
            get { return _l0; }
            set
            {
                _l0 = value;
                Refresh();
            }
        }

        private double _l1 = 0;
        public double L1
        {
            get { return _l1; }
            set
            {
                _l1 = value;
                Refresh();
            }
        }

        private double _alpha0 = 0;
        public double Alpha0
        {
            get { return _alpha0; }
            set
            {
                _alpha0 = value;
                Refresh();
            }
        }

        private double _alpha1 = 0;

        public double Alpha1
        {
            get { return _alpha1; }
            set
            {
                _alpha1 = value;
                Refresh();
            }
        }


        private double _alpha0bis = 0;
        public double Alpha0bis
        {
            get { return _alpha0bis; }
            set
            {
                _alpha0bis = value;
                Refresh();
            }
        }

        private double _alpha1bis = 0;

        public double Alpha1bis
        {
            get { return _alpha1bis; }
            set
            {
                _alpha1bis = value;
                Refresh();
            }
        }

        private Point CenterPoint = new Point(500, 500);

        public Point Point0
        {
            get
            {
                return (CenterPoint);
            }
        }

        public Point Point1
        {
            get
            {
                //return CenterPoint + new Vector(L0 * Math.Cos(_alpha0), L0 * Math.Sin(_alpha0));
                var joint = new Point(CenterPoint.X + (L0 * Math.Cos(Alpha0)), CenterPoint.Y + (L0 * Math.Sin(Alpha0)));
              

                return joint;
            }
        }

        public Point Point2
        {
            get
            {
                // return (CenterPoint + new Vector(L0 * Math.Cos(_alpha0), L0 * Math.Sin(_alpha0)) + new Vector(L1 * Math.Cos(_alpha0 + _alpha1), L1 * Math.Sin(_alpha0 + _alpha1)));
                var joint = new Point(CenterPoint.X + (L0 * Math.Cos(Alpha0)), CenterPoint.Y + (L0 * Math.Sin(Alpha0)));
                var end = new Point(joint.X + (L1 * (((Math.Cos(Alpha1) * Math.Cos(Alpha0))) + (Math.Sin(Alpha1) * Math.Sin(Alpha0)))),
                    joint.Y + (L1 * (-(Math.Sin(Alpha1) * Math.Cos(Alpha0)) + (Math.Cos(Alpha1) * Math.Sin(Alpha0)))));
                return end;
            }

        }

        public Point Point1bis
        {
            get
            {
                //return CenterPoint + new Vector(L0 * Math.Cos(_alpha0), L0 * Math.Sin(_alpha0));
                var joint = new Point(CenterPoint.X + (L0 * Math.Cos(Alpha0bis)), CenterPoint.Y + (L0 * Math.Sin(Alpha0bis)));


                return joint;
            }
        }

        public Point Point2bis
        {
            get
            {
                // return (CenterPoint + new Vector(L0 * Math.Cos(_alpha0), L0 * Math.Sin(_alpha0)) + new Vector(L1 * Math.Cos(_alpha0 + _alpha1), L1 * Math.Sin(_alpha0 + _alpha1)));
                var joint = new Point(CenterPoint.X + (L0 * Math.Cos(Alpha0bis)), CenterPoint.Y + (L0 * Math.Sin(Alpha0bis)));
                var end = new Point(joint.X + (L1 * (((Math.Cos(Alpha1bis) * Math.Cos(Alpha0bis))) + (Math.Sin(Alpha1bis) * Math.Sin(Alpha0bis)))),
                    joint.Y + (L1 * (-(Math.Sin(Alpha1bis) * Math.Cos(Alpha0bis)) + (Math.Cos(Alpha1bis) * Math.Sin(Alpha0bis)))));
                return end;
            }

        }

    }
}