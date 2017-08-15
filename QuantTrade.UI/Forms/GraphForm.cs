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

using QuantTrade.Core.Algorithm;
using QuantTrade.Core.Reports;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using QuantTrade.Core.Utilities;
using System.Drawing;

namespace QuantTrade.UI
{
   
    /// <summary>
    /// 
    /// </summary>
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
        }

     
        /// <summary>
        /// Add alog results to the the graph
        /// </summary>
        /// <param name="algorithmResults"></param>
        private void graphResults(IAlogorithm algorithmResults)
        {
            //Create a unique name
            string symbolName = algorithmResults.Symbol;

            if (Chart.Series.IsUniqueName(symbolName) == false)
            {
                symbolName = symbolName + "_" + DateTime.Now.Second.ToString();
            }
         
            //Create a new series and add to the chart
            Series series = new Series()
            {
                Name = symbolName,
                ChartType = SeriesChartType.Line
            };

            Chart.Series.Add(series);


            //Add equity over time results to the graph
            foreach (KeyValuePair<DateTime, decimal> equity in algorithmResults.Broker.EquityOverTime)
            {
                Chart.Series[symbolName].Points.AddXY(equity.Key, equity.Value);
            }

            //Add summary report to collection for the grid
            //Update the symbol name in the report - avoiding duplicates
            algorithmResults.SummaryReport.Symbol = symbolName;
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
            Grid.Columns["RunDates"].Visible = false;

            Grid.DefaultCellStyle.SelectionBackColor = Color.White;
            Grid.DefaultCellStyle.SelectionForeColor = Color.Black;
          
            //Grid.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Empty;
            Grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.AliceBlue;
            //Grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Silver;
            Grid.EnableHeadersVisualStyles = true;
        }

        /// <summary>
        /// For Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
            _algorithmSummaryReports = new List<SummaryReport>();
            getDefaultAlgorithm();

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
        /// Get default algo from config file
        /// </summary>
        private void getDefaultAlgorithm()
        {
            _defaultAlgoName = $"QuantTrade.Core.Algorithm.{Config.GetToken("default-alogrithm")}";
            _defaultAlgoType = Type.GetType($"{_defaultAlgoName}, QuantTrade.Core");

            //Update chart Title
            Chart.Titles.Clear();
            string newTitle = _defaultAlgoName.ToString().Replace("QuantTrade.Core.Algorithm.", "");
            Title title = Chart.Titles.Add(newTitle + " Results");
            title.Font = new System.Drawing.Font("Arial", 12, FontStyle.Bold);
        }


        /// <summary>
        /// Loops the stocks and run the default alogorithm
        /// </summary>
        private void runAlgorithm(Type defaultAlgoType, string symbols, bool buyAndHold)
        {
            if (string.IsNullOrEmpty(symbols)) return;

            //Loops the stocks and run the alogo
            string[] symbolsCollection = symbols.Split(' ');
      
            foreach (var symbol in symbolsCollection)
            {
                Application.DoEvents();
                IAlogorithm algo = Activator.CreateInstance(defaultAlgoType) as IAlogorithm;
                algo.Initialize(symbol, buyAndHold);
                graphResults(algo);
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


        #region Menu Bars

        /// <summary>
        /// Configure Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureForm frm = new ConfigureForm();
            frm.ShowDialog();

            //Refresh default algo
            getDefaultAlgorithm();
        }

        /// <summary>
        /// Run Algo click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadAlgorithmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Clear old results
            Chart.Series.Clear();
            _algorithmSummaryReports.Clear();
            Status.Text = "Running";

            //Run buy and hold stocks - used to benchmark 
            runAlgorithm(_defaultAlgoType, Config.GetToken("buyandhold-stocks"), true);

            //Run swing trade stocks
            runAlgorithm(_defaultAlgoType, Config.GetToken("swingtrade-stocks"), false);

            //Update the Grid
            Grid.DataSource = null;
            Grid.DataSource = _algorithmSummaryReports;
            formatGridColumns();

            Status.Text = "Idle";
        }

        #endregion
    }
}
