using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace ExecLogDiag
{
    class Program
    {
        static void Main(string[] args)
        {
            
            if (args.Length == 2 || args.Length == 4)
            {
                string loglevel = "";
                if(args.Length == 4)
                {
                    if (args[2] == "-loglevel")
                    {
                        loglevel = args[3];
                    }
                }

                if(args[0] == "-f")
                {
                    string filepath = args[1];
                    string text = File.ReadAllText(filepath);
                    string[] lines = text.Split(
                                         new[] { Environment.NewLine },
                                          StringSplitOptions.None
                                         );
                    
                    List<LogData> lstLogData = new List<LogData>();
                    int linenum = 0;
                    foreach (string line in lines)
                    {
                        try
                        {
                            linenum++;
                            LogData objLogData = new LogData();
                            var firstSpaceIndex = line.IndexOf(" ");
                            string firstStr = line.Substring(0, firstSpaceIndex);
                            string remainingString = line.Substring(firstSpaceIndex + 1);
                            var secStrSpaceIdx = remainingString.IndexOf(" ");
                            string secondStr = remainingString.Substring(0, secStrSpaceIdx);
                            string thirdStr = remainingString.Substring(secStrSpaceIdx + 1);
                            //Console.WriteLine(firstStr);
                            //Console.WriteLine(secondStr);
                            //Console.WriteLine(thirdStr);
                            dynamic jsondata = JObject.Parse(thirdStr);
                            objLogData.LogNum = linenum;
                            objLogData.TimeStamp = firstStr;
                            objLogData.LogLevel = secondStr;
                            objLogData.LogMessage = jsondata.message;
                            objLogData.ProcessName = jsondata.processName;
                            objLogData.FileName = jsondata.fileName;
                            objLogData.RobotName = jsondata.robotName;

                            lstLogData.Add(objLogData);
                        }
                        catch(Exception ex)
                        {
                            //do nothing
                        }
                    }
                    Console.WriteLine("Number of Logs generated: "+lstLogData.Count);
                    
                    if(loglevel == "")
                    {
                        var errormsgs = lstLogData.Where(obj => obj.LogLevel.ToLower() == "error").Select(obj => new { lineNum = obj.LogNum, timestmp = obj.TimeStamp, filename = obj.FileName, processName = obj.ProcessName, errMsg = obj.LogMessage });
                        var warnmsgs = lstLogData.Where(obj => obj.LogLevel.ToLower() == "warn").ToList();
                        var infomsgs = lstLogData.Where(obj => obj.LogLevel.ToLower() == "info").ToList();
                        var robotname = lstLogData.Select(obj => obj.RobotName).Distinct().First();
                        var processnames = lstLogData.Select(obj => obj.ProcessName).Distinct().ToList();
                        //machinename
                        //user id


                        Console.WriteLine();
                        Console.WriteLine("Total number of error Log: " + errormsgs.Count());
                        Console.WriteLine("Total number of Warning Log: " + warnmsgs.Count());
                        Console.WriteLine("Total number of Information Log: " + infomsgs.Count());
                        Console.WriteLine("Executing Robot: " + robotname);
                        Console.WriteLine("Processes Executed:");
                        foreach(var process in processnames)
                        {
                            Console.WriteLine(process);
                        }
                        Console.WriteLine("==============================================================================");
                        foreach (var msg in errormsgs)
                        {
                            Console.WriteLine("Log#: " + msg.lineNum);
                            Console.WriteLine("Timestamp: " + msg.timestmp);
                            Console.WriteLine("Filename: " + msg.filename);
                            if(processnames.Count>1)
                            {
                                Console.WriteLine("ProcessName: " + msg.processName);
                            }
                            Console.WriteLine("ErrorMessage: " + msg.errMsg);
                            Console.WriteLine("");
                        }
                    }
                    else
                    {
                        var msgs = lstLogData.Where(obj => obj.LogLevel == loglevel).Select(obj => new { lineNum = obj.LogNum, timestmp = obj.TimeStamp, filename = obj.FileName, errMsg = obj.LogMessage });
                        Console.WriteLine("Total number of "+loglevel+": " + msgs.Count());
                        Console.WriteLine("==============================================================================");
                        foreach (var msg in msgs)
                        {
                            Console.WriteLine("Log#: " + msg.lineNum);
                            Console.WriteLine("Timestamp: " + msg.timestmp);
                            Console.WriteLine("Filename: " + msg.filename);
                            Console.WriteLine(loglevel+" Message: " + msg.errMsg);
                            Console.WriteLine("");
                        }
                    }

                }
                else
                {
                    Console.WriteLine("Invalid Input. Type 'ExecLogDia.exe -help' for help documents for the Utility");
                }
            }

            if(args.Length == 1)
            {
                if (args[0] == "-help")
                {
                    Console.WriteLine("\nThis Utility provides analyzes the execution log and provides required details to the user\n");
                    Console.WriteLine("ExecLogDiag.exe -help: Provides all help documents for the Utility");
                    Console.WriteLine("ExecLogDiag.exe -f <log_file>: Provides Summary of the File and Error Logs");
                    Console.WriteLine("ExecLogDiag.exe -f <log_file> -loglevel <LogLevel>: filters the logs based on the given logging level");
                    Console.WriteLine("ExecLogDiag.exe -f <log_file> -tocsv: exports the logs to the csv in Desktop");
                }
                else
                {
                    Console.WriteLine("Type 'ExecLogDia.exe -help' for help documents for the Utility");
                }
            }
            

            if (args.Length == 3)
            {
                List<LogData> lstLogData = new List<LogData>();

                if (args[0] == "-f" && args[2] == "-tocsv")
                {
                    string filepath = args[1];
                    string text = File.ReadAllText(filepath);
                    string[] lines = text.Split(
                                         new[] { Environment.NewLine },
                                          StringSplitOptions.None
                                         );

                    //List<LogData> lstLogData = new List<LogData>();
                    int linenum = 0;
                    foreach (string line in lines)
                    {
                        try
                        {
                            linenum++;
                            LogData objLogData = new LogData();
                            var firstSpaceIndex = line.IndexOf(" ");
                            string firstStr = line.Substring(0, firstSpaceIndex);
                            string remainingString = line.Substring(firstSpaceIndex + 1);
                            var secStrSpaceIdx = remainingString.IndexOf(" ");
                            string secondStr = remainingString.Substring(0, secStrSpaceIdx);
                            string thirdStr = remainingString.Substring(secStrSpaceIdx + 1);
                            //Console.WriteLine(firstStr);
                            //Console.WriteLine(secondStr);
                            //Console.WriteLine(thirdStr);
                            dynamic jsondata = JObject.Parse(thirdStr);
                            objLogData.LogNum = linenum;
                            objLogData.TimeStamp = firstStr;
                            objLogData.LogLevel = secondStr;
                            objLogData.LogMessage = jsondata.message;
                            objLogData.ProcessName = jsondata.processName;
                            objLogData.FileName = jsondata.fileName;
                            objLogData.RobotName = jsondata.robotName;

                            lstLogData.Add(objLogData);
                        }
                        catch (Exception ex)
                        {
                            //do nothing
                        }
                    }

                    using (var mem = new MemoryStream())
                    using (var writer = new StreamWriter(mem))
                    using (var csvWriter = new CsvWriter(writer))
                    {
                        csvWriter.Configuration.Delimiter = ",";
                        csvWriter.Configuration.HasHeaderRecord = true;
                        csvWriter.Configuration.AutoMap<LogData>();

                        csvWriter.WriteHeader<LogData>();
                        csvWriter.WriteRecords(lstLogData);

                        writer.Flush();
                        var result = Encoding.UTF8.GetString(mem.ToArray());
                        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        string srcFile = args[1];
                        string srcFileName = srcFile.Split('\\').Last().Split('.').First() + ".csv";
                        Console.WriteLine("Writing content to "+srcFileName+" in Desktop");
                        //Console.WriteLine(result);
                        File.WriteAllText(Path.Combine(desktop,srcFileName), result);
                    }
                }
            }
        }
    }
}
