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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using QuantTrade.Core.Utilities;

namespace QuantTrade.Core.Data
{
    /// <summary>
    /// AlphaAdvantage downloader class. 
    /// </summary>
    public class AlphaAdvantage : IDataSource
    {
        #region Properties

        private string _csvFileName;
        private static readonly object _locker = new object();

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        public string GenerateData(string symbol, Resolution resolution)
        {

            string outputDirectory = Config.GetToken("data-directory");
            string apiKey = Config.GetToken("alpha-api-key");

            _csvFileName = outputDirectory + @"\" + resolution + "_" + symbol + ".csv";

            if (isDataNeeded())
            {
                //do not mutithread the down load!!!
                lock (_locker)
                {

                    //See https://www.alphavantage.co/documentation/ for docs!
                    string requestString = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={symbol}&apikey={apiKey}&datatype=csv&outputsize=full";

                    callWebsite(requestString, symbol);
                }
            }
            
            return _csvFileName;
        }

        /// <summary>
        /// The website downloads 20 years worth of data so there is not need to continuously download the data
        /// </summary>
        private bool isDataNeeded()
        {
            bool isNeeded = true;

            if (File.Exists(_csvFileName))
            {
                isNeeded = false;
            }

            return isNeeded;
        }


        /// <summary>
        /// Download CSV fomr website
        /// </summary>
        private void callWebsite(string requestString, string symbol)
        {
            Logger.Log($"Downloading {symbol} data......." + Environment.NewLine, ConsoleColor.Green);

            WebRequest request = WebRequest.Create(requestString);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string data = reader.ReadToEnd();
            reader.Close();
            response.Close();
            formatCSV(data);
        }

        /// <summary>
        /// Formats the CSV in memory.
        /// </summary>
        private void formatCSV(string data)
        {
            List<string> tempStringBuilder = new List<string>();
            int counter = 0;

            using (StringReader reader = new StringReader(data))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    //remove header
                    if (counter == 0)
                    {
                        counter++;
                        continue;
                    }
           
                    //Alpha Format: Date, Open, High, Low, Close, Volume
                    tempStringBuilder.Add(line);
                    counter++;
                }
            }

            //Now I have to reverse the data in the string builder
            tempStringBuilder.Reverse();

            StringBuilder dataToExport = new StringBuilder();
            foreach (var item in tempStringBuilder)
            {
                dataToExport.AppendLine(item);
            }

            writeCSV(dataToExport.ToString());
        }


        /// <summary>
        /// Write the CSV to disk
        /// </summary>
        private void writeCSV(string data)
        {
            //get rid of the old files
            File.Delete(_csvFileName);
      
            //Write the CSV & then zip it.
            File.WriteAllText(_csvFileName, data);
        }


     }
}
