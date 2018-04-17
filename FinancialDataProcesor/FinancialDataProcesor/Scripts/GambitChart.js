var chart_form_id = "chart_form";
var chart_path = "Reports/Chart.aspx";
var chart_data_parameter_name = "ChartJsonData";
var chart_title_parameter_name = "ChartTitle";
var chart_units_parameter_name = "ChartUnits";
var chart_scale_parameter_name = "ChartTimeScale";

function AddParameter(win, form, name, value) {
    var input = win.document.createElement('input');
    input.setAttribute("name", name);
    input.setAttribute("value", value);
    input.setAttribute("type", "hidden");
    form.appendChild(input);
}

function OpenChart(ChartTitle, ChartUnits, ChartJsonData, ChartScale, InSameWindow) {
    if (InSameWindow !== true) {
        var chartWindow = window.open(
            "",
            "_blank",
            "location=no,menubar=no,resizable=yes,status=no,titlebar=yes,toolbar=no,width=1200,height=600,left=0,top=0");
        chartWindow.focus();
    }
    else {
        var chartWindow = window;
    }
    
    var form = chartWindow.document.createElement('form');
    chartWindow.document.body.appendChild(form);
    form.setAttribute("id", chart_form_id);
    form.setAttribute("action", chart_path);
    form.setAttribute("method", "post");
    AddParameter(chartWindow, form, chart_data_parameter_name, ChartJsonData);
    AddParameter(chartWindow, form, chart_title_parameter_name, ChartTitle);
    AddParameter(chartWindow, form, chart_units_parameter_name, ChartUnits);
    AddParameter(chartWindow, form, chart_scale_parameter_name, ChartScale);

    form.submit();
}


