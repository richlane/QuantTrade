using QuantTrade.Core.Algorithm;
using QuantTrade.Core.Reports;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.ComponentModel;

namespace QuantTrade.UI
{

    public partial class GraphForm : Form
    {
        private List<SummaryReport> _reports;

        public GraphForm()
        {
            InitializeComponent();

            _reports = new List<SummaryReport>();
        }

        
        public void GraphResults(IAlogorithm algo)
        {
            Series s = new Series()
            {
                Name = algo.Symbol,
                ChartType = SeriesChartType.Line,
            };

            Chart.Series.Add(s);

            foreach (KeyValuePair<DateTime, decimal> equity in algo.Broker.EquityOverTime)
                Chart.Series[algo.Symbol].Points.AddXY(equity.Key, String.Format("{0:C}", equity.Value.ToString()));

            _reports.Add(algo.SummaryReport);
            Grid.Refresh();
        }

      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
            Chart.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineWidth = 0;
            Chart.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineWidth = 1;
            Chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            Chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
             Chart.ChartAreas[0].CursorX.IsUserEnabled = true;
             Chart.ChartAreas[0].CursorY.IsUserEnabled = true;
            Chart.ChartAreas[0].CursorX.AutoScroll = true;
            Chart.ChartAreas[0].CursorY.AutoScroll = true;

            Grid.DataSource = _reports;
        
            //Format columns
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


        

        //private void Chart_SelectionRangeChanged(object sender, CursorEventArgs e)
        //{
        //    double startX, endX, startY, endY;

        //    if (Chart.ChartAreas[0].CursorX.SelectionStart > Chart.ChartAreas[0].CursorX.SelectionEnd)
        //    {
        //        startX = Chart.ChartAreas[0].CursorX.SelectionEnd;
        //        endX = Chart.ChartAreas[0].CursorX.SelectionStart;
        //    }
        //    else
        //    {
        //        startX = Chart.ChartAreas[0].CursorX.SelectionStart;
        //        endX = Chart.ChartAreas[0].CursorX.SelectionEnd;
        //    }
        //    if (Chart.ChartAreas[0].CursorY.SelectionStart > Chart.ChartAreas[0].CursorY.SelectionEnd)
        //    {
        //        endY = Chart.ChartAreas[0].CursorY.SelectionStart;
        //        startY = Chart.ChartAreas[0].CursorY.SelectionEnd;
        //    }
        //    else
        //    {
        //        startY = Chart.ChartAreas[0].CursorY.SelectionStart;
        //        endY = Chart.ChartAreas[0].CursorY.SelectionEnd;
        //    }

        //    if (startX == endX && startY == endY)
        //    {
        //        return;
        //    }

        //    Chart.ChartAreas[0].AxisX.ScaleView.Zoom(startX, (endX - startX), DateTimeIntervalType.Auto, true);
        //    Chart.ChartAreas[0].AxisY.ScaleView.Zoom(startY, (endY - startY), DateTimeIntervalType.Auto, true);
        //}
    }
}
