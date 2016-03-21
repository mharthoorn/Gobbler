using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Gobbler
{

    public class Feed 
    {
        public string Name { get; set; }
        public Uri Uri { get; set; }
        public Image Image { get; set; }
        public DateTime Updated { get; set; }
        private List<Entry> items = new List<Entry>();

        public IEnumerable<Entry> Entries
        {
            get
            {
                return items;
            }
        }

        public Feed() { }
        public Feed(Uri uri)
        {
            this.Uri = uri;
        }

        public Feed(string uri)
        {
            this.Uri = new Uri(uri);
        }
        
        public void Merge(Feed feed)
        {
            foreach(Entry item in feed.Entries)
            {
                if (!Entries.Contains(item))
                    items.Add(item);
            }
        }

        public void Refill(Feed feed)
        {
            this.items.Clear();
            this.items.AddRange(feed.Entries);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Items"));
        }

        public void Load(IEnumerable<Entry> entries)
        {
            this.items.Clear();
            this.items.AddRange(entries);
        }

        public void Update()
        {
            Feed feed = FeedReader.Read(this.Uri);
            Merge(feed);
            Updated = DateTime.UtcNow;
        }

        public void Clear()
        {
            items.Clear();
            Updated = DateTime.MinValue;
        }

        public void Archive()
        {
            foreach(Entry entry in Entries)
            {
                if (entry.Status == EntryStatus.Read)
                    entry.Status = EntryStatus.Archived;
            }
        }

        public IEnumerable<Entry> BackLog
        {
            get
            {
                return Entries.Where(e => e.Status == EntryStatus.Unread);
            }
        }
        public int Count()
        {
            return BackLog.Count();
        }

        public bool Exists(Entry item)
        {
            return items.Exists(i => i.Equals(item));
        }

        public void Add(Entry item)
        {
            if (!Exists(item))
                items.Add(item);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

  

}
