/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: khannan
 * Date: 2017-6-15
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web;

namespace Admin.Models
{

    /// <summary>
    /// Represents a data set
    /// </summary>
    [DataContract()]
    public class BarDataSet
    {

        /// <summary>
        /// Ctor for bar data set
        /// </summary>
        public BarDataSet(String fillColor, String strokeColor, params int[] data)
        {
            this.data = new List<int>(data);
            this.fillColor = fillColor;
            this.strokeColor = this.pointColor = strokeColor;
            this.pointStrokeColor = "#fff";
        }

        /// <summary>
        /// Gets or sets the data for the plot
        /// </summary>
        [DataMember(Name = "data")]
        public List<Int32> data { get; set; }

        /// <summary>
        /// Stroke color
        /// </summary>
        public String pointStrokeColor { get; set; }
        /// <summary>
        /// Point color
        /// </summary>
        public String pointColor { get; set; }

        /// <summary>
        /// Fill color
        /// </summary>
        [DataMember(Name = "fillColor")]
        public String fillColor { get; set; }
        
        /// <summary>
        /// Stroke color
        /// </summary>
        [DataMember(Name = "strokeColor")]
        public String strokeColor { get; set; }

    }

    /// <summary>
    /// PIE dataset
    /// </summary>
    [DataContract]
    public class PieDataSet
    {

        /// <summary>
        /// Creates a new pie data set
        /// </summary>
        public PieDataSet(String color, int value)
        {
            this.value = value;
            this.color = color;
        }

        [DataMember(Name = "value")]
        public int value { get; set; }

        [DataMember(Name = "color")]
        public String color { get; set; }
    }

    /// <summary>
    /// Stats plot data
    /// </summary>
    [DataContract()]
    public class StatisticsChart
    {

        private static readonly string[] CHART_AUTO_COLORS = new string[]{
            "0C527F", "CBD2B0", "70A664", "5B4B43", "BE4D27", "85A38C", "D7C231", 
            "FFC6A7", "FF5400", "5E5A53", "FF6EDE", "96A2D4", "C85141", "FF0000", 
            "00FF00", "0000FF", "FFFF00", "FF00FF", "00FFFF", "000000", "800000", 
            "008000", "000080", "808000", "800080", "008080", "808080", "C00000", 
            "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0", "400000", 
            "004000", "000040", "404000", "400040", "004040", "404040", "200000", 
            "002000", "000020", "202000", "200020", "002020", "202020", "600000", 
            "006000", "000060", "606000", "600060", "006060", "606060", "A00000", 
            "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0", "E00000", 
            "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0", 
        };

        /// <summary>
        /// Stats chart
        /// </summary>
        public StatisticsChart()
        {
            this.legend = new Dictionary<string, string>();
        }

        /// <summary>
        /// Add a legend item
        /// </summary>
        public void AddLegendItem(String name, String color)
        {
            if (color == null)
            {
                var dnColor = System.Drawing.ColorTranslator.FromHtml(String.Format("#{0}", CHART_AUTO_COLORS[this.legend.Count % CHART_AUTO_COLORS.Length]));
                color = string.Format("rgba({0},{1},{2},0.5)", dnColor.R, dnColor.G, dnColor.B);
            }
            this.legend.Add(name, color);
        }

        /// <summary>
        /// Add a legend item
        /// </summary>
        public void AddLegendItems(params String[] names)
        {
            foreach (var name in names)
            {
                var color = System.Drawing.ColorTranslator.FromHtml(String.Format("#{0}", CHART_AUTO_COLORS[this.legend.Count % CHART_AUTO_COLORS.Length]));
                string colorHtml = string.Format("rgba({0},{1},{2},0.5)", color.R, color.G, color.B);
                this.legend.Add(name, colorHtml);
            }
        }

        /// <summary>
        /// Represents a legend of colors and labels
        /// </summary>
        [DataMember(Name = "legend")]
        public Dictionary<String, String> legend { get; set; }

        /// <summary>
        /// Plot data
        /// </summary>
        [DataMember(Name = "data")]
        public PlotData data { get; set; }
    }

    /// <summary>
    /// Plot data
    /// </summary>
    public abstract class PlotData
    {
        protected StatisticsChart m_chart;

        /// <summary>
        /// Chart context
        /// </summary>
        /// <param name="context"></param>
        public PlotData(StatisticsChart context)
        {
            this.m_chart = context;
        }
    }

    /// <summary>
    /// Data for Bar charts
    /// </summary>
    [DataContract()]
    public class BarPlotData : PlotData
    {

        // Create a color
        private String ChangeAlpha(String color, decimal newAlpha)
        {
            Regex re = new Regex(@"rgba\(([\d\.]*)?,([\d\.]*)?,([\d\.]*)?,([\d\.]*)?\)");
            var match = re.Match(color);
            if(match.Groups.Count >= 4)
            {
                return String.Format("rgba({0},{1},{2},{3:#.#})", match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, newAlpha);
            }
            return color;
        }


        /// <summary>
        /// Ctor for bar plot data
        /// </summary>
        public BarPlotData(StatisticsChart context) : base(context)
        {
            this.labels = new List<string>();
            this.datasets = new List<BarDataSet>();
        }

        /// <summary>
        /// Add a dataset
        /// </summary>
        public void AddDataset(String legendName, params int[] data)
        {
            string color = null;
            if (!this.m_chart.legend.TryGetValue(legendName, out color))
                throw new InvalidOperationException("Must link this dataset to a legend item");
            this.datasets.Add(new BarDataSet(color, this.ChangeAlpha(color, 1), data)); 
        }

        /// <summary>
        /// Labels for the plot
        /// </summary>
        [DataMember(Name = "labels")]
        public List<String> labels { get; set; }

        /// <summary>
        /// Datasets in the plot
        /// </summary>
        [DataMember(Name = "datasets")]
        public List<BarDataSet> datasets { get; set; }
    }

    /// <summary>
    /// Data for the stats
    /// </summary>
    public class PiePlotData : PlotData, IEnumerable<PieDataSet>
    {

        private List<PieDataSet> m_list = new List<PieDataSet>();

        /// <summary>
        /// Statistics chart
        /// </summary>
        public PiePlotData(StatisticsChart chart) : base(chart)
        {

        }

        /// <summary>
        /// Add data
        /// </summary>
        public void AddData(String legendName, int value)
        {
            string color = null;
            if (!this.m_chart.legend.TryGetValue(legendName, out color))
                throw new InvalidOperationException("Must link this dataset to a legend item");
            this.m_list.Add(new PieDataSet(color, value));
        }

        #region IEnumerable<PieDataSet> Members

        /// <summary>
        /// Get enumerator
        /// </summary>
        public IEnumerator<PieDataSet> GetEnumerator()
        {
            return this.m_list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Get enumerator
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.m_list.GetEnumerator();
        }

        #endregion
    }

}