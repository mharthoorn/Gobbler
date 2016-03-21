using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobbler
{
    public class Publication
    {
        public string Origin { get; set; }
        public DateTime When { get; set; }
        public Publication(string origin)
        {
            this.Origin = origin;
            DateTime result;
            Understood = SyndicationDateTimeUtility.TryParseRfc822DateTime(origin, out result);
            if (Understood)
            {
                When = result;
            }
        }

        public bool Understood;

        public override string ToString()
        {
            if (Understood)
            {
                return When.ToString("dd MMMM");
            }
            else
            {
                return Origin;
            }
        }
    }
}
