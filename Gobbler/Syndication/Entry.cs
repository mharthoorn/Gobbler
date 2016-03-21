using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobbler
{
    public enum EntryStatus { Unread, Read, Archived, Starred }
    public class Entry : INotifyPropertyChanged
    {
        private EntryStatus status;

        public EntryStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                if (PropertyChanged != null)
                {
                    PropertyChangedEventArgs e = new PropertyChangedEventArgs("Status");
                    PropertyChanged(this, e);
                }
            }
        }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public Publication Published { get; set; }
        public string PubString { get; set; }
        public string Guid { get; set; }

        public bool IsBacklog
        {
            get
            {
                return (status == EntryStatus.Unread) || (status == EntryStatus.Read);
            }
        }
        public string When
        {
            get
            {
                string s = Published.ToString();
                return s;
            }
        }
        public override bool Equals(object obj)
        {
            Entry other;
            if (obj.Cast(out other))
                return other.Guid == this.Guid;
            else
                return false;
        }
        public override int GetHashCode()
        {
            return this.Guid.GetHashCode();
        }
        public Entry Clone()
        {
            Entry entry = new Entry();
            entry.Status = this.Status;
            entry.Title = this.Title;
            entry.Link = this.Link;
            entry.Description = this.Description;
            entry.Published = new Publication(this.Published.Origin);
            entry.Guid = this.Guid;
            return entry;
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
