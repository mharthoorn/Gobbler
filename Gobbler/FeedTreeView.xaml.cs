using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gobbler
{
    /// <summary>
    /// Interaction logic for FeedTreeView.xaml
    /// </summary>
    public partial class FeedTreeView : UserControl
    {
        public FeedTreeView()
        {
            InitializeComponent();
            
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return this.tree.ItemsSource;
            }
            set
            {
                this.tree.ItemsSource = value;
            }
        }
        public INode root { get; set; }

        public INode SelectedNode
        {
            get
            {
                INode node;
                if (tree.SelectedItem.Cast(out node))
                {
                    return node;
                }
                else
                {
                    return null;
                }
            }
        }
        public bool GetSelectedNode(out INode node)
        {
            node = SelectedNode;
            return (node != null);
        }
        public bool GetSelectedFolder(out Folder folder)
        {
            INode node = SelectedNode;
            folder = (node as Folder);
            return (folder != null);
        }

        public void Refresh()
        {
            root.Refresh();
        }

        public class Family
        {
            public INode Parent;
            public INode Child;
        }
        private void tree_MouseMove(object sender, MouseEventArgs e)
        {
            INode node;
            if (e.LeftButton == MouseButtonState.Pressed && GetSelectedNode(out node) && node.IsUserNode() )
            {
                INode parent = root.ParentOf(node);
                Family family = new Family { Parent = parent, Child = node };

                DragDrop.DoDragDrop(tree, family, DragDropEffects.All);
            }
        }

        private void tree_Drop(object sender, DragEventArgs e)
        {
            Point point = e.GetPosition((UIElement)sender);

            Family family = (Family)e.Data.GetData(typeof(Family));
            INode target = GetItemAtLocation(point);
            if (family.Child != null && target != null && family.Child != target)
            {
                if (target is Folder)
                {
                    Folder f = (target as Folder);
                    family.Parent.Delete(family.Child);
                    f.Nodes.Add(family.Child);

                    target.IsExpanded = true; //todo: this doesn't always work.
                    family.Child.IsSelected = true;
                }
            }
        }

        private void tree_DragOver(object sender, DragEventArgs e)
        {
            Point point = e.GetPosition((UIElement)sender);
            INode target = GetItemAtLocation(point);
            Family family = (Family)e.Data.GetData(typeof(Family));
            if (family.Child != target)
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                
            }
        }

        private void StackPanel_DragOver(object sender, DragEventArgs e)
        {

        }

        INode GetItemAtLocation(Point location)
        {
            INode node = default(INode);
            HitTestResult hitTestResults = VisualTreeHelper.HitTest(tree, location);

            if (hitTestResults.VisualHit is FrameworkElement)
            {
                object data = (hitTestResults.VisualHit as FrameworkElement).DataContext;

                if (data is INode)
                {
                    node = (INode)data;
                }
            }

            return node;
        }
    }
}
