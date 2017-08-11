/*
* BSD 2-Clause License 
* Copyright (c) 2017, Rich Lane 
* All rights reserved. 
* 
* Redistribution and use in source and binary forms, with or without 
* modification, are permitted provided that the following conditions are met: 
* 
* Redistributions of source code must retain the above copyright notice, this 
* list of conditions and the following disclaimer. 
* 
* Redistributions in binary form must reproduce the above copyright notice, 
* this list of conditions and the following disclaimer in the documentation 
* and/or other materials provided with the distribution. 
* 
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
* DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE 
* FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
* DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
* SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
* CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
* OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
* OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/

using QuantTrade.Core.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace QuantTrade.Core.Utilities
{
    /// <summary>
    /// Logging utilities class
    /// </summary>
    public static class Logger
    {
    
        #region Properties

        private static string _transactionLogLocation;
        private static bool _enableTransactionLogging;
        private static string _summaryReportLogFile;
        private static readonly object _locker = new object();

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        static Logger()
        {
            //get log file locations
            if(string.IsNullOrEmpty(_transactionLogLocation) || string.IsNullOrEmpty(_summaryReportLogFile))
            {
                //Results Log file
                _summaryReportLogFile = Config.GetToken("summary-report-log");
                if (File.Exists(_summaryReportLogFile))
                {
                    File.Delete(_summaryReportLogFile);
                }

                //Transaction log file
                _enableTransactionLogging = Convert.ToBoolean(Config.GetToken("enable-transaction-logging"));
                _transactionLogLocation = Config.GetToken("transaction-log-location");

                if(!_transactionLogLocation.EndsWith(@"\"))
                {
                    _transactionLogLocation = _transactionLogLocation + @"\";
                }

            }
        }

        /// <summary>
        /// Writes transaction to a log file.
        /// </summary>
        public static void LogTransactionsToFile(string transactions, string symbol)
        {
            if (_enableTransactionLogging == false) return;

            string fileName = $"{_transactionLogLocation}Trans_{symbol}.csv";

            lock (_locker)
            {
                //Clean old files
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                  
                File.AppendAllText(fileName, transactions + Environment.NewLine);
            }
        }

        /// <summary>
        /// Logs the summary report output to a file.
        /// </summary>
        private static void logSummaryReportToFile(string results)
        {
            lock (_locker)
            {
                File.AppendAllText(_summaryReportLogFile, results + Environment.NewLine);
            }
        }

        /// <summary>
        /// Retieves report property attributes
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static Dictionary<string, object> getPropertyAttributes(PropertyInfo property)
        {
            Dictionary<string, object> attribs = new Dictionary<string, object>();
            // look for attributes that takes one constructor argument
            foreach (CustomAttributeData attribData in property.GetCustomAttributesData())
            {

                if (attribData.ConstructorArguments.Count == 1)
                {
                    string typeName = attribData.Constructor.DeclaringType.Name;
                    if (typeName.EndsWith("Attribute")) typeName = typeName.Substring(0, typeName.Length - 9);
                    attribs[typeName] = attribData.ConstructorArguments[0].Value;
                }
                else if(attribData.NamedArguments.Count == 1)
                {
                    string typeName = attribData.Constructor.DeclaringType.Name;
                    if (typeName.EndsWith("Attribute")) typeName = typeName.Substring(0, typeName.Length - 9);
                    attribs[typeName] = attribData.NamedArguments[0].TypedValue.ToString();
                }

            }
            return attribs;
        }


        /// <summary>
        /// Logs summary report to console and flat file
        /// </summary>
        /// <param name="report"></param>
        public static void LogSummaryReport (SummaryReport report)
        {
            //Console.ForegroundColor = ConsoleColor.White;
            StringBuilder sb = new StringBuilder();
            
            //Loop class properties
            foreach (PropertyInfo pi in report.GetType().GetProperties())
            {
                //get custom attributes
                var attributes = getPropertyAttributes(pi);

                //Get deiplay name and value
                string displayName = attributes["DisplayName"].ToString();
                var displayValue = pi.GetValue(report, null);
                
                //Get the display format, if availiable, and apply it to the value
                object obj; 
                attributes.TryGetValue("DisplayFormat", out obj);
                if (obj != null)
                {
                    string displayFormat = obj.ToString();
                    displayValue = string.Format(displayFormat, displayValue).Trim('"').Replace(" ", "") ;
                }
                
                if(displayValue != null) sb.AppendLine(displayName + ": " + displayValue.ToString());
            }
            
            sb.AppendLine("--------------------------------");
            sb.AppendLine("");

            //Log Report
            logSummaryReportToFile(sb.ToString());

            //Display report to console
           // Console.Write(sb.ToString());

        }


        /// <summary>
        /// Writes to the console window.
        /// </summary>
        public static void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            //Console.ForegroundColor = color;

            //Console.WriteLine(message);

            //Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
