
using QuantTrade.Core.Algorithm;
using QuantTrade.Core.Utilities;
using QuantTrade.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

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
                Logger.Log("Starting run..." + Environment.NewLine, ConsoleColor.Green);

                //TESTING TESTING TESTING
                //runCustomRSI2Alog("TQQQ");

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
                Logger.Log("Run complete. Hit any Enter to exit... ", ConsoleColor.Green);
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
            string[] symbolsCollection = symbols.Split(' ');

            //Start Timer
            DateTime startRun = DateTime.Now;

            //Non-parallel method for processing
            //foreach (var symbol in symbolsCollection)
            //{
            //    IAlogorithm algo = Activator.CreateInstance(_defaultAlgoType) as IAlogorithm;
            //    algo.Initialize(symbol, buyAndHold);

            //   // Console.WriteLine("{0}, Thread Id= {1}", symbol, Thread.CurrentThread.ManagedThreadId);
            //}

            //Parallel method for processing
            Parallel.ForEach(symbolsCollection, symbol =>
            {
                IAlogorithm algo = Activator.CreateInstance(_defaultAlgoType) as IAlogorithm;
                algo.Initialize(symbol, buyAndHold);

                // Console.WriteLine("{0}, Thread Id= {1}", symbol, Thread.CurrentThread.ManagedThreadId);
            });

            //End Timer
            DateTime endRun = DateTime.Now;
            decimal totalRunTime = (endRun - startRun).Milliseconds;
            Logger.Log($"Total Run Time: {totalRunTime.ToString()} ms", ConsoleColor.Yellow);
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
