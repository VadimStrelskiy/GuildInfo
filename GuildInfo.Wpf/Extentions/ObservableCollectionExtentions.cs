using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GuildInfo.Wpf.Extentions
{
    public static class ObservableCollectionExtentions
    {
        public static void Add<T>(this ObservableCollection<T> self, IEnumerable<T> values)
        {
            foreach (T value in values)
            {
                self.Add(value);
            }
        }
    }
}
