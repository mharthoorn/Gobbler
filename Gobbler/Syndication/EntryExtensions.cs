using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobbler
{
    public static class EntryExtensions
    {
        public static bool OpenLink(this Entry entry)
        {
            if (entry != null)
            {
                try
                {
                    Process.Start(entry.Link);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
            
        }

    }
}
