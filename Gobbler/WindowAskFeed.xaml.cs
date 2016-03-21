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
    /// Interaction logic for WindowAskFeed.xaml
    /// </summary>
    public partial class WindowAskFeed : Window
    {
        FeedNode node;

        public WindowAskFeed(string title, FeedNode feednode)
        {
            InitializeComponent();
            this.Title = title;
            this.node = feednode;

        }

        public TimeSpan Interval;
        
        private void setFields()
        {
            edtName.Text = node.Name;
            edtUrl.Text = (node.Feed.Uri != null) ? node.Feed.Uri.ToString() : null;
        }

        private void getfields()
        {
            node.Name = edtName.Text;
            node.Feed.Name = node.Name;
            node.Feed.Uri = new Uri(edtUrl.Text);
            node.Interval = Interval;
        }

        public bool TestFeed(string url, out Feed feed)
        {
            try
            {
                Feed test = FeedReader.Read(url);
                feed = test;
                return true;
            }
            catch
            {
                feed = null;
                return false;
            }
        }
        private bool validate()
        {
            if (edtName.Text.Length < 3)
            {
                txtError.Text = "Name is too short";
                txtError.Visibility = Visibility.Visible;
                return false;
            }

            Uri result;
            bool valid = Uri.TryCreate(edtUrl.Text, UriKind.Absolute, out result);
            if (!valid)
            {
                txtError.Text = "Url is not valid";
                txtError.Visibility = Visibility.Visible;
                return false;
            }

            valid = Converter.IntervalFromString(edtInterval.Text, out Interval);
            if (!valid)
            {
                txtError.Text = "Time span is not valid";
                txtError.Visibility = Visibility.Visible;
                return false;
            }

            Feed test;
            if (!TestFeed(edtUrl.Text, out test))
            {
                txtError.Text = "The feed is not valid";
                txtError.Visibility = Visibility.Visible;
                return false;
            }
            return true;
        }

        private void ClickOk(object sender, RoutedEventArgs e)
        {
            if (validate())
                DialogResult = true;
        }

        private void ClickCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public bool Dialog()
        {
            setFields();
            bool ok = this.ShowDialog() ?? false;
            if (ok)
            {
                getfields();
            }
            return ok;
        }

        public static bool Ask(Window owner, string title, FeedNode feednode)
        {
            WindowAskFeed w = new WindowAskFeed(title, feednode);
            w.Owner = owner;
            return w.Dialog();
        }

        private void edtName_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Feed feed;
            if (TestFeed(edtUrl.Text, out feed))
            {
                (sender as TextBox).Text = feed.Name;
            }
        }
    }
}
