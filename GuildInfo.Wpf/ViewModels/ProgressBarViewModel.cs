namespace GuildInfo.Wpf.ViewModels
{
    public class ProgressBarViewModel : ViewModelBase
    {
        private double _progress;

        public double Progress
        {
            get { return _progress;}
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }
    }
}
