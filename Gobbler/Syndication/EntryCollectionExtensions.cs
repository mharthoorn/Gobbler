using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobbler
{
    public static class EntryCollectionExtensions
    {
        public static IEnumerable<Entry> Backlog(this Feed feed)
        {
            return feed.Entries.Where(entry => entry.IsBacklog).OrderBy(f => f.Published.When);
        }

        public static IEnumerable<Entry> BackLog(this IEnumerable<Entry> entries)
        {
            return entries.Where(item => item.Status == EntryStatus.Unread).OrderBy(entry => entry.Published.When);
        }

        public static void Fill<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            collection.Clear();
            foreach (T item in items)
            {
                collection.Add(item);
            }
        }

        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> enumerable)
        {
            ObservableCollection<T> collection = new ObservableCollection<T>();
            collection.Fill(enumerable);
            return collection;
        }

        public static int Count(this Feed feed)
        {
            return feed.Backlog().Count();
        }

        public static void Merge<T>(this ObservableCollection<T> collection, IEnumerable<T> input)
        {
            foreach (T item in input)
            {
                if (!collection.Contains(item))
                    collection.Add(item);
            }
        }

        public static void Update(this ObservableCollection<Entry> collection, IEnumerable<Entry> input)
        {
            List<Entry> reference = input.ToList();
            collection.RemoveWhen(e => !reference.Contains(e));
            collection.Merge(reference);
        }

        public static void RemoveWhen<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
        {
            List<T> removals = new List<T>();

            foreach (T item in collection)
            {
                if (predicate(item))
                    removals.Add(item);
            }
            foreach (T item in removals)
            {
                collection.Remove(item);
            }
        }

        public static IEnumerable<INode> All(this INode node)
        {
            foreach(INode n in node.Nodes)
            {
                    yield return n;

                foreach (INode m in n.All())
                {
                        yield return m;
                }
            }

        }
    }
}
