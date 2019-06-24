using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace BOMCSVParser
{
    public class BOMCSVParser
    {
        #region Public properties
        public string FilePath { get; set; }
        public IEnumerable<KeyValuePair<DateTime, float?>> Data {get; set;}
        #endregion

        #region Private properties
        private List<DailyRainFall> _dailyRainFallData { get; set; }
        #endregion

        #region Public methods
        // Return true for success, false for errors.
        public void Parse()
        {
                using (var reader = new StreamReader(FilePath))
                using (var csvReader = new CsvReader(reader))
                {
                    csvReader.Configuration.RegisterClassMap<BOMCSVMap>();
                    _dailyRainFallData = csvReader.GetRecords<DailyRainFall>().ToList();
                    Data = _dailyRainFallData.Select(d => new KeyValuePair<DateTime, float?>(d.Date, d.Amount));
                }
        }
        #endregion


        #region Helper classes
        public class DailyRainFall
        {
            public DateTime Date { get; set; }
            public float? Amount { get; set; }
        }

        class BOMCSVMap : ClassMap<DailyRainFall>
        {
            public BOMCSVMap()
            {
                Map(m => m.Date).ConvertUsing(row =>
                {
                    DateTime date;

                    int year = int.Parse(row.GetField("Year"));
                    int month = int.Parse(row.GetField("Month"));
                    int day = int.Parse(row.GetField("Day"));

                    date = new DateTime(year, month, day);

                    return date;
                });
                Map(m => m.Amount).Name("Rainfall amount (millimetres)");
            }
        }
        #endregion
    }
}
