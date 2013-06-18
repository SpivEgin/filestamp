using System;


using CommandLine;
using CommandLine.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FileStamp
{
    ///Type For Parsing Target
    class Options
    {
        [Option('f', "filename", Required = true, HelpText = "The file for which to set the creation date and time")]
        public string InputFile { get; set; }

        [Option('d', "cdate", Required = true, HelpText = "The new creation date - yyyymmdd")]
        public string NewCreationDate { get; set; }

        [Option('t', "ctime", Required = true, HelpText = "The new creation time - hhmmss")]
        public string NewCreationTime { get; set; }

        [Option('v', "verbose", DefaultValue = false, HelpText = "Shows various details, e.g. for error messages")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //var usage = new StringBuilder();
            //usage.AppendLine("FileStamp\n");
            //usage.AppendLine("A utility to change the creation and modification dates of files.\n\n");
            //usage.AppendLine("-f <filename> Set file creation date");
            //usage.AppendLine("-cdate <yyyymmdd> Set file creation date");
            //usage.AppendLine(string.Format("-ctime <hhmmss> Set file creation time", ""));
            //return usage.ToString();
            var help = new HelpText
            {
                Heading = new HeadingInfo("Filestamp", "v1"),
                Copyright = new CopyrightInfo("Pandita Revolution", 2013),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("\n");
            help.AddPreOptionsLine("This utility can be used to modify the creation date of an input file. It comes with no warrenty, security, promises or responsibilities for the author of the software. You effectively have found this on the street. Use it modify it or throw it away.\n\n");
            help.AddPreOptionsLine("Usage: filestamp -f inputfile -d yyyymmdd -t hhmmss");
            help.AddOptions(this);
            return help;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ///Parse Command Line
            Options opts = new Options();
            CommandLine.Parser parser = new Parser();

            // Checking for input and executing command
            if (parser.ParseArguments(args, opts))
            {

                // Testing user input for the date and time using regex :(
                Regex rgx_date = new Regex(@"[0-9]{8}");
                Regex rgx_time = new Regex(@"[0-9]{6}");
                int year = 0;
                int month = 0;
                int day = 0;
                int hour = 0;
                int minutes = 0;
                int seconds = 0;

                if (rgx_date.IsMatch(opts.NewCreationDate) & rgx_time.IsMatch(opts.NewCreationTime))
                {
                    year = Convert.ToInt32(opts.NewCreationDate.Substring(0, 4));
                    month = Convert.ToInt32(opts.NewCreationDate.Substring(4, 2));
                    day = Convert.ToInt32(opts.NewCreationDate.Substring(6, 2));
                    hour = Convert.ToInt32(opts.NewCreationTime.Substring(0, 2));
                    minutes = Convert.ToInt32(opts.NewCreationTime.Substring(2, 2));
                    seconds = Convert.ToInt32(opts.NewCreationTime.Substring(4, 2));
                }
                else
                {
                    Console.WriteLine("The input for the new date and/or the new time seems inccorect. Please check the format:\n\n-d yyyymmdd\n-t hhmmss");
                    return;
                }

                // Trying to change the creation date and time of the file
                try
                {
                    DateTime newCreate = new DateTime(year, month, day, hour, minutes, seconds);
                    Console.WriteLine(newCreate.ToString());
                    File.SetCreationTime(opts.InputFile,newCreate);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Apologies, something went wrong...\n\n");

                    if (opts.Verbose)
                    {
                        Console.WriteLine("Excemption Message: " + ex.Message);
                        Console.WriteLine("\nExcemption StackTrace: " + ex.StackTrace);
                        Console.WriteLine("\n\n\nType filestamp -h for instructions how to use the program.");
                    }
                    else
                    {
                        Console.WriteLine(opts.GetUsage());
                    }
                }
            }
            else
                Console.WriteLine(opts.GetUsage());
        }
    }
}
