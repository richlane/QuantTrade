using QuantTrade.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTrade.Core.Utilities
{
   public static class Logger
    {
        private static string _transactionLogLocation;
        private static string _resultsLogFile;
        private static readonly object _locker = new object();


        static Logger()
        {
            if(string.IsNullOrEmpty(_transactionLogLocation))
            {
                //Results Log file
                _resultsLogFile = Config.GetToken("results-log");
                if (File.Exists(_resultsLogFile))
                {
                    File.Delete(_resultsLogFile);
                }

                //Transaction log
                _transactionLogLocation = Config.GetToken("transaction-log-location");
                if(!_transactionLogLocation.EndsWith(@"\"))
                    _transactionLogLocation = _transactionLogLocation + @"\";
                
            }
        }

        /// <summary>
        /// Write to the transaction log file
        /// </summary>
        /// <param name="message"></param>
        public static void LogTransaction(string message, string symbol)
        {
            string fileName = $"{_transactionLogLocation}Trans_{symbol}.csv";

            lock (_locker)
            {
                //Clean old files
                if (File.Exists(fileName))
                    File.Delete(fileName);
 
                File.AppendAllText(fileName, message + Environment.NewLine);
            }
        }

        /// <summary>
        /// Logs the report outputs to log file
        /// </summary>
        /// <param name="message"></param>
        public static void LogResults(string message)
        {
            lock (_locker)
            {
                File.AppendAllText(_resultsLogFile, message + Environment.NewLine);
            }
        }


        /// <summary>
        /// Writes to the console window
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;

            Console.WriteLine(message);
        }
    }
}
