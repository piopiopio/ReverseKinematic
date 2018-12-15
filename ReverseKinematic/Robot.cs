using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Media3D;

namespace ReverseKinematic
{
    public class Robot : ViewModelBase
    {
        public Robot()
        {

        }

        //private bool _visibility = true;
        //public bool Visibility
        //{
        //    get { return _visibility; }
        //    set
        //    {
        //        _visibility = value;
        //        OnPropertyChanged();
        //    }
        //}
        public Robot(double l0, double l1, double alpha0, double alpha1, double alpha0bis, double alpha1bis)
        {
            _l0 = l0;
            _l1 = l1;
            _alpha0 = alpha0;
            _alpha1 = alpha1;
            _alpha0bis=alpha0bis;
            _alpha1bis = alpha1bis;
        }

        public Robot(double l0, double l1, Point p)
        {
            _l0 = l0;
            _l1 = l1;
            SetNewPositionWorldCoordintaes(p);
        }
        public Robot Clone()
        {
            return new Robot(_l0, _l1,_alpha0, _alpha1, _alpha0bis, _alpha1bis);
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(L0));
            OnPropertyChanged(nameof(L1));
            OnPropertyChanged(nameof(Point1));
            OnPropertyChanged(nameof(Point2));
            OnPropertyChanged(nameof(Point1bis));
            OnPropertyChanged(nameof(Point2bis));
            OnPropertyChanged(nameof(ExternalBoundaryRadius));
            OnPropertyChanged(nameof(InternalBoundaryRadius));
        }

        public void RefreshFast()
        {
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

        public double _alpha0 = 0;
        public double Alpha0
        {
            get { return _alpha0; }
            set
            {
                _alpha0 = value;
                Refresh();
            }
        }

        public double _alpha1 = 0;

        public double Alpha1
        {
            get { return _alpha1; }
            set
            {
                _alpha1 = value;
                Refresh();
            }
        }


        public double _alpha0bis = 0;
        public double Alpha0bis
        {
            get { return _alpha0bis; }
            set
            {
                _alpha0bis = value;
                Refresh();
            }
        }

        public double _alpha1bis = 0;

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


        public Vector3D ExternalBoundaryRadius
        {
            get
            {
                double r = (L0 + L1);
                return new Vector3D(500-r,500-r,2*r);
            }

        }

        public Vector3D InternalBoundaryRadius
        {
            get
            {
                double r = Math.Abs(L0 - L1);
                return new Vector3D(500 - r, 500 - r, 2 * r);
            }

        }

        double Length(Point p1, Point p2)
        {
            return (new Vector(p2.X, p2.Y) - new Vector(p1.X, p1.Y)).Length;
        }

        public bool[] SetNewPositionWorldCoordintaes(Point positionXY)
        {
            var JointCoordinates = CalculateArmAnglesForPosition(positionXY);
            bool temp1 = false;
            bool temp2 = false;
            if (CheckIfDoubleIsNumber(JointCoordinates[0]) && CheckIfDoubleIsNumber(JointCoordinates[1]))
            {
                Alpha0 = JointCoordinates[0];
                Alpha1 = JointCoordinates[1];
                Alpha0bis = JointCoordinates[2];
                Alpha1bis = JointCoordinates[3];
                temp1 = true;
            }
            if (CheckIfDoubleIsNumber(JointCoordinates[2]) && CheckIfDoubleIsNumber(JointCoordinates[3]))
            {
                Alpha0 = JointCoordinates[2];
                Alpha1 = JointCoordinates[3];
                Alpha0bis = JointCoordinates[0];
                Alpha1bis = JointCoordinates[1];
                temp2 = true;
            }
            else
            {
                MessageBox.Show("No solution found");
                Alpha0 = JointCoordinates[0];
                Alpha1 = JointCoordinates[1];
                Alpha0bis = JointCoordinates[2];
                Alpha1bis = JointCoordinates[3];
            }

            //return new double[]{Alpha0, Alpha1, Alpha0bis, Alpha1bis};
            return new bool[]{temp1, temp2};
    }

        public static bool CheckIfDoubleIsNumber(double x)
        {
            return (!double.IsNaN(x) && !double.IsInfinity(x));
        }
        public double[] CalculateArmAnglesForPosition(Point position)
        {


            var P = position - new Point(500, 500);
            //var L0 = Robot1.L0;
            //var L1 = Robot1.L1;

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
            Robot tempRobot1 = this.Clone();
            tempRobot1.Alpha0 = tempAlpha1;
            tempRobot1.Alpha1 = beta1;
            double epsilon = 0.1;

            var d1 = Length(tempRobot1.Point2, position);
            if (d1 < epsilon)
            {
                alpha1 = tempRobot1.Alpha0;
                //_showFirstPermission = true;
                //OnPropertyChanged(nameof(ShowFirst));
            }
            else
            {
                tempRobot1.Alpha0 = tempAlpha2;
                d1 = Length(tempRobot1.Point2, position);
                if (d1 < epsilon)
                {
                    alpha1 = tempRobot1.Alpha0;
                  //  _showFirstPermission = true;
                   // OnPropertyChanged(nameof(ShowFirst));
                }
                else
                {
                    //TODO: Obsluga błędów
                    alpha1 = double.NaN;
                    MessageBox.Show("Solution 1 not exist");
                   // _showFirstPermission = false;
                  //  OnPropertyChanged(nameof(ShowFirst));
                }
            }

            var alpha2 = 0.0;
            Robot tempRobot2 = this.Clone();
            tempRobot2.Alpha0 = tempAlpha3;
            tempRobot2.Alpha1 = beta2;
            var d2 = Length(tempRobot2.Point2, position);
            if (d2 < epsilon)
            {
                alpha2 = tempRobot2.Alpha0;
              //  _showSecondPermission = true;
               // OnPropertyChanged(nameof(ShowSecond));
            }
            else
            {
                tempRobot2.Alpha0 = tempAlpha4;
                d2 = Length(tempRobot2.Point2, position);
                if (d2 < epsilon)
                {
                    alpha2 = tempRobot2.Alpha0;
                    //_showSecondPermission = true;
                    //OnPropertyChanged(nameof(ShowSecond));
                }
                else
                {
                    MessageBox.Show("Solution 2 not exist");
                    alpha2 = double.NaN;
                    //_showSecondPermission = false;
                    //OnPropertyChanged(nameof(ShowSecond));
                }
            }

            var output = new double[4] { alpha1, beta1, alpha2, beta2 };
            for (int i = 0; i < 4; i++)
            {
                if (output[i] < 0) output[i] += (Math.PI * 2);
            }


            return output;
        }
    }
}