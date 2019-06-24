using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BOMCSVParserTests
{
    public class UnitTest1
    {
        [Fact]
        public void AllRowsHaveRainfallValues()
        {
            var parser = new BOMCSVParser.BOMCSVParser();
            parser.FilePath = "../../../Files/Jan_Data_Test.csv";

            parser.Parse();

            var data = parser.Data.OrderBy(p => p.Key).ToList();

            Assert.True(data.Count == 31);

            foreach (KeyValuePair<DateTime, float?> row in data)
            {
                Assert.True(row.Value.HasValue);
            }
        }

        [Fact]
        public void AllRowsHaveCorrectYear()
        {
            var parser = new BOMCSVParser.BOMCSVParser();
            parser.FilePath = "../../../Files/Jan_Data_Test.csv";

            parser.Parse();

            var data = parser.Data.OrderBy(p => p.Key).ToList();

            Assert.True(data.Count == 31);

            foreach (KeyValuePair<DateTime, float?> row in data)
            {
                Assert.True(row.Key.Year == 2019);
            }
        }

        [Fact]
        public void RainfallValuesReadCorrectly()
        {
            var parser = new BOMCSVParser.BOMCSVParser();
            parser.FilePath = "../../../Files/Jan_Data_Test.csv";

            parser.Parse();

            var data = parser.Data.OrderBy(p => p.Key).ToList();

            Assert.True(data.Count == 31);

            foreach (KeyValuePair<DateTime, float?> row in data)
            {
                Assert.True(row.Value.Value == (float)8.8);
                // TODO: Iterating through more of the values
                return;
            }
        }
    }
}
