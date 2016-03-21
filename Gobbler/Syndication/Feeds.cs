using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobbler
{
    class Feeds
    {
        List<Feed> list = new List<Feed>();
        public void Add(Uri uri)
        {
            Feed feed = new Feed(uri.ToString());
            feed.Update();
            list.Add(feed);
        }
    }
}
