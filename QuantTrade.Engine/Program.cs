
using QuantTrade.Core.Algorithm;
using QuantTrade.Core.Utilities;
using QuantTrade.Core.Configuration;
using System;


namespace QuantTrade
{
    class Program
    {
        private static string _defaultAlgo;
        private static Type _defaultAlgoType;  

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                Console.WindowHeight = Console.LargestWindowHeight;
                Console.WriteLine("Starting application" + Environment.NewLine);

                //Get the default algo to run from the config.json file
                _defaultAlgo = $"QuantTrade.Core.Algorithm.{Config.GetToken("default-alogrithm")}";
                _defaultAlgoType = Type.GetType($"{_defaultAlgo}, QuantTrade.Core");

                //Run buy and hold stocks - used to benchmark 
                runAlgorithm(Config.GetToken("buyandhold-stocks"), true);
           
                //Run swing trade stocks
                runAlgorithm(Config.GetToken("swingtrade-stocks"), false);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                Logger.Log(ex.StackTrace.ToString());
            }
            finally
            {
                Console.WriteLine("Hit any Enter to exit");
                Console.ReadLine();
            }
           
        }

        /// <summary>
        /// Loops the stocks and run the alogo
        /// </summary>
        private static void runAlgorithm(string symbols, bool buyAndHold)
        {
            if (string.IsNullOrEmpty(symbols)) return;
            
            //Loops the stocks and run the alogo
            string[] symbolsArray = symbols.Split(' ');

            foreach (var symbol in symbolsArray)
            {
                IAlogorithm algo = Activator.CreateInstance(_defaultAlgoType) as IAlogorithm;
                algo.Initialize(symbol, buyAndHold);
            }

        }
    }
}
