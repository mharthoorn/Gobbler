using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobbler
{
    public class Converter
    {
        public static bool IntervalFromString(string s, out TimeSpan interval)
        {
            try
            {
                string[] hm = s.Split(':');
                if (hm.Count() == 2)
                {
                    int hours, minutes;
                    if (int.TryParse(hm[0], out hours))
                        if (int.TryParse(hm[1], out minutes))
                        {
                            interval = new TimeSpan(hours, minutes, 0);
                            return true;
                        }
                }
                {
                    interval = default(TimeSpan);
                    return false;
                }
                
            }
            catch
            {
                interval = default(TimeSpan);
                return false;
            }
            
        }

        public static string StringFromInterval(TimeSpan interval)
        {
            return interval.ToString();
        }
    }
}
