using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gobbler;

namespace Tests
{
    [TestClass]
    public class FeedParsing
    {
        [TestMethod]
        public void TestDateTimeConversions()
        {
            string s = "Thu, 19 Jun 2014 00:00:00 CDT";
            //DateTime.Parse()
            //DateTime convertedDate = DateTime.SpecifyKind( DateTime.Parse(s), DateTimeKind.Utc);
            DateTime result;
            SyndicationDateTimeUtility.TryParseRfc822DateTime(s, out result);


        }
    }
}
