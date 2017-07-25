
using QuantTrade.Core.Algorithm;
using QuantTrade.Core.Utilities;
using QuantTrade.Core.Configuration;
using System;


namespace QuantTrade
{
    class Program
    {
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

                //Run benchmark test
                bool runBenchmark = Convert.ToBoolean(Config.GetToken("run-benchmark"));
            
                if (runBenchmark)
                {
                    string benchmarkSymbols = Config.GetToken("benchmark-symbols");
                    runAlgorithm(benchmarkSymbols, true);
                }

                //Run Standard tests
                string symbols = Config.GetToken("symbols");
                runAlgorithm(symbols, false);
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
        /// Run benchmark stocks for comparison
        /// </summary>
        /// <param name="alogoType"></param>
        private static void runAlgorithm(string symbols, bool buyAndHold)
        {
            if (string.IsNullOrEmpty(symbols)) return;
            
            //Get the default algo to run from the config.json file
            string defaultAlgo = $"QuantTrade.Core.Algorithm.{Config.GetToken("default-alogrithm")}";
            Type defaultAlgoType = Type.GetType($"{defaultAlgo}, QuantTrade.Core");

            ////////////////////////////////////////////////
            //Loops the stocks and run the alogo
            ////////////////////////////////////////////////
            string[] symbolsArray = symbols.Split(' ');

            foreach (var symbol in symbolsArray)
            {
                IAlogorithm algo = Activator.CreateInstance(defaultAlgoType) as IAlogorithm;
                algo.Initialize(symbol, buyAndHold);
            }

        }
    }
}
