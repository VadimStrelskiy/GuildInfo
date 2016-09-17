using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GuildInfo.Wpf.Selection
{
    public class SelectionList<T> : List<SelectionItem<T>>, INotifyPropertyChanged
    {
        private int _selectionCount;

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as SelectionItem<T>;
            if ((item != null) && e.PropertyName == "IsSelected")
            {
                if (item.IsSelected)
                    SelectionCount = SelectionCount + 1;
                else
                    SelectionCount = SelectionCount - 1;
            }
        }

        public SelectionList()
        { }

        public SelectionList(IEnumerable<T> elements)
        {
            foreach (T element in elements)
                AddItem(element);
        }

        public void AddItem(T element)
        {
            var item = new SelectionItem<T>(element);
            item.PropertyChanged += item_PropertyChanged;

            Add(item);
        }

        public IEnumerable<T> GetSelection()
        {
            return this.Where(e => e.IsSelected).Select(e => e.Element);
        }

        public IEnumerable<U> GetSelection<U>(Func<SelectionItem<T>, U> expression)
        {
            return this.Where(e => e.IsSelected).Select(expression);
        }

        public int SelectionCount
        {
            get { return _selectionCount; }

            private set
            {
                _selectionCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectionCount"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
