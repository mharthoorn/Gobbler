using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gobbler
{
    public static class GobblerCommands
    {
        static GobblerCommands()
        {
            AddTopFolder = new RoutedUICommand("Add top folder", "AddTopFoldert", typeof(MainWindow));
            AddToFavourites = new RoutedUICommand("Read later", "AddToFavourites", typeof(MainWindow));
            MarkRead = new RoutedUICommand("Mark read", "MarkRead", typeof(MainWindow));
            Edit = new RoutedUICommand("Edit", "Edit Feed", typeof(MainWindow));
        }
        public static RoutedUICommand AddTopFolder { get; private set; }
        public static RoutedUICommand AddToFavourites { get; private set; }
        public static RoutedUICommand MarkRead { get; private set; }

        public static RoutedUICommand Edit { get; private set; }
    }
}
