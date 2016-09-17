using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GuildInfo.Wpf.Controls
{
    public partial class CharacterControl : UserControl
    {
        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public CharacterControl()
        {
            InitializeComponent();
        }

        private void ListView_HeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked == null) return;
            if (headerClicked.Role == GridViewColumnHeaderRole.Padding) return;

            string header = headerClicked.Column.Header as string;
            if (header == null)
            {
                headerClicked = ClassColumn;
                header = "Class";
            }

            ListSortDirection direction;
            if (headerClicked != _lastHeaderClicked)
            {
                direction = ListSortDirection.Ascending;
            }
            else
            {
                direction = _lastDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            
            _lastHeaderClicked = headerClicked;
            _lastDirection = direction;

            Binding b = headerClicked.Column.DisplayMemberBinding as Binding;
            if (b != null)
            {
                header = b.Path.Path;
            }

            ICollectionView resultDataView = CollectionViewSource.GetDefaultView(ListView.ItemsSource);
            resultDataView.SortDescriptions.Clear();
            resultDataView.SortDescriptions.Add(new SortDescription(header, direction));
            resultDataView.Refresh();
        }
    }
}