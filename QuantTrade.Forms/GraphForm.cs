using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace QuantTrade.Forms
{


    //public class Equity
    //{
    //    public string Day { get; set; }
    //    public double Price { get; set; }

    //    public Equity(string day, double price)
    //    {
    //        Day = day;
    //        Price = price;
    //    }

    //}


    public partial class GraphForm : Form
    {
        public GraphForm()
        {
            InitializeComponent();
        }

        public void GraphResults(string symbol, SortedDictionary<DateTime, decimal> equityOverTime)
        {
            //List<Equity> myList = new List<Equity>();

            //foreach (KeyValuePair<DateTime, decimal> item in equityOverTime)
            //{
            //    myList.Add(new Equity(item.Key.ToString(), Convert.ToDouble(item.Value)));

            //}


            //Series s = new Series()
            //{
            //    Name = symbol,
            //    ChartType = SeriesChartType.Line,
            //    XValueMember = "Day",
            //    XValueType = ChartValueType.String,
            //    YValueMembers = "Price",
            //    YValueType = ChartValueType.Double,

            //};

            //Chart.Series.Add(s);
            //Chart.DataSource = myList;
            //Chart.DataBind();



            Series s = new Series()
            {
                Name = symbol,
                ChartType = SeriesChartType.Line,
            };

            Chart.Series.Add(s);

            foreach (KeyValuePair<DateTime, decimal> equity in equityOverTime)
                Chart.Series[symbol].Points.AddXY(equity.Key, equity.Value);
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


            //List<Equity> l = new List<Equity>();
            //l.Add(new Equity("11/1", 20.12));
            //l.Add(new Equity("11/2", 23.19));


            //Series s1 = new Series()
            //{
            //    Name = "X",
            //    ChartType = SeriesChartType.Line,
            //    XValueMember = "Day",
            //    XValueType = ChartValueType.String,
            //    YValueMembers = "Price",
            //    YValueType = ChartValueType.Double,

            //};

            //Chart.Series.Add(s1);
            //Chart.DataSource = l;
            //Chart.DataBind();


        }
    }
}
