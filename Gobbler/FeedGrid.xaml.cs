using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Gobbler
{
    public partial class EntriesGrid : UserControl
    {
        public EntriesGrid()
        {
            InitializeComponent();
            grid.MouseDown += Mygrid_MouseDown;
            grid.PreviewKeyDown += Grid_PreviewKeyDown;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private IEnumerable<Entry> items;

        public IEnumerable<Entry> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                SetItems(value);
            }
        }

        private void SetItems(IEnumerable<Entry> items)
        {
            grid.ItemsSource = null;
            grid.ItemsSource = items;
           
        }
            
        private void _markSingleRead()
        {
            Entry entry;
            if (grid.SelectedItem.Cast(out entry))
            {
                entry.Status = EntryStatus.Read;
                OnPropertyChanged("Status");
            }
        }

        private void _markSelectedRead()
        {
            Entry item = null;
            int count = grid.SelectedItems.Count;

            foreach (object x in grid.SelectedItems)
            {
                if (x.Cast(out item))
                {
                    item.Status = EntryStatus.Read;
                    OnPropertyChanged("Status");
                }
            }

            grid.UnselectAll();
            if (item != null)
                grid.SelectedItem = item;

            grid.ItemFocus(+1);
        }
       
        public bool HasSelected
        {
            get
            {
                return (SelectedEntry != null);
            }
        }

        public Entry SelectedEntry
        {
            get
            {
                Entry entry;
                if (grid.SelectedItem.Cast(out entry))
                {
                    return entry;
                }
                return null;
            }
        }

        public void OpenItem()
        {
            if (HasSelected)
            {
                SelectedEntry.OpenLink();
            }
        }

        private void Mygrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (HasSelected)
                {
                    SelectedEntry.OpenLink();
                    _markSingleRead();
                    e.Handled = true;
                }
            }

        }

        public void MarkRead()
        {
            _markSelectedRead();
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (this.HasSelected)
                {
                    SelectedEntry.OpenLink();
                    _markSingleRead();
                    grid.ItemFocus(+1);
                    e.Handled = true;
                    
                }
            }
        }

        private void ClickMarkEntryAsread(object sender, RoutedEventArgs e)
        {
            _markSelectedRead();
        }
    }
}

