using Gobbler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobbler
{
    public enum NodeKind { Root, Feeds, Favourites, Folder, Feed };

    public interface INode
    {
        string Name { get; set; }
        int Count { get; set; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        NodeKind Kind { get; set; }

        ObservableCollection<INode> Nodes { get; set; } 
        ObservableCollection<Entry> Entries { get; set; }
        void Archive();
        void Refresh();
        void Update();
        void RefreshCount();
        void Pulse();
    }


    public abstract class Node : INode, INotifyPropertyChanged
    {
        public Node(string name, NodeKind kind)
        {
            this.Name = name;
            this.Kind = kind;
            Nodes = new ObservableCollection<INode>();
            Entries = new ObservableCollection<Entry>();

            Entries.CollectionChanged += Entries_CollectionChanged;
        }

        private int _count;
        private bool _isExpanded;
        private bool _isSelected;

        public string Name { get; set; }
        public int Count
        {
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
            get
            {
                return _count;
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public NodeKind Kind { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<INode> Nodes { get; set; } 
        public ObservableCollection<Entry> Entries { get; set; }

        private void Entries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Count");
        }

        public abstract void Archive();
        public abstract void Update();
        public abstract void Refresh();
        public abstract void RefreshCount();
        public abstract void Pulse();
        
    }



}
