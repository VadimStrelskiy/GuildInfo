namespace GuildInfo.Wpf.ViewModels
{
    public class ClassViewModel : ViewModelBase
    {
        public ClassViewModel(int id, string icon)
        {
            Id = id;
            Icon = icon;
        }

        public int Id { get; private set; }
        public string Icon { get; private set; }
    }
}
