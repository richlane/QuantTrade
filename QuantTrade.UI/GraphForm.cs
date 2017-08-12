using QuantTrade.Core.Algorithm;
using QuantTrade.Core.Reports;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using QuantTrade.Core.Utilities;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace QuantTrade.UI
{
   
    public partial class GraphForm : Form
    {
        #region Private Members

        private List<SummaryReport> _algorithmSummaryReports;
        private string _defaultAlgoName;
        private Type _defaultAlgoType;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GraphForm()
        {
            InitializeComponent();

            _algorithmSummaryReports = new List<SummaryReport>();
        }

        
        /// <summary>
        /// Add alog results to the the graph
        /// </summary>
        /// <param name="algorithmResults"></param>
        private void graphResults(IAlogorithm algorithmResults)
        {
            Series series = new Series()
            {
                Name = algorithmResults.Symbol,
                ChartType = SeriesChartType.Line,
            };

            Chart.Series.Add(series);

            foreach (KeyValuePair<DateTime, decimal> equity in algorithmResults.Broker.EquityOverTime)
            {
                Chart.Series[algorithmResults.Symbol].Points.AddXY(equity.Key, equity.Value);
            }

            //Add summary report to collection for the grid
            _algorithmSummaryReports.Add(algorithmResults.SummaryReport);
       
        }

        /// <summary>
        /// Format the datagrid cell columns
        /// </summary>
        private void formatGridColumns()
        {
            Grid.Columns["StartingAccount"].DefaultCellStyle.Format = "c0";
            Grid.Columns["EndingAccount"].DefaultCellStyle.Format = "c0";
            Grid.Columns["NetProfit"].DefaultCellStyle.Format = "p0";
            Grid.Columns["AnnualReturn"].DefaultCellStyle.Format = "p2";
            Grid.Columns["WinRate"].DefaultCellStyle.Format = "p0";
            Grid.Columns["LossRate"].DefaultCellStyle.Format = "p0";
            Grid.Columns["MaxDrawDown"].DefaultCellStyle.Format = "p0";
            Grid.Columns["TotalFees"].DefaultCellStyle.Format = "c0";
            Grid.Columns["Comments"].Visible = false;

            Grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.AliceBlue;
            Grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Silver;
            Grid.EnableHeadersVisualStyles = false;
        }

        /// <summary>
        /// For Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
            //Format chart
            Chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            Chart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;
            Chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            Chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            Chart.ChartAreas[0].CursorX.IsUserEnabled = true;
            Chart.ChartAreas[0].CursorY.IsUserEnabled = true;
            Chart.ChartAreas[0].CursorX.AutoScroll = true;
            Chart.ChartAreas[0].CursorY.AutoScroll = true;
            Chart.ChartAreas[0].AxisY.LabelStyle.Format = "C";

            //Bind Grid 
            Grid.DataSource = _algorithmSummaryReports;
            formatGridColumns();
        }


        /// <summary>
        /// Run the Algo click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadAlgorithmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Clear old results
            Chart.Series.Clear();
            _algorithmSummaryReports.Clear();

            _defaultAlgoName = $"QuantTrade.Core.Algorithm.{Config.GetToken("default-alogrithm")}";
            _defaultAlgoType = Type.GetType($"{_defaultAlgoName}, QuantTrade.Core");

            //Run buy and hold stocks - used to benchmark 
            runAlgorithm(Config.GetToken("buyandhold-stocks"), true);

            //Run swing trade stocks
            runAlgorithm(Config.GetToken("swingtrade-stocks"), false);

            //Update the Grid
            Grid.DataSource = null; // _summaryReports;
            Grid.DataSource = _algorithmSummaryReports;
            formatGridColumns();
        }


        /// <summary>
        /// Loops the stocks and run the default alogorithm
        /// </summary>
        private void runAlgorithm(string symbols, bool buyAndHold)
        {
            if (string.IsNullOrEmpty(symbols)) return;

            //Loops the stocks and run the alogo
            string[] symbolsCollection = symbols.Split(' ');

            foreach (var symbol in symbolsCollection)
            {
                IAlogorithm algo = Activator.CreateInstance(_defaultAlgoType) as IAlogorithm;
                algo.Initialize(symbol, buyAndHold);
                graphResults(algo);

                Application.DoEvents();
            }

            #region Parallel method for processing
            //Parallel.ForEach(symbolsCollection, symbol =>
            //{
            //    IAlogorithm algo = Activator.CreateInstance(_defaultAlgoType) as IAlogorithm;
            //    algo.Initialize(symbol, buyAndHold);

            //    graphResults(algo);
            //    // Console.WriteLine("{0}, Thread Id= {1}", symbol, Thread.CurrentThread.ManagedThreadId);
            //});
            #endregion
        }

    }
}
