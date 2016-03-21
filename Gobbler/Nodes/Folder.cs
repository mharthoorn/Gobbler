using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobbler
{
    public class Folder : Node, INotifyPropertyChanged
    {
        public Folder(string name, NodeKind kind) : base(name, kind) { }

        public void UpdateChildren()
        {
            foreach (Node node in Nodes)
            {
                node.Update();
            }
        }

        public void RefreshEntries()
        {
            IEnumerable<Entry> input = Nodes.SelectMany(n => n.Entries).OrderBy(e => e.Published.When);
            Entries.Update(input);
        }

        public void RefreshChildren()
        {
            foreach (Node node in Nodes)
            {
                node.Refresh();
            }
        }

        public override void Archive()
        {
            foreach (INode node in Nodes)
            {
                node.Archive();
            }
        }

        public override void Update()
        {
            //UpdateChildren();
        }

        public override void Refresh()
        {
            RefreshChildren();
            RefreshEntries();
            RefreshCount();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Count);
        }

        public override void RefreshCount()
        {
            int c = 0;
            foreach (INode n in Nodes)
            {
                n.RefreshCount();
                c += n.Count;
            }
            this.Count = c;

        }

        public override void Pulse()
        {
            foreach (INode node in Nodes)
            {
                node.Pulse();
            }
        }
    }

}

    
  
