using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Gobbler
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            tree.tree.SelectedItemChanged += tree_SelectedItemChanged;
            grid.PropertyChanged += Grid_PropertyChanged;

            Init();
            Load();
            AsyncPulseAll();
            StartTimer();
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            tree.Refresh();
        }

        INode feeds, favourites, readlater;
        Storage feedstore, favstore, laterstore;

        DispatcherTimer timer;

        public void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 5, 0);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            AsyncPulseAll();
        }

        public void Init()
        {
            tree.ItemsSource = null;

            tree.root = new Folder("Root", NodeKind.Root);
            tree.ItemsSource = tree.root.Nodes;

            feeds = new Folder("Feeds", NodeKind.Feeds);
            favourites = new FavouritesNode("Favourites");
            readlater = new FavouritesNode("Reed later");

            tree.root.Nodes.Add(feeds);
            tree.root.Nodes.Add(favourites);
            tree.root.Nodes.Add(readlater);

            feedstore = new Storage(feeds, "folders");
            favstore = new Storage(favourites, "favourites");
            laterstore = new Storage(readlater, "later");
        }

        public void Load()
        {
            feedstore.Load();
            favstore.Load();
            laterstore.LoadEntries();
            tree.Refresh();
        }

        public void Save()
        {
            feedstore.Save();
            favstore.Save();
            laterstore.SaveEntries();
        }

        public void WaitWhile(Action action)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                action();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        
        public void AsyncUpdateNode(INode node)
        {
            Task.Run(delegate ()
            {
                node.Archive();
                node.Update();
            });
            tree.Refresh();
        }

        public void ForceFullReset()
        {
            List<FeedNode> list = feeds.All().OfType<FeedNode>().ToList();
            foreach (FeedNode n in list)
            {
                n.Feed.Clear();
            }
        }

        public void Refresh()
        {
            tree.Refresh();
            SyncCurrentNode();
        }

        object updater = new object();

        public void AsyncRun(INode root, Action<INode> action)
        {
            UpdateStatus.Visibility = Visibility.Visible;

            ProgressBarWorker.Work(bar,

            delegate (BackgroundWorker worker)
            {
                lock (ProgressBarWorker.Passage)
                {
                    if (root is FeedNode)
                    {
                        action(root);
                    }
                    else
                    {
                        int idx = 0;
                        List<FeedNode> list = root.All().OfType<FeedNode>().ToList();
                        foreach (FeedNode node in list)
                        {
                            action(node);
                            idx++;
                            int p = 100 * idx / list.Count;
                            worker.ReportProgress(p);
                        }
                    }
                }
            },

            Refresh,
            
            delegate ()
            {
                UpdateStatus.Visibility = Visibility.Hidden;
                
            });

        }

        public void AsyncPulseAll()
        {
            AsyncRun(tree.root, n => n.Pulse());
        }

        public void AsyncArchiveAndUpdate(INode node)
        {
            AsyncRun(node, n => { n.Archive(); n.Update(); });
        }

        public void AsyncPulse(INode node)
        {
            AsyncRun(node, n => n.Pulse());
        }

        public void AsyncArchiveAndUpdateCurrentNode()
        {
            INode node;
            if (tree.GetSelectedNode(out node))
            {
                AsyncArchiveAndUpdate(node);
            }
        }

        private void SyncGrid(INode node)
        {
            if (grid.Items != node.Entries)
            {
                grid.Items = node.Entries;
            }
        }

        private void SyncCurrentNode()
        {
            INode node;
            if (tree.GetSelectedNode(out node))
            {
                SyncGrid(node);
            }
        }

        #region events

        private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            INode node;
            if (e.NewValue.Cast(out node))
                SyncGrid(node);
        }
                
        private void MenuAddFolder_Click(object sender, RoutedEventArgs e)
        {
            Folder folder;
            if (tree.GetSelectedFolder(out folder))
            {
                Folder sub = new Folder("<name>", NodeKind.Folder);
                if (WindowAskFolderName.Ask(folder, ref sub))
                {
                    folder.Nodes.Add(sub);
                    folder.IsExpanded = true;
                    sub.IsSelected = true;
                }
            }
        }

        private void MenuAddFeed_Click(object sender, RoutedEventArgs e)
        {
            INode parent;
            if (tree.GetSelectedNode(out parent))
            {
                if (parent is FeedNode)
                {
                    parent = tree.root.ParentOf(parent);
                }

                if (parent is Folder)
                {
                    Feed f = new Feed();
                    FeedNode node = new FeedNode(f);
                    if (WindowAskFeed.Ask(this, "New feed", node))
                    {
                        parent.Nodes.Add(node);
                        node.Update();
                        parent.IsExpanded = true;
                        node.IsSelected = true;
                        tree.Refresh();
                    }
                }
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            lock (ProgressBarWorker.Passage)
            {
                Application.Current.Shutdown();
            }
        }
        
        private void MenuSaveFile_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MenuNewFile_Click(object sender, RoutedEventArgs e)
        {
            tree.ItemsSource = null;
            grid.Items = null;
        }

        private void MenuLoadFile_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Save();
        }

        private void ExecDeleteNode(object sender, ExecutedRoutedEventArgs e)
        {
            INode node;
            if (tree.GetSelectedNode(out node))
            {
                var r = MessageBox.Show("Are you sure you want to delete\n " + node.Name, "Confirm", MessageBoxButton.OKCancel);
                if (r == MessageBoxResult.OK)
                {
                    tree.root.Delete(node);
                    tree.Refresh();
                }
            }
        }

        private void Click_About(object sender, RoutedEventArgs e)
        {
            WindowAbout w = new WindowAbout();
            w.Owner = this;
            w.ShowDialog();
        }

        private void ExecRefresh(object sender, ExecutedRoutedEventArgs e)
        {
            AsyncArchiveAndUpdateCurrentNode();
        }

        private void ExecEditFeed(object sender, ExecutedRoutedEventArgs e)
        {
            WindowAbout w = new WindowAbout();
            w.Owner = this;
            w.ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuShowArchive_Click(object sender, RoutedEventArgs e)
        {
            INode node;
            if (tree.GetSelectedNode(out node))
            {
                FeedNode f = (node as FeedNode);
                if (f != null)
                {
                    grid.Items = (f.Feed.Entries.Where(entry  => entry.Status == EntryStatus.Archived));
                }
            }
        }

        private void MenuRefreshAll_Click(object sender, RoutedEventArgs e)
        {
            AsyncRun(tree.root, n => n.Update());
        }

        private void MenuClearItems_Click(object sender, RoutedEventArgs e)
        {
            foreach (FeedNode f in feeds.All().OfType<FeedNode>())
            {
                f.Clear();
                f.Feed.Clear();
                tree.root.Refresh();
            }

            
        }

        private void ExecAddToFavourites(object sender, ExecutedRoutedEventArgs e)
        {
            Entry origin = grid.SelectedEntry;
            if (origin != null)
            {
                Entry entry = origin.Clone();
                entry.Status = EntryStatus.Starred;
                readlater.Entries.Add(entry);
                origin.Status = EntryStatus.Archived;
                SyncCurrentNode();
                tree.root.Refresh();
            }
        }

        private void ExecMarkRead(object sender, ExecutedRoutedEventArgs e)
        {
            grid.MarkRead();
            tree.root.RefreshCount();
        }

        #endregion

    }

}
