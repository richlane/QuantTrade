using QuantTrade.Core.Configuration;
using System;
using System.IO;

namespace QuantTrade.Core.Utilities
{
    /// <summary>
    /// Logging utilities class
    /// </summary>
    public static class Logger
    {
        #region Properties

        private static string _transactionLogLocation;
        private static string _resultsLogFile;
        private static readonly object _locker = new object();

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        static Logger()
        {
            //get log file locations
            if(string.IsNullOrEmpty(_transactionLogLocation) || string.IsNullOrEmpty(_resultsLogFile))
            {
                //Results Log file
                _resultsLogFile = Config.GetToken("results-log");
                if (File.Exists(_resultsLogFile))
                {
                    File.Delete(_resultsLogFile);
                }

                //Transaction log file
                _transactionLogLocation = Config.GetToken("transaction-log-location");
                if(!_transactionLogLocation.EndsWith(@"\"))
                    _transactionLogLocation = _transactionLogLocation + @"\";
                
            }
        }

        /// <summary>
        /// Writes transaction to a log file.
        /// </summary>
        public static void LogTransactionsToFile(string transactions, string symbol)
        {
            string fileName = $"{_transactionLogLocation}Trans_{symbol}.csv";

            lock (_locker)
            {
                //Clean old files
                if (File.Exists(fileName))
                    File.Delete(fileName);
 
                File.AppendAllText(fileName, transactions + Environment.NewLine);
            }
        }

        /// <summary>
        /// Logs the report output to log file.
        /// </summary>
        public static void LogReportResultsToFile(string results)
        {
            lock (_locker)
            {
                File.AppendAllText(_resultsLogFile, results + Environment.NewLine);
            }
        }


        /// <summary>
        /// Writes to the console window.
        /// </summary>
        public static void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;

            Console.WriteLine(message);
        }
    }
}
