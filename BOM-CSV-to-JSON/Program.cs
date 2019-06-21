using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using System.Linq;
using CsvHelper.Configuration;

namespace BOM_CSV_to_JSON
{
    class Program
    {
        enum ExitCode
        {
            Success = 0,
            FileNotFound = 2,
            PathNotFound = 3,
            UnknownError = 10
        }

        static int Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var parser = new BOMCSVParser();

            parser.FilePath = "../../../../../CSVFiles/IDCJAC0009_066062_1800_Data.csv";

            var parseSuccess = parser.Parse();

            if (!parseSuccess)
            {
                var errors = parser.GetErrors();
                foreach (KeyValuePair<int, string> error in errors)
                {
                    Console.WriteLine($"Error {error.Key}: {error.Value}");
                }

                Environment.ExitCode = (int)ExitCode.UnknownError;
            }
            else
            {
                Environment.ExitCode = (int)ExitCode.Success;
            }

            return Environment.ExitCode;
        }
    }

    public class BOMCSVParser
    {
        public string FilePath { get; set; }
        public List<DailyRainFall> Data { get; set; }
        private List<KeyValuePair<int, string>> errors { get; set; }

        // Return true for success, false for errors.
        public bool Parse()
        {
            try
            {
                using (var reader = new StreamReader(FilePath))
                using (var csvReader = new CsvReader(reader))
                {
                    csvReader.Configuration.RegisterClassMap<BOMCSVMap>();
                    Data = csvReader.GetRecords<DailyRainFall>().ToList();
                }
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }

        public List<KeyValuePair<int, string>> GetErrors()
        {
            return errors;
        }


        public class DailyRainFall
        {
            public DateTime? Date { get; set; }
            public float? Amount { get; set; }
        }

        class BOMCSVMap : ClassMap<DailyRainFall>
        {
            public BOMCSVMap()
            {
                Map(m => m.Date).ConvertUsing(row =>
                {
                    DateTime? date = null;

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
