using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Gobbler
{
    public static class VisualsExtensions
    {
    
        public static void ExpandAll(this TreeView tree)
        {
            
            foreach (TreeViewItem item in tree.Items)
            {
                item.ExpandSubtree();
            }
        }

        public static bool Select(this ItemsControl control, object x)
        {
            if (control.HasItems)
            {
                foreach (var item in control.Items)
                {
                    var pin = control.ItemContainerGenerator.ContainerFromItem(item);
                    TreeViewItem node = pin as TreeViewItem;

                    if (node != null)
                    {
                        if (item == x)
                        {
                            node.IsSelected = true;
                            return true;
                        }
                        else if (node.Select(x))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void FocusItem(this TreeView tree, object item)
        {
            var pin = tree.ItemContainerGenerator.ContainerFromItem(item);
            TreeViewItem node = pin as TreeViewItem;
            node.IsExpanded = true;
            tree.Select(item);
        }
        public static void ItemFocus(this DataGrid grid, int relative)
        {
            if (grid.Items.Count > 0)
            {
                int index = grid.SelectedIndex + relative;
                if (index < 0) index = 0;
                if (index > grid.Items.Count - 1) index = grid.Items.Count - 1;

                grid.SelectedIndex = index; // view
                grid.CurrentItem = grid.Items[index]; // data
                grid.ScrollIntoView(grid.SelectedItem);
            }
        }

        

    }
}
