using System;
using System.Collections.Generic;
using BOMCSVParser;

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
            BOMCSVParser.BOMCSVParser parser = new BOMCSVParser.BOMCSVParser();
            parser.FilePath = "../../../../../CSVFiles/2018-2019_Data_Test.csv";

            var parseSuccess = parser.Parse();

            if (!parseSuccess)
            {
                // Early exit if error with parsing the BOM CSV file.
                var errors = parser.GetErrors();
                foreach (KeyValuePair<int, string> error in errors)
                {
                    Console.WriteLine($"Error {error.Key}: {error.Value}");
                }

                Environment.ExitCode = (int)ExitCode.UnknownError;
                return Environment.ExitCode;
            }

            // Continue to now generate JSON.
            JSONBuilder.JSONBuilder builder = new JSONBuilder.JSONBuilder();
            builder.Data = parser.Data;

            string json = builder.BuildJson();

            Console.WriteLine(json);

            Environment.ExitCode = (int)ExitCode.Success;
            return Environment.ExitCode;
        }
    }
}
