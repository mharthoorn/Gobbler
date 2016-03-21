using System;
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
using System.Windows.Shapes;

namespace Gobbler
{
    /// <summary>
    /// Interaction logic for WindowAskGroupName.xaml
    /// </summary>
    public partial class WindowAskFolderName : Window
    {
        public WindowAskFolderName(string title)
        {
            InitializeComponent();
            this.Title = title;
        }

        

        private void ClickOk(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public bool Dialog()
        {
            bool? ok = this.ShowDialog();
            return ok ?? false;
        }

        public static bool Ask(string title, ref string groupname)
        {
            WindowAskFolderName w = new WindowAskFolderName(title);
            
            w.edtGroup.Text = groupname;
            bool ok = w.Dialog();
            
            if (ok)
                groupname = w.edtGroup.Text;
            return ok;
        }

        public static bool Ask(INode parent, ref Folder folder)
        {
            string name = null;
            if (WindowAskFolderName.Ask("New folder", ref name))
            {
                folder.Name = name;
                return true;
            }
            else
            {
                folder = null;
                return false;
            }

        }

     
    }
}
