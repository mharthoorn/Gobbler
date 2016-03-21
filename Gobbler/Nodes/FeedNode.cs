using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobbler
{
    public class FeedNode : Node, INotifyPropertyChanged
    {
        public FeedNode(string name, Feed feed) : base(name, NodeKind.Feed)
        {
            this.Feed = feed;
        }

        public FeedNode(Feed feed) : base(feed.Name, NodeKind.Feed)
        {
            this.Feed = feed;
        }
        
        private Feed feed;

        public Feed Feed
        {
            get { return feed; }
            set
            {
                feed = value;
                this.Name = feed.Name;
                this.Entries.Fill(feed.Backlog());
            }
        }

        public TimeSpan Interval { get; set; } = new TimeSpan(1, 0, 0);

        public override void Update()
        {
            Feed.Update();
        }


        public void Clear()
        {
            this.Entries.Clear();
            
            feed.Clear();
        }

        public override void Archive()
        {
            Feed.Archive();
        }

        public bool NeedsUpdate
        {
            get
            {
                DateTime next = this.feed.Updated + this.Interval;
                return next < DateTime.UtcNow;
            }
        }

        public void RefreshEntries()
        {
            Entries.RemoveWhen(e => e.Status == EntryStatus.Archived);
            Entries.Merge(Feed.Backlog());
        }

        public int CountUnread()
        {
            return Entries.Where(e => e.Status == EntryStatus.Unread).Count();
        }

        public override void Refresh()
        {
            RefreshEntries();
            RefreshCount();
        }

        public override void RefreshCount()
        {
            this.Count = CountUnread();
        }

        public override void Pulse()
        {
            if (NeedsUpdate)
                this.Update();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Count);
        }
    }
}
