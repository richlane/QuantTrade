using QuantTrade.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using QuantTrade.Core;


namespace QuantTrade.Data.Providers
{
    public class AlphaAdvantage : IGenerator
    {

        private string _csvFileName;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        public string GenerateData(string symbol, Resolution resolution)
        {
            string outputDirectory = Config.GetToken("data-directory");
            _csvFileName = outputDirectory + @"\" + resolution + "_" + symbol + ".csv";

            if (isDataNeeded())
            {
                //See https://www.alphavantage.co/documentation/ for docs!
                string requestString = @"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=" + symbol +
                       "&apikey=1CP91IVE5IWVQE4A&datatype=csv&outputsize=full";

                callWebsite(requestString);
            }

            return _csvFileName;
        }

        /// <summary>
        /// The website downloads 20 years worth of data so there is not need to continuously download the data
        /// </summary>
        /// <returns></returns>
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
        /// 
        /// </summary>
        private void callWebsite(string requestString)
        {
            string data = "";

            WebRequest request = WebRequest.Create(requestString);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            data = reader.ReadToEnd();
            reader.Close();
            response.Close();
            exportCSV(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void exportCSV(string data)
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

        private void writeCSV(string data)
        {
            //get rid of the old files
            File.Delete(_csvFileName);
      
            //Write the CSV & then zip it.
            File.WriteAllText(_csvFileName, data);
        }


        #region Formating Functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priceString"></param>
        /// <returns></returns>
        private string formatPrice(string priceString)
        {
            decimal p1 = Math.Round(Convert.ToDecimal(priceString), 2) * 10000M;
            p1 = Math.Round(p1, 0);
            string p2 = p1.ToString();

            return p2;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns></returns>
        private string formatDate(string dateString)
        {
            DateTime dt = DateTime.Parse(dateString);
            return dt.ToString("MM/dd/yyyy HH:mm");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns></returns>
        private string formatVolume(string volumeString)
        {
            int volume = Convert.ToInt32(volumeString) * 1000;
            return volume.ToString();
        }

        #endregion
    }
}
