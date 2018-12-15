using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReverseKinematic
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName=null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public event PropertyChangedEventHandler TurnOnAnimationModeReverseKinematic;

        protected void TurnOnAnimation()
        {
            //BusyEllipseLed = 1;
            if (TurnOnAnimationModeReverseKinematic != null)
                TurnOnAnimationModeReverseKinematic(this, new PropertyChangedEventArgs("TurnOnControlsReverseKinematic"));
            //BusyEllipseLed = 0;
        }

        public event PropertyChangedEventHandler TurnOffAnimationModeReverseKinematic;

        protected void TurnOffAnimation()
        {
            //BusyEllipseLed = 1;
            if (TurnOffAnimationModeReverseKinematic != null)
                TurnOffAnimationModeReverseKinematic(this, new PropertyChangedEventArgs("TurnOffControlsReverseKinematic"));
            //BusyEllipseLed = 0;
        }
    }
}

