using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FinancialDataProcesor.Pages
{
    public partial class Chart : System.Web.UI.Page
    {
        string chart_form_id = "chart_form";
        string chart_title_parameter_name = "ChartTitle";
        string chart_units_parameter_name = "ChartUnits";
        string chart_data_parameter_name = "ChartJsonData";
        string chart_scale_parameter_name = "ChartTimeScale";

        enum ChartTimeScale
        {
            NotImplemented = 0,
            Hour,
            Day,
            Week,
            Month,
            Quarter,
            Year
        }

        enum ColorType
        {
            NotImplemented = 0,
            Gray,
            Colorful
        }

        class DataPiece
        {
            public DateTime Date;
            public double Value;

            public DataPiece(DateTime date, double data)
            {
                Date = date;
                Value = data;
            }
        }

        class DataSet
        {
            public string Label;
            private List<DataPiece> DataCollection;
            public string BackgroundColor;
            public string BorderColor;

            public DataSet(string label, List<DataPiece> dataCollection = null, ColorType colorType = ColorType.Colorful, double opatique = 0.0)
            {
                Label = label;
                DataCollection = dataCollection ?? new List<DataPiece>();
                BackgroundColor = GetColorString(0.8 - opatique, label, colorType);
                BorderColor = GetColorString(0.9 - opatique, label, colorType);
            }

            public void AddDataPiece(DateTime date, double value)
            {
                DataPiece dataPiece = DataCollection.Find(x => x.Date == date);
                if (dataPiece == null)
                {
                    dataPiece = new DataPiece(date, value);
                    DataCollection.Add(dataPiece);
                }
                else
                {
                    dataPiece.Value += value;
                }
            }

            public double[] GetValueArray(DateTime[] datesOrder)
            {
                double[] result = new double[datesOrder.Length];
                for (int i = 0; i < datesOrder.Length; i++)
                {
                    DataPiece currentDataPiece = DataCollection.Find(x =>
                        x.Date >= datesOrder[i] &&
                        x.Date < (i < datesOrder.Length - 1 ? datesOrder[i + 1] : i > 0 ? datesOrder[i].Add(datesOrder[i] - datesOrder[i - 1]) : DateTime.MaxValue));
                    result[i] = currentDataPiece != null ? currentDataPiece.Value : 0.0;
                }
                return result;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.Form[chart_data_parameter_name]))
            {
                string chartTitle = !string.IsNullOrWhiteSpace(Request.Form[chart_title_parameter_name]) ? Request.Form[chart_title_parameter_name] : "Chart";
                string chartUnits = Request.Form[chart_units_parameter_name] ?? "[-]";
                string chartData = Request.Form[chart_data_parameter_name];
                ChartTimeScale chartTimeScale = ChartTimeScale.NotImplemented;
                Enum.TryParse(Request.Form[chart_scale_parameter_name] ?? "Month", true, out chartTimeScale);

                chart_data.Value = PrepareChartData(chartTitle, chartUnits, chartData, chartTimeScale);
                Title = chartTitle;
                chart_label_scale.Text = chartTimeScale.ToString();
                DisableButtons(chartTimeScale);

                ViewState["chartTitle"] = chartTitle;
                ViewState["chartUnits"] = chartUnits;
                ViewState["chartData"] = chartData;
                ViewState["chartTimeScale"] = chartTimeScale;
            }
        }

        private void DisableButtons(ChartTimeScale currentTimeScale)
        {
            switch (currentTimeScale)
            {
                case ChartTimeScale.Hour:
                    chart_scalebuttonR.Enabled = false;
                    chart_scalebuttonL.Enabled = true;
                    break;
                case ChartTimeScale.Year:
                    chart_scalebuttonR.Enabled = true;
                    chart_scalebuttonL.Enabled = false;
                    break;
                default:
                    chart_scalebuttonR.Enabled = true;
                    chart_scalebuttonL.Enabled = true;
                    break;
            }
        }

        private static string PrepareChartData(string chartTitle, string yUnits, string rawData, ChartTimeScale chartTimeScale = ChartTimeScale.Month)
        {
            JToken Data = JToken.Parse(rawData);

            double maxAbsValue;
            DateTime[] roundedDates;
            List<DataSet> dataSets;

            if (!ParseData(chartTimeScale, Data, out maxAbsValue, out roundedDates, out dataSets))
            {
                return string.Empty;
            }

            JObject chart = GetConstantChartSchema(chartTitle, yUnits);
            ConvertDataToJData(roundedDates, dataSets, chart, chartTimeScale);

            return chart.ToString();
        }

        private static void ConvertDataToJData(DateTime[] roundedDates, List<DataSet> dataSets, JObject chart, ChartTimeScale chartTimeScale)
        {
            JObject jdata = new JObject();
            int count = roundedDates.Length;
            JArray jlabels = GetLabels(roundedDates, chartTimeScale);
            jdata.Add("labels", jlabels);
            JArray jDataSets = new JArray();
            foreach (DataSet dataSet in dataSets)
            {
                JObject jDataSet = new JObject();
                jDataSet.Add("label", dataSet.Label);
                jDataSet.Add("data", new JArray(dataSet.GetValueArray(roundedDates)));
                JArray bckColor = new JArray(GetFilledArray(dataSet.BackgroundColor, count));
                jDataSet.Add("backgroundColor", bckColor);
                JArray brdColor = new JArray(GetFilledArray(dataSet.BorderColor, count));
                jDataSet.Add("borderColor", brdColor);
                jDataSet.Add("borderWidth", "1");

                jDataSets.Add(jDataSet);
            }
            jdata.Add("datasets", jDataSets);
            chart.Add("data", jdata);
        }

        private static JArray GetLabels(DateTime[] roundedDates, ChartTimeScale chartTimeScale)
        {
            string dateFormat;
            switch (chartTimeScale)
            {
                case ChartTimeScale.Hour:
                    dateFormat = "yyyy-MM-dd HH:mm";
                    break;
                case ChartTimeScale.Day:
                    dateFormat = "yyyy-MM-dd";
                    break;
                case ChartTimeScale.Week:
                    dateFormat = "yyyy-MM-dd";
                    break;
                case ChartTimeScale.Month:
                    dateFormat = "yyyy-MM";
                    break;
                case ChartTimeScale.Quarter:
                    dateFormat = "yyyy-MM";
                    break;
                case ChartTimeScale.Year:
                    dateFormat = "yyyy";
                    break;
                default:
                    dateFormat = "yyyy-MM-dd HH:mm";
                    break;
            }
            JArray jlabels = new JArray();
            foreach (DateTime date in roundedDates)
            {
                jlabels.Add(date.ToString(dateFormat));
            }

            return jlabels;
        }

        private static T[] GetFilledArray<T>(T obj, int count)
        {
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = obj;
            }
            return result;
        }

        private static bool ParseData(ChartTimeScale chartTimeScale, JToken Data, out double maxAbsValue, out DateTime[] roundedDates, out List<DataSet> dataSets)
        {
            maxAbsValue = -1.0;
            DateTime maxDateTime = DateTime.MinValue;
            DateTime minDateTime = DateTime.MaxValue;
            dataSets = new List<DataSet>();
            List<DateTime> tmproundedDates = new List<DateTime>();

            foreach (JToken datapiece in Data.SelectTokens("data[*]"))
            {
                string label = datapiece["label"].ToString();
                DateTime date = DateTime.Parse(datapiece["date"].ToString());
                double val = double.Parse(datapiece["value"].ToString());

                DataSet currentDataSet = dataSets.Find(x => x.Label == label);
                if (currentDataSet == null)
                {
                    currentDataSet = new DataSet(label);
                    dataSets.Add(currentDataSet);
                }

                DateTime currentDate = RoundDateTime(date, chartTimeScale);
                currentDataSet.AddDataPiece(currentDate, val);
                if (currentDate > maxDateTime)
                    maxDateTime = currentDate;
                if (currentDate < minDateTime)
                    minDateTime = currentDate;

                double absval = Math.Abs(val);
                if (absval > maxAbsValue)
                {
                    maxAbsValue = absval;
                }
            }
            if (maxAbsValue <= 0)
            {
                roundedDates = new DateTime[0];
                return false;
            }
            roundedDates = GenerateDatesArray(minDateTime, maxDateTime, chartTimeScale);

            return true;
        }

        private static DateTime[] GenerateDatesArray(DateTime minDateTime, DateTime maxDateTime, ChartTimeScale chartTimeScale)
        {
            List<DateTime> result = new List<DateTime>();
            DateTime currentDateTime = minDateTime;
            while (currentDateTime <= maxDateTime)
            {
                result.Add(currentDateTime);
                switch (chartTimeScale)
                {
                    case ChartTimeScale.Hour:
                        currentDateTime = currentDateTime.AddHours(1.0);
                        break;
                    case ChartTimeScale.Day:
                        currentDateTime = currentDateTime.AddDays(1.0);
                        break;
                    case ChartTimeScale.Week:
                        currentDateTime = currentDateTime.AddDays(7.0);
                        break;
                    case ChartTimeScale.Month:
                        currentDateTime = currentDateTime.AddMonths(1);
                        break;
                    case ChartTimeScale.Quarter:
                        currentDateTime = currentDateTime.AddMonths(3);
                        break;
                    case ChartTimeScale.Year:
                        currentDateTime = currentDateTime.AddYears(1);
                        break;
                    default:
                        currentDateTime = currentDateTime.AddMonths(1);
                        break;
                }
            }
            return result.ToArray();
        }

        private static JObject GetConstantChartSchema(string chartTitle, string yUnits)
        {
            JObject chart = new JObject();
            chart.Add("type", "bar");
            JObject options = new JObject();
            options.Add("responsive", true);
            options.Add("maintainAspectRatio", true);
            options.Add("responsiveAnimationDuration", "1000");
            JObject legend = new JObject();
            legend.Add("display", true);
            legend.Add("position", "right");
            JObject legendLabels = new JObject();
            legendLabels.Add("fontSize", 12);
            legendLabels.Add("fontFamily", "Verdana,Arial,Helvetica");
            legendLabels.Add("fontColor", "rgb(0,0,60)");
            legend.Add("labels", legendLabels);
            options.Add("legend", legend);
            JObject title = new JObject();
            title.Add("display", true);
            title.Add("position", "top");
            title.Add("text", chartTitle);
            title.Add("fontSize", 18);
            title.Add("fontFamily", "Verdana,Arial,Helvetica");
            title.Add("fontColor", "rgb(0,0,60)");
            options.Add("title", title);
            JObject scales = new JObject();
            JArray xAxes = new JArray();
            JObject stacked = new JObject();
            stacked.Add("stacked", true);
            xAxes.Add(stacked);
            scales.Add("xAxes", xAxes);
            JArray yAxes = new JArray();
            JObject scaleLabel = new JObject();
            JObject sLabel = new JObject();
            sLabel.Add("display", true);
            sLabel.Add("labelString", yUnits);
            sLabel.Add("fontSize", 16);
            sLabel.Add("fontFamily", "Verdana,Arial,Helvetica");
            sLabel.Add("fontColor", "rgb(0,0,60)");
            scaleLabel.Add("scaleLabel", sLabel);
            scaleLabel.Add("stacked", true);
            yAxes.Add(scaleLabel);
            scales.Add("yAxes", yAxes);
            options.Add("scales", scales);
            chart.Add("options", options);
            return chart;
        }

        private static DateTime RoundDateTime(DateTime date, ChartTimeScale chartTimeScale)
        {
            int diffrence;
            switch (chartTimeScale)
            {
                case ChartTimeScale.Hour:
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
                case ChartTimeScale.Day:
                    return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                case ChartTimeScale.Week:
                    diffrence = (int)date.DayOfWeek + 1;
                    if (diffrence == 1)
                        diffrence = 7;
                    DateTime dateTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                    dateTime.AddDays(-diffrence);
                    return dateTime;
                case ChartTimeScale.Month:
                    return new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                case ChartTimeScale.Quarter:
                    diffrence = (date.Month - 1) % 3;
                    return new DateTime(date.Year, date.Month - diffrence, 1, 0, 0, 0);
                case ChartTimeScale.Year:
                    return new DateTime(date.Year, 1, 1, 0, 0, 0);
                default:
                    return date;
            }
        }

        private static string GetColorString(double opatiqueLevel, string seed, ColorType colorType = ColorType.Colorful, double grayTreshold = 100.0 / 255.0)
        {
            const double maxColorValue = 255;
            double[] color = new double[4];
            Random randomGenerator = new Random(seed.GetHashCode());
            for (int i = 0; i < 3; i++)
            {
                color[i] = randomGenerator.NextDouble();
            }
            color[3] = opatiqueLevel;

            switch (colorType)
            {
                case ColorType.Gray:
                    {
                        color = MakeColorGray(color, grayTreshold);
                        break;
                    }
                default:
                    {
                        color = MakeColorColorful(color, grayTreshold);
                        break;
                    }
            }
            return "rgba(" + Convert.ToInt32(maxColorValue * color[0]).ToString()
                + ", " + Convert.ToInt32(maxColorValue * color[1]).ToString()
                + ", " + Convert.ToInt32(maxColorValue * color[2]).ToString()
                + ", " + color[3].ToString("F2").Replace(',', '.')
                + ")";
        }

        private static double[] MakeColorColorful(double[] color, double grayTreshold, double factor = 0.75)
        {
            double mean = (color[0] + color[1] + color[2]) / 3;
            double[] result = new double[4];
            for (int i = 0; i < 3; i++)
            {
                if (color[i] - mean > 0)
                {
                    result[i] = factor * (color[i] + (grayTreshold / 2));
                }
                else if (color[i] - mean < 0)
                {
                    result[i] = factor * (color[i] - (grayTreshold / 2));
                }
                result[i] = EnsureZeroOneValue(result[i]);
            }
            result[3] = EnsureZeroOneValue(color[3]);
            return result;
        }

        private static double[] MakeColorGray(double[] color, double grayTreshold)
        {
            double mean = (color[0] + color[1] + color[2]) / 3;
            double[] result = new double[4];
            for (int i = 0; i < 3; i++)
            {
                if (color[i] - mean > grayTreshold || mean - color[i] > grayTreshold)
                {
                    result[i] = mean;
                }
                result[i] = EnsureZeroOneValue(result[i]);
            }
            result[3] = EnsureZeroOneValue(color[3]);
            return result;
        }

        private static double EnsureZeroOneValue(double value)
        {
            if (value < 0.0)
            {
                return 0.0;
            }
            else if (value > 1.0)
            {
                return 1.0;
            }
            else
            {
                return value;
            }
        }

        protected void chart_scalebuttonR_Click(object sender, EventArgs e)
        {
            ChartTimeScale currentTimeScale = (ChartTimeScale)ViewState["chartTimeScale"];
            ChartTimeScale newTimeScale;
            switch (currentTimeScale)
            {
                case ChartTimeScale.Hour:
                    newTimeScale = ChartTimeScale.NotImplemented;
                    break;
                case ChartTimeScale.Day:
                    newTimeScale = ChartTimeScale.Hour;
                    break;
                case ChartTimeScale.Week:
                    newTimeScale = ChartTimeScale.Day;
                    break;
                case ChartTimeScale.Month:
                    newTimeScale = ChartTimeScale.Week;
                    break;
                case ChartTimeScale.Quarter:
                    newTimeScale = ChartTimeScale.Month;
                    break;
                case ChartTimeScale.Year:
                    newTimeScale = ChartTimeScale.Quarter;
                    break;
                default:
                    newTimeScale = ChartTimeScale.Month;
                    break;
            }

            ViewState["chartTimeScale"] = newTimeScale;
            chart_label_scale.Text = newTimeScale.ToString();
            chart_data.Value = PrepareChartData(ViewState["chartTitle"].ToString(), ViewState["chartUnits"].ToString(), ViewState["chartData"].ToString(), newTimeScale);
            DisableButtons(newTimeScale);
        }

        protected void chart_scalebuttonL_Click(object sender, EventArgs e)
        {
            ChartTimeScale currentTimeScale = (ChartTimeScale)ViewState["chartTimeScale"];
            ChartTimeScale newTimeScale;
            switch (currentTimeScale)
            {
                case ChartTimeScale.Hour:
                    newTimeScale = ChartTimeScale.Day;
                    break;
                case ChartTimeScale.Day:
                    newTimeScale = ChartTimeScale.Week;
                    break;
                case ChartTimeScale.Week:
                    newTimeScale = ChartTimeScale.Month;
                    break;
                case ChartTimeScale.Month:
                    newTimeScale = ChartTimeScale.Quarter;
                    break;
                case ChartTimeScale.Quarter:
                    newTimeScale = ChartTimeScale.Year;
                    break;
                case ChartTimeScale.Year:
                    newTimeScale = ChartTimeScale.NotImplemented;
                    break;
                default:
                    newTimeScale = ChartTimeScale.Month;
                    break;
            }

            ViewState["chartTimeScale"] = newTimeScale;
            chart_label_scale.Text = newTimeScale.ToString();
            chart_data.Value = PrepareChartData(ViewState["chartTitle"].ToString(), ViewState["chartUnits"].ToString(), ViewState["chartData"].ToString(), newTimeScale);
            DisableButtons(newTimeScale);
        }
    }
}