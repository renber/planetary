using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.ViewModels.Navigation
{
    class NavigationElement : ViewModelBase
    {
        public String IconName { get; }

        public string Title { get; }

        public object DataContext { get; }

        bool selected;
        public bool IsSelected
        {
            get => selected;
            set => ChangeProperty(ref selected, value);
        }

        bool expanded;
        public bool IsExpanded
        {
            get => expanded;
            set
            {
                if (ChangeProperty(ref expanded, value))
                {
                    OnPropertyChanged(nameof(IconName));
                }
            }
        }

        public ObservableCollection<NavigationElement> Items { get; private set; } = new ObservableCollection<NavigationElement>();

        public bool HasChildren => Items.Count > 0;

        public NavigationElement(string title, string iconName, object dataContext, params NavigationElement[] items)
        {
            Title = title ?? "";
            DataContext = dataContext;
            IconName = iconName;

            foreach (var item in items)
                Items.Add(item);
        }

        public void ReplaceItemsCollection(ObservableCollection<NavigationElement> items)
        {
            Items = items;
            OnPropertyChanged(nameof(items));
        }
    }
}
