using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Gobbler
{
    public class Storage
    {
        XmlWriter writer;
        
        IsolatedStorageFile store;
        INode rootfolder;
        string filename;

        public Storage(INode rootfolder, string filename)
        {
            this.rootfolder = rootfolder;
            this.filename = filename;
            store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            
        }

        public void Write(Folder folder)
        {
            writer.WriteStartElement("Folder");
            writer.WriteElementString("Name", folder.Name);
            writer.WriteElementString("Kind", folder.Kind.ToString());
            Write(folder.Nodes);
            WriteEntries(folder);
            writer.WriteEndElement();
        }

        public void Write(Entry entry)
        {
            writer.WriteStartElement("Entry");
            {
                writer.WriteElementString("Title", entry.Title);
                writer.WriteElementString("Link", entry.Link);
                writer.WriteElementString("Guid", entry.Guid);
                writer.WriteElementString("pubDate", entry.Published.Origin);
                writer.WriteElementString("Status", entry.Status.ToString());
                writer.WriteStartElement("Description");
                
                writer.WriteCData(entry.Description);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            
        }

        public void Write(Feed feed)
        {
            writer.WriteStartElement("Feed");
            {
                writer.WriteElementString("Name", feed.Name);
                writer.WriteElementString("Uri", feed.Uri.ToString());
                
                writer.WriteStartElement("Entries");
                foreach (Entry entry in feed.Entries)
                {
                    Write(entry);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public void Write(FeedNode feednode)
        {
            writer.WriteStartElement("FeedNode");
            {
                writer.WriteElementString("Name", feednode.Name);
                writer.WriteElementString("Interval", Converter.StringFromInterval(feednode.Interval));
                if (feednode.Feed is Feed)
                    Write(feednode.Feed as Feed);
            }
            writer.WriteEndElement();
        }

        public void WriteEntries(INode node)
        {
            if (node is FavouritesNode)
            {
                writer.WriteStartElement("Entries");
                {
                    foreach (Entry entry in node.Entries)
                    {
                        Write(entry);
                    }
                }
                writer.WriteEndElement();
            }
        }

        public void Write(Collection<INode> nodes)
        {
            writer.WriteStartElement("Nodes");
            
            foreach (INode node in nodes)
            {
                if (node is Folder)
                    Write(node as Folder);

                else if (node is FeedNode)
                    Write(node as FeedNode);
            }
            writer.WriteEndElement();            
        }

        public void Save()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            using(IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename + ".xml", FileMode.Create, store))
            using(writer = XmlWriter.Create(stream, settings))
            {
                Write(rootfolder.Nodes);
            }
        }

        public void SaveEntries()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename + ".xml", FileMode.Create, store))
            using (writer = XmlWriter.Create(stream, settings))
            {
                if (rootfolder is FavouritesNode)
                {
                    WriteEntries(rootfolder);
                }
            }
        }

        

        public Folder ReadFolder(XPathNavigator xnode)
        {
            string name = xnode.SelectSingleNode("Name").Value;
            NodeKind kind = (NodeKind)Enum.Parse(typeof(NodeKind), xnode.Value("Kind"));
            Folder folder = new Folder(name, kind);
            return folder;
        }

        public Entry ReadEntry(XPathNavigator xnode)
        {
            Entry entry = new Entry();
            entry.Title = xnode.SelectSingleNode("Title").Value;
            entry.Guid = xnode.SelectSingleNode("Guid").Value;
            entry.Link = xnode.SelectSingleNode("Link").Value;
            entry.Description = xnode.SelectSingleNode("Description").Value;
            entry.Published = new Publication(xnode.Value("pubDate"));
            entry.Status = (EntryStatus)Enum.Parse(typeof(EntryStatus), xnode.Value("Status"));
            return entry;

        }

        public ObservableCollection<Entry> ReadEntries(XPathNavigator xnode)
        {
            ObservableCollection<Entry> entries = new ObservableCollection<Entry>();
            foreach (XPathNavigator n in xnode.Select("Entries/Entry"))
            {
                Entry e = ReadEntry(n);
                entries.Add(e);
            }
           
            return entries;
        }

        public Feed ReadFeed(XPathNavigator node)
        {
            Feed f = new Feed();
            f.Name = node.Value("Name");
            f.Uri = new Uri(node.Value("Uri"));
            f.Load(ReadEntries(node));
            return f;
        }

        public FeedNode ReadFeedNode(XPathNavigator xnode)
        {
            Feed f = ReadFeed(xnode.SelectSingleNode("Feed"));
            FeedNode fn = new FeedNode(f);
            fn.Name = xnode.SelectSingleNode("Name").Value;
            string i = xnode.Value("Interval");
            TimeSpan interval;
            if (Converter.IntervalFromString(i, out interval))
            {
                fn.Interval = interval;
            }
            return fn;
 
        }

        public FavouritesNode ReadFavouritesNode(XPathNavigator xnode)
        {
            string name = xnode.Value("Name");
            FavouritesNode fav = new FavouritesNode(name);

            fav.Entries = ReadEntries(xnode);
            return fav;            
        }

        public void ReadNodes(XPathNavigator xnode, INode owner)
        {
            //ObservableCollection<INode> collection = new ObservableCollection<INode>();
            foreach (XPathNavigator n in xnode.Select("Nodes/*"))
            {
                if (n.Name == "Folder")
                {
                    Folder f = ReadFolder(n);
                    owner.Nodes.Add(f);
                    ReadNodes(n, f);
                }
                else if (n.Name == "FeedNode")
                {
                    FeedNode fn = ReadFeedNode(n);
                    owner.Nodes.Add(fn);
                }
                else if (n.Name == "FavNode")
                {
                    FavouritesNode fav = ReadFavouritesNode(n);
                    owner.Nodes.Add(fav);
                }
            }
        }

        public bool Load()
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;

                XmlDocument document = new XmlDocument();
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename + ".xml", FileMode.Open, store))
                {
                    document.Load(stream);
                    XPathNavigator xroot = document.CreateNavigator();
                    ReadNodes(xroot, rootfolder);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool LoadEntries()
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;

                XmlDocument document = new XmlDocument();
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename + ".xml", FileMode.Open, store))
                {
                    document.Load(stream);
                    XPathNavigator xroot = document.CreateNavigator();
                    rootfolder.Entries = ReadEntries(xroot);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
