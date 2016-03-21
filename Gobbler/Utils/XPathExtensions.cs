using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Gobbler
{
    public static class XPathExtensions
    {
        public static string Value(this XPathNavigator navigator, string name)
        {
            XPathNavigator n = navigator.SelectSingleNode(name);
            if (n != null)
                return n.Value;
            else
                return null;
        }
    }
}
