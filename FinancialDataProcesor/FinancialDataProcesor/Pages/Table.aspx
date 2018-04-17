<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Table.aspx.cs" Inherits="FinancialDataProcesor.Pages.Table" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Report</title>
    <link type="text/css" href="../Css/jquery-ui.min.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Scripts/jquery-1.6.4.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui.min.js"></script>
    <script type="text/javascript" src="../Scripts/GambitChart.js"></script>
    <script type="text/javascript">
        chart_path = "/Pages/Chart.aspx";
    </script>
</head>
<body>
    <form id="Table_form" runat="server">
        <div>
            <div>
                <div>
                    <asp:FileUpload ID="Table_FileUpload" runat="server" AllowMultiple="True" Height="20px" Width="80%" />
                </div>
                <div>
                    <span id="From">
                        From:
                        <script>
                            $(function () {
                                $("#Fromdatepicker").datepicker({
                                    changeMonth: true,
                                    changeYear: true
                                });
                            });
                        </script>
                        <asp:TextBox ID="Fromdatepicker" runat="server"></asp:TextBox>
                    </span>
                    <span id="Till">
                        Till:
                        <script>
                            $(function () {
                                $("#Tilldatepicker").datepicker({
                                    changeMonth: true,
                                    changeYear: true
                                });
                            });
                        </script>
                        <asp:TextBox ID="Tilldatepicker" runat="server"></asp:TextBox>
                    </span>
                    <span>
                        <asp:Button ID="Table_Subbmit" runat="server" Text="Go" />
                    </span>
                </div>
                <div>
                    <script>
                            function NewChart() {
                                chartData = document.getElementById("Table_ChartData").getAttribute("value");
                                OpenChart("chart", "PLN", chartData, "Month", false);
                            }
                    </script>
                    <input id="PrintChart" type="button" value="PrintChart" onclick ="NewChart()" />
                </div>
            </div>
            <div>
                <asp:GridView ID="Table_GridView" runat="server" AllowSorting="True"></asp:GridView>
                <asp:HiddenField ID="Table_ChartData" runat="server" />
                
            </div>
        </div>
    </form>
</body>
</html>
