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
        private static string _transactionLogFile;
        private static readonly object _locker = new object();


        static Logger()
        {
            if(string.IsNullOrEmpty(_transactionLogFile))
            {
                _transactionLogFile = Config.GetToken("transaction-log");
                if(File.Exists(_transactionLogFile))
                {
                    File.Delete(_transactionLogFile);
                }
            }
        }

        /// <summary>
        /// Write to the transaction log file
        /// </summary>
        /// <param name="message"></param>
        public static void LogTransaction(string message)
        {
            lock (_locker)
            {
                File.AppendAllText(_transactionLogFile, message + Environment.NewLine);
            }
        }

        /// <summary>
        /// Writes to the console window
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
