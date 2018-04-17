
function zero(dataset, index)
{
    dataset.data = [];
    dataset.backgroundColor = [];
    dataset.borderColor = [];
}

function addData(chart, jdata, current_bars) {
    
    chart.data.labels.unshift(jdata.data.labels[jdata.data.labels.length - current_bars-1]);
    for (j = 0; j < chart.data.datasets.length; j++) {
        chart.data.datasets[j].data.unshift(jdata.data.datasets[j].data[jdata.data.datasets[j].data.length - current_bars - 1].toFixed(2));
        if (Array.isArray(jdata.data.datasets[j].backgroundColor)) {
            chart.data.datasets[j].backgroundColor.unshift(jdata.data.datasets[j].backgroundColor[jdata.data.datasets[j].backgroundColor.length - current_bars - 1]);
            total_hours = total_hours + parseFloat(jdata.data.datasets[j].data[jdata.data.datasets[j].data.length - current_bars - 1]);
            if (jdata.data.datasets[j].backgroundColor[jdata.data.datasets[j].backgroundColor.length - current_bars - 1].split(',')[3] == "0.8)")
                def_hours = def_hours + parseFloat(jdata.data.datasets[j].data[jdata.data.datasets[j].data.length - current_bars - 1]);
        }
        else {
            chart.data.datasets[j].backgroundColor = jdata.data.datasets[j].backgroundColor;
            norm_hours = norm_hours + parseFloat(jdata.data.datasets[j].data[jdata.data.datasets[j].data.length - current_bars - 1]);
        }

        if (Array.isArray(jdata.data.datasets[j].borderColor))
            chart.data.datasets[j].borderColor.unshift(jdata.data.datasets[j].borderColor[jdata.data.datasets[j].borderColor.length - current_bars - 1]);
        else
            chart.data.datasets[j].borderColor = jdata.data.datasets[j].borderColor;
        
    }

    var percent_string;
    hbars = document.getElementById("FreelancersGeneralTab_HiddenField_Client");
    if (hbars.getAttribute("value").split(',')[2] == "projects") {
        if (total_hours == 0)
            total_hours = 1;
        if (def_hours > 0)
            percent_string = ",  Default projects: " + (100 * def_hours / total_hours).toFixed(0) + "%";
        else
            percent_string = "";
    }
    else
        percent_string = "";
    
    chart.options.title.display = true;
    chart.options.title.text = "Occupancy: " + (100 * total_hours / norm_hours).toFixed(0) + "%";
    chart.options.title.text += percent_string;

    chart.update();
}

function removeData(chart) {
    chart.data.labels.shift();
    for (j = 0 ; j < chart.data.datasets.length; j++) {
        chart.data.datasets[j].data.shift();
        if (Array.isArray(jdata.data.datasets[j].backgroundColor)) {
            chart.data.datasets[j].backgroundColor.shift();
            total_hours = total_hours - parseFloat(jdata.data.datasets[j].data[jdata.data.datasets[j].data.length - current_bars - 1]);
            if (jdata.data.datasets[j].backgroundColor[jdata.data.datasets[j].backgroundColor.length - current_bars - 1].split(',')[3] == "0.8)")
                def_hours = def_hours - parseFloat(jdata.data.datasets[j].data[jdata.data.datasets[j].data.length - current_bars - 1]);
        }
        else
        {
            norm_hours = norm_hours - parseFloat(jdata.data.datasets[j].data[jdata.data.datasets[j].data.length - current_bars - 1]);
        }
        if (Array.isArray(jdata.data.datasets[j].borderColor))
            chart.data.datasets[j].borderColor.shift();
    }
    //set_def_project_percent()
    var percent_string;
    hbars = document.getElementById("FreelancersGeneralTab_HiddenField_Client");
    if (hbars.getAttribute("value").split(',')[2] == "projects") {
        if (total_hours == 0)
            total_hours = 1;
        if (def_hours > 0)
            percent_string = ",  Default projects: " + (100 * def_hours / total_hours).toFixed(0) + "%";
        else
            percent_string = "";
    }
    else
        percent_string = "";

    chart.options.title.display = true;
    chart.options.title.text = "Occupancy: " + (100 * total_hours / norm_hours).toFixed(0) + "%";
    chart.options.title.text += percent_string;

    chart.update();
}

function chartLclick()
{
    if (parseInt(current_bars) < parseInt(loaded_bars) && parseInt(current_bars) < jdata.data.labels.length ) {
        addData(myChart, jdata, current_bars);
        current_bars++;
        hbars.setAttribute("value", (loaded_bars) + "," + (current_bars)+ ","+(data_type) );
    }
    else {
        var reload = document.getElementById("FreelancersGeneralTab_Button_reload");
        if (reload !== undefined) {
            current_bars++;
            hbars.setAttribute("value", (2 * loaded_bars) + "," + (current_bars) + "," + (data_type)  );
            reload.click();
        }
    }
    if (current_bars > 1)
        document.getElementById("JSButtonR").disabled = false;
}

function chartRclick()
{
    if ( current_bars > 1) {
        current_bars--;
        hbars.setAttribute("value", (loaded_bars) + "," + (current_bars) + "," + (data_type)  );
        removeData(myChart);
        if (current_bars == 1)
            document.getElementById("JSButtonR").disabled = true;
    }
    
}

var chart_area;
var ctx;
var myChart;
var current_bars;
var loaded_bars;
var jdata;
var hbars;
var hfield;
var data_type;
var total_hours = parseFloat(0);
var def_hours = parseFloat(0);
var norm_hours = parseFloat(0);


function chart_click()
{
    //var opened = window.open("");
    //opened.document.write("<html><head><title>MyTitle</title></head><body>test</body></html>");
}

function onloadchart()
{
    total_hours = parseFloat(0);
    def_hours = parseFloat(0);
    norm_hours = parseFloat(0);

    chart_area = document.getElementById("chart_area1");
    if (chart_area === undefined || chart_area === null)
        return;

    hfield = document.getElementById("FreelancersGeneralTab_HiddenField_JSon_4chart");
    hbars = document.getElementById("FreelancersGeneralTab_HiddenField_Client");
    ctx = document.getElementById("myChart").getContext('2d');
    

    if (hfield !== undefined && hfield!=null && hfield.hasAttribute("value") && hfield.getAttribute("value") !== ""
        && hbars !== undefined && hbars!=null && hbars.hasAttribute("value") && hbars.getAttribute("value") !== "")
    {
        var tmp = hbars.getAttribute("value").split(',');
        loaded_bars = tmp[0];
        current_bars = tmp[1];
        data_type = tmp[2];
        if (current_bars == 1)
            document.getElementById("JSButtonR").disabled = true;

        jdata = JSON.parse(hfield.getAttribute("value"));

        if (jdata.data.labels.length - 1 < current_bars) {
            current_bars = jdata.data.labels.length - 1;
            hbars.setAttribute("value", (loaded_bars) + "," + (current_bars) + "," +data_type);
        }

        var chartdata = JSON.parse(hfield.getAttribute("value"));
        chartdata.data.labels = [];
        chartdata.data.datasets.forEach(zero);
        myChart = new Chart(ctx, chartdata);


        for (i = 0 ; i < current_bars && i < jdata.data.labels.length; i++)
        {
            addData(myChart, jdata, i);
        }

        set_active_tab();
    }
}



function chart_projects_click()
{
    hfield.setAttribute("value", "");
    hbars.setAttribute("value", (loaded_bars) + "," + (current_bars) + "," + "projects");
    var reload = document.getElementById("FreelancersGeneralTab_Button_reload");
    if (reload !== undefined) {
        reload.click();
    }
}

function chart_roles_click()
{
    hfield.setAttribute("value", "");
    hbars.setAttribute("value", (loaded_bars) + "," + (current_bars) + "," + "roles");
    var reload = document.getElementById("FreelancersGeneralTab_Button_reload");
    if (reload !== undefined) {
        reload.click();
    }
}

function chart_langs_click()
{
    hfield.setAttribute("value", "");
    hbars.setAttribute("value", (loaded_bars) + "," + (current_bars) + "," + "langs");
    var reload = document.getElementById("FreelancersGeneralTab_Button_reload");
    if (reload !== undefined) {
        reload.click();
    }
}

function set_active_tab()
{
    var t = hbars.getAttribute("value").split(',')[2];
    var i = 0;
    switch (t) {
        case "projects":
            i= 0;
            break;
        case "roles":
            i = 1;
            break;
        case "langs":
            i= 2;
            break;
    }

    for (j = 0; j < 3; j++)
    {
        var tab = document.getElementById("Chart_Tab" + j);
        if (tab != null) {
            if (j == i)
                tab.className = "tab2_header_active";
            else
                tab.className = "tab2_header_inactive";
        }
    }
}

function set_def_project_percent()
{
    if (hbars.getAttribute("value").split(',')[2] == "projects") {
        if (total_hours == 0)
            total_hours = 1;
        var res = 100 * def_hours / total_hours;
        var label = document.getElementById("FreelancersGeneralTab_Default_projects_percent");
        label.innerHTML = "Default projects - " + res.toFixed(1) + " %";
    }
    else
    {
        var label = document.getElementById("FreelancersGeneralTab_Default_projects_percent");
        label.innerHTML = "";
    }

}



