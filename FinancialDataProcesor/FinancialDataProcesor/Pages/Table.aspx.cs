using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace FinancialDataProcesor.Pages
{
    public partial class Table : System.Web.UI.Page
    {
        string[] colNames = { "kategoria", "dataOperacji", "dataKsiegowania", "opisOperacji", "kwota", "waluta", "saldo" };
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Table_FileUpload.HasFiles)
            {
                DataTable data = new DataTable();
                IList<HttpPostedFile> files = Table_FileUpload.PostedFiles;
                foreach (HttpPostedFile file in files)
                {
                    string fileName = file.FileName.Split('\\').Last();
                    string category = fileName.Substring(31, fileName.Length - 31 - 4);
                    ReadXmlFile(file.InputStream, category, ref data);
                }
                PrepareChartData(data);
                Table_GridView.DataSource = data;
            }
            Table_GridView.DataBind();

            
        }
        private void PrepareChartData(DataTable data)
        {
            string json = "{data:[";
            foreach (DataRow row in data.Rows)
            {
                json = json + "{label:\"" + row[colNames[0]].ToString() + "\",";
                json = json + "date:\"" + row[colNames[1]].ToString() + "\",";
                json = json + "value:" + row[colNames[4]].ToString().Replace(',','.') + "},";
            }
            if(json[json.Length-1] == ',')
                json = json.Substring(0, json.Length - 1) + "";
            json = json + "]}";
            Table_ChartData.Value = json;
        }
        private void ReadXmlFile(Stream fileStream, string category, ref DataTable result)
        {

            fileStream.Seek(0L, SeekOrigin.Begin);
            XmlReader reader = XmlReader.Create(fileStream);
            foreach (string colName in colNames)
            {
                if (!result.Columns.Contains(colName))
                {
                    result.Columns.Add(colName);
                }
            }

            string dataOperacjiS = "";
            string dataKsiegowaniaS = "";
            string opisOperacjiS = "";
            string kwotaS = "";
            string walutaS = "";
            string saldoS = "";

            bool moved = false;
            while (moved || reader.Read())
            {
                moved = false;
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "dataOperacji":
                            dataOperacjiS = reader.ReadElementContentAsString();
                            moved = true;
                            break;
                        case "dataKsiegowania":
                            dataKsiegowaniaS = reader.ReadElementContentAsString();
                            moved = true;
                            break;
                        case "opisOperacji":
                            opisOperacjiS = reader.ReadElementContentAsString();
                            moved = true;
                            break;
                        case "kwota":
                            kwotaS = reader.ReadElementContentAsString();
                            moved = true;
                            break;
                        case "waluta":
                            walutaS = reader.ReadElementContentAsString();
                            moved = true;
                            break;
                        case "saldo":
                            saldoS = reader.ReadElementContentAsString();
                            moved = true;
                            break;
                    }
                    if (!string.IsNullOrWhiteSpace(dataOperacjiS) &&
                        !string.IsNullOrWhiteSpace(dataKsiegowaniaS) &&
                        !string.IsNullOrWhiteSpace(opisOperacjiS) &&
                        !string.IsNullOrWhiteSpace(kwotaS) &&
                        !string.IsNullOrWhiteSpace(walutaS) &&
                        !string.IsNullOrWhiteSpace(saldoS))
                    {
                        DateTime dataOperacji = DateTime.ParseExact(dataOperacjiS, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        DateTime dataKsiegowania = DateTime.ParseExact(dataKsiegowaniaS, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        double kwota = double.Parse(kwotaS);
                        double saldo = double.Parse(saldoS);

                        result.Rows.Add(category, dataOperacji, dataKsiegowania, opisOperacjiS, kwota, walutaS, saldo);

                        dataOperacjiS = "";
                        dataKsiegowaniaS = "";
                        opisOperacjiS = "";
                        kwotaS = "";
                        walutaS = "";
                        saldoS = "";
                    }
                    reader.MoveToElement();
                }
            }
        }
    }
}