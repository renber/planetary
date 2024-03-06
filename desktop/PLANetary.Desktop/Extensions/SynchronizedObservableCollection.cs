using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Extensions
{
    /// <summary>
    /// Wraps a list of TIn objects and exposes them as TOut objects
    /// Both lists are synchronized
    /// </summary>    
    class SynchronizedObservableCollection<TIn, TOut> : ObservableCollection<TOut>
    {
        private ObservableCollection<TIn> SourceCollection { get; }

        private Func<TIn, TOut> ItemGenerator { get; }

        private Func<TOut, TIn> SourceItemExtractor { get; }

        public SynchronizedObservableCollection(ObservableCollection<TIn> sourceCollection, Func<TIn, TOut> itemGenerator, Func<TOut, TIn> sourceItemExtractor)
        {
            SourceCollection = sourceCollection ?? throw new ArgumentNullException(nameof(sourceCollection));
            ItemGenerator = itemGenerator ?? throw new ArgumentNullException(nameof(itemGenerator));
            SourceItemExtractor = sourceItemExtractor ?? throw new ArgumentNullException(nameof(sourceItemExtractor));
            
            SourceCollection.CollectionChanged += SourceCollection_CollectionChanged;

            BuildFromSource();
        }

        private void BuildFromSource()
        {
            Clear();
            foreach (var item in SourceCollection)
            {
                Add(ItemGenerator.Invoke(item));
            }
        }

        /// <summary>
        /// Synchronizes changes from the SourceCollection to this Collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    for(int i = e.NewStartingIndex; i < e.NewStartingIndex + e.NewItems.Count; i++)
                    {
                        if (i >= Count)
                            Add(ItemGenerator.Invoke((TIn)e.NewItems[i - e.NewStartingIndex]));
                        else
                            Insert(i, ItemGenerator.Invoke((TIn)e.NewItems[i - e.NewStartingIndex]));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        Move(e.OldStartingIndex + i, e.NewStartingIndex + i);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    for (int i = e.OldStartingIndex + e.OldItems.Count - 1; i >= e.OldStartingIndex; i++)
                    {
                        RemoveAt(i);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    for (int i = e.OldStartingIndex; i < e.OldStartingIndex + e.OldItems.Count; i++)
                    {
                        SetItem(i, ItemGenerator.Invoke((TIn)e.NewItems[i - e.OldStartingIndex]));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    BuildFromSource();
                    break;
            }
        }
    }
}
