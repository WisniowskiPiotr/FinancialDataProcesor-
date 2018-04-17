<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Chart.aspx.cs" Inherits="FinancialDataProcesor.Pages.Chart" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Chart</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <link type="text/css" href="../css/FanaticHTML5.css" rel="Stylesheet" />
    <link type="text/css" href="../css/Chart.css" rel="Stylesheet" />
    <script type="text/javascript" src="../scripts/jquery-1.6.4.min.js"></script>
    <script type="text/javascript" src="../scripts/Chart.js"></script>
    <script type="text/javascript" src="../scripts/GambitChart.Internal.js"></script>
</head>
<body>
    <form id="chart_form" runat="server">
        <div>
            <div id="chart_area" class="reports_chart_area">
                <div>
                    <div class="reports_chart_side">
                        <span class ="reports_smalltext">Click on particular dataset label to turn it on/off.</span>
                    </div>
                    <div id="chart_container" class="reports_chart" >
                        <canvas id="chart_instance"></canvas>
                    </div>
                    
                    <div style="display:none;">
                        <br />
                        <!--<a id="chart_tooltipstyle_timebuttonL" class="reports_tooltipstyle">
                            <asp:Button ID="chart_timebuttonL" runat="server" CssClass="button reports_tooltipstyle" Text="&lt;" />
                        </a>-->
                        <a id="chart_tooltipstyle_scalebuttonL" class="reports_tooltipstyle">
                            <asp:Button ID="chart_scalebuttonL" runat="server" CssClass="button reports_tooltipstyle" Text="&lt;-" OnClick="chart_scalebuttonL_Click" />
                        </a>

                        <asp:Label ID="chart_label_scale" runat="server" Text="Month" CssClass="reports_label"/>

                        <a id="chart_tooltipstyle_scalebuttonR" class="reports_tooltipstyle">
                            <asp:Button ID="chart_scalebuttonR" runat="server" CssClass="button reports_tooltipstyle" Text="-&gt;" OnClick="chart_scalebuttonR_Click" />
                        </a>
                        <!--<a id="chart_tooltipstyle_timebuttonR" class="reports_tooltipstyle">
                            <asp:Button ID="chart_timebuttonR" runat="server" CssClass="button reports_tooltipstyle" Text="&gt;" />
                        </a>-->
                        <br />
                    </div>
                </div>
                <asp:HiddenField ID="chart_data" runat="server" Value='' />
            </div>
        </div>
    </form>
</body>
</html>