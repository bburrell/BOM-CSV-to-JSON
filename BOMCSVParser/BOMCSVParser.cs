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
        public string FilePath { get; set; }
        private List<DailyRainFall> _dailyRainFallData { get; set; }

        public IEnumerable<KeyValuePair<DateTime, float?>> Data {get; set;}

        private List<KeyValuePair<int, string>> errors { get; set; }

        public List<KeyValuePair<int, string>> GetErrors()
        {
            return errors;
        }

        // Return true for success, false for errors.
        public bool Parse()
        {
            try
            {
                using (var reader = new StreamReader(FilePath))
                using (var csvReader = new CsvReader(reader))
                {
                    csvReader.Configuration.RegisterClassMap<BOMCSVMap>();
                    _dailyRainFallData = csvReader.GetRecords<DailyRainFall>().ToList();
                    Data = _dailyRainFallData.Select(d => new KeyValuePair<DateTime, float?>(d.Date, d.Amount));
                }
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }

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
    }
}
