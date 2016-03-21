using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobbler
{
    public class FavouritesNode : Node, INotifyPropertyChanged
    {
        public FavouritesNode(string name) : base(name, NodeKind.Favourites) { }
        public FavouritesNode() : base("<name>", NodeKind.Favourites) { }

        public override void Archive()
        {
            // doet niets.
        }
        public override void Update()
        {
            Refresh();
        }

        public override void Refresh()
        {
            RefreshCount();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Count);
        }
        public override void RefreshCount()
        {
            this.Count = Entries.Count;
        }

        public override void Pulse()
        {
            // should do nothing.
        }
    }
}
