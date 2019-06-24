using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace BOMCSVParser
{
    [TestFixture]
    public class BOMCSVParserTests
    {
        [Test]
        public void Averages_Test()
        {
            var parser = new BOMCSVParser();
            parser.FilePath = "../../../BOMCSVParser/Testing/Files/Jan_Data_Test.csv";

            parser.Parse();

            var data = parser.Data.OrderBy(p => p.Key).ToList();

            foreach (KeyValuePair<DateTime, float?> row in data)
            {
                Assert.IsTrue(row.Value.HasValue);
                Assert.IsTrue(row.Key.Year == 2019);
            }
        }
    }
}
