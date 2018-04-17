
var chart_instance;
window.onload = onloadchart;
function onloadchart() {
    canvas = document.getElementById("chart_instance").getContext('2d');
    hfChartData = document.getElementById("chart_data");

    if (hfChartData !== undefined &&
        hfChartData.hasAttribute("value") &&
        hfChartData.getAttribute("value") !== "") {
        jsonChartData = JSON.parse(hfChartData.getAttribute("value"));
    }
    else
    {
        jsonChartData = {
            type: "bar",
            options: {
                responsive: true,
                maintainAspectRatio: false,
                responsiveAnimationDuration: 1000,
                legend: {
                    display: true,
                    position: "right"
                },
                title: {
                    display: true,
                    position: "top",
                    text: "No Data Provided"
                }
            },
            data: {
                labels: ["No Data Provided"],
                data: [0.0],
                backgroundColor: ["rgba(255, 0, 0, 1.0)"],
                borderColor: ["rgba(255, 0, 0, 0.8)"],
                borderWidth: 1
            }
        };
    }
    chart_instance = new Chart(canvas, jsonChartData);
}
