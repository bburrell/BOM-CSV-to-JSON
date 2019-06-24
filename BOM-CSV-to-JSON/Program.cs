using System;
using System.Collections.Generic;
using System.IO;
using BOMCSVParser;

namespace BOM_CSV_to_JSON
{
    class Program
    {
        enum ExitCode
        {
            Success = 0,
            GeneralError = 1,
            FileNotFound = 2,
            PathNotFound = 3
        }

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("usage:");
                Console.WriteLine("BOM_CSV_to_JSON <filepath for CSV to be convert>");
                Environment.ExitCode = (int)ExitCode.GeneralError;
                return Environment.ExitCode;
            }

            BOMCSVParser.BOMCSVParser parser = new BOMCSVParser.BOMCSVParser();
            //parser.FilePath = "../../../../../CSVFiles/2019_Jan-Apr_Data_Test.csv";
            parser.FilePath = args[0];

            try { 
                parser.Parse();
            } catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    Environment.ExitCode = (int)ExitCode.FileNotFound;
                    Console.WriteLine("File not found.");
                } else if (ex is DirectoryNotFoundException)
                {
                    Environment.ExitCode = (int)ExitCode.PathNotFound;
                    Console.WriteLine("Path not found.");
                } else
                {
                    Environment.ExitCode = (int)ExitCode.GeneralError;
                    Console.WriteLine("An error occurred trying to parse the input file.");
                }

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
