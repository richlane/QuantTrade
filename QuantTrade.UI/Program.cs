using QuantTrade.Core.Algorithm;
using QuantTrade.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuantTrade.UI
{
    static class Program
    {
        private static string _defaultAlgo;
        private static Type _defaultAlgoType;
        private static GraphForm _graph;


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
         
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GraphForm());


            //_graph = new GraphForm();
        
            //try
            //{
            //    //Get the default algo to run from the config.json file
            //    _defaultAlgo = $"QuantTrade.Core.Algorithm.{Config.GetToken("default-alogrithm")}";
            //    _defaultAlgoType = Type.GetType($"{_defaultAlgo}, QuantTrade.Core");

            //    //Run buy and hold stocks - used to benchmark 
            //    runAlgorithm(Config.GetToken("buyandhold-stocks"), true);

            //    //Run swing trade stocks
            //    runAlgorithm(Config.GetToken("swingtrade-stocks"), false);
          
            //    _graph.ShowDialog();
            //}
            //catch (Exception ex)
            //{
            //    Logger.Log(ex.Message);
            //    Logger.Log(ex.StackTrace.ToString());
            //}
        }

        /// <summary>
        /// Loops the stocks and run the default alogorithm
        /// </summary>
        private static void runAlgorithm(string symbols, bool buyAndHold)
        {
            //if (string.IsNullOrEmpty(symbols)) return;

            ////Loops the stocks and run the alogo
            //string[] symbolsCollection = symbols.Split(' ');

            ////Parallel method for processing
            //Parallel.ForEach(symbolsCollection, symbol =>
            //{
            //    IAlogorithm algo = Activator.CreateInstance(_defaultAlgoType) as IAlogorithm;
            //    algo.Initialize(symbol, buyAndHold);

            //    _graph.GraphResults(algo);
            //    // Console.WriteLine("{0}, Thread Id= {1}", symbol, Thread.CurrentThread.ManagedThreadId);
            //});

        }


        /// <summary>
        /// Here I am trying to have to software set a slew of parameters and tell me what generates the best results.
        /// </summary>
        private static void runCustomRSI2Alogo(string symbol)
        {
            for (int i = 1; i < 100; i++)
            {
                RSI2Algorithm algo = new RSI2Algorithm();
                algo._smaLookBackPeriod = i;

                algo.Initialize(symbol, false, "i = " + i);
            }
        }

    }
}
