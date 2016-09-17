namespace GuildInfo.Wpf.Selection
{
    public class SelectionItem<T> : ViewModels.ViewModelBase
    {
        private bool _isSelected;

        public SelectionItem(T element, bool isSelected)
        {
            Element = element;
            IsSelected = isSelected;
        }

        public SelectionItem(T element) : this(element, false)
        {
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public T Element { get; set; }
    }
}
