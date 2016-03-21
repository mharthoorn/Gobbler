using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.XPath;

namespace Gobbler
{

    public static class FeedReader
    {
       
        public static void ReadAtom(XPathNavigator root, Feed feed)
        {

            XmlNameTable nameTable = root.NameTable;
            XmlNamespaceManager ns = new XmlNamespaceManager(nameTable);
            ns.AddNamespace("atom", "http://www.w3.org/2005/Atom");


            feed.Name = root.SelectSingleNode("/atom:feed/atom:title", ns).Value;

            foreach (XPathNavigator n in root.Select("/atom:feed/atom:entry", ns))
            {
                Entry item = new Entry();
                item.Status = EntryStatus.Unread;
                item.Title = n.SelectSingleNode("atom:title", ns).Value;

                var xnode = n.SelectSingleNode("atom:summary", ns);
                if (xnode != null)
                {
                    item.Description = xnode.Value;
                }
                else
                {
                    xnode = n.SelectSingleNode("atom:content", ns);
                    if (xnode != null)
                        item.Description = xnode.Value;
                }
                item.Link = n.SelectSingleNode("atom:link/@href", ns).Value;

                item.Published = new Publication(n.SelectSingleNode("atom:updated", ns).Value);

                item.Guid = n.SelectSingleNode("atom:id", ns).Value;
                feed.Add(item);
            }

        }

        public static void ReadRss(XPathNavigator root, Feed feed)
        {
            feed.Name = root.SelectSingleNode("/rss/channel/title").Value;

            foreach (XPathNavigator n in root.Select("/rss/channel/item"))
            {
                Entry item = new Entry();
                item.Status = EntryStatus.Unread;
                item.Title = n.Value("title");
                item.Description = n.Value("description");
                item.Link = n.Value("link");

                item.Published = new Publication(n.Value("pubDate"));
                
                item.Guid = n.Value("guid");
                feed.Add(item);
            }

        }

        public static void Read(XPathNavigator root, Feed feed)
        {
            string element = root.SelectSingleNode("/*").Name;
            if (element == "feed")
            {
                ReadAtom(root, feed);
            }
            else if (element == "rss")
            {
                ReadRss(root, feed);
            }
        }

        public static void Read(Feed feed)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(feed.Uri.ToString());
                XPathNavigator root = doc.CreateNavigator();
                Read(root, feed);
            }
            catch (Exception e)
            {

                throw new InvalidOperationException("The feed could not b read", e);
            }

        }
        public static Feed Read(Uri uri)
        {
            Feed feed = new Feed(uri);
            Read(feed);
            return feed;
        }

        public static Feed Read(string struri)
        {
            Uri uri = new Uri(struri);
            return Read(uri);
        }

        public static void Update(this Feed feed)
        {
            Feed f = Read(feed.Uri);
            feed.Merge(f);
            
            
        }
    }
}
