

function reloadPage(date, urlInput) {
    $.ajax({
        url: urlInput,
        data: { dateStr: date },
        dataType: 'html',
        success: function (data) {
            $('#bodyHomePage').html('');
            $('#bodyHomePage').html(data);

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError)
        }
    });
}
var duKienThang;
var duKienNam;
var thucTeThang;
var thucTeNam;
var sanLuongTrongNgay;
var percentThang;
var percentNam;
var formatThang;
var formatNam;
var daThuThap;
var chuaThuThap;
var chart1;
var chart2;
var chart3;
var chart4;

var setValue = function (duKienT, duKienN, thucTeT, thucTeN, sanLuong, percentT, percentN, formatT, formatN,done,notyet) {
    duKienThang = duKienT;
    duKienNam = duKienN;
    thucTeThang = thucTeT;
    thucTeNam = thucTeN;
    sanLuongTrongNgay = sanLuong;
    percentThang = percentT;
    percentNam = percentN;
    formatThang = formatT;
    formatNam = formatN;
    daThuThap = done;
    chuaThuThap = notyet;
    runMain();
}
function runMain() {


    if (duKienThang != null && duKienThang != 0) {
        if (chart1 != null) {
            chart1 = null;
        }
        $("chart1Label").val('');
        am4core.ready(function () {
            // Themes begin
            am4core.useTheme(am4themes_animated);
            // Themes end



            // Create chart instance
            chart1 = am4core.create("insert_chart", am4charts.RadarChart);

            // Add data
            chart1.data = [{
                "category": "Tháng",
                "value": thucTeThang,
                "full": duKienThang
            }];

            // Make chart not full circle
            chart1.radius = am4core.percent(95);
            chart1.startAngle = 160;
            chart1.endAngle = 380;
            chart1.innerRadius = am4core.percent(70);
            chart1.logo.disabled = true;
            chart1.paddingTop = 0;
            chart1.marginTop = 0;

            // Set number format

            // Create axes
            var categoryAxis = chart1.yAxes.push(new am4charts.CategoryAxis());
            categoryAxis.dataFields.category = "category";
            categoryAxis.renderer.grid.template.location = 0;
            categoryAxis.renderer.grid.template.strokeOpacity = 0;
            categoryAxis.renderer.minGridDistance = 10;
            categoryAxis.disabled = true;

            var valueAxis = chart1.xAxes.push(new am4charts.ValueAxis());
            valueAxis.renderer.grid.template.strokeOpacity = 0;
            valueAxis.min = 0;
            valueAxis.max = duKienThang;
            valueAxis.strictMinMax = true;
            valueAxis.disabled = true;


            // Create series
            var series1 = chart1.series.push(new am4charts.RadarColumnSeries());
            series1.dataFields.valueX = "full";
            series1.dataFields.categoryY = "category";
            series1.clustered = false;
            series1.columns.template.fill = new am4core.InterfaceColorSet().getFor("alternativeBackground");
            series1.columns.template.fillOpacity = 0.08;
            series1.columns.template.cornerRadiusTopLeft = 20;
            series1.columns.template.strokeWidth = 0;
            series1.columns.template.radarColumn.cornerRadius = 10;

            var series2 = chart1.series.push(new am4charts.RadarColumnSeries());
            series2.dataFields.valueX = "value";
            series2.dataFields.categoryY = "category";
            series2.clustered = false;
            series2.columns.template.strokeWidth = 0;
            series2.columns.template.tooltipText = "{category}: [bold]{value}MW[/]";
            series2.columns.template.radarColumn.cornerRadius = 20;

            series2.columns.template.adapter.add("fill", function (fill, target) {
                return chart1.colors.getIndex(target.dataItem.index);
            });
            var label = chart1.seriesContainer.createChild(am4core.Label);
            label.textAlign = "middle";
            label.horizontalCenter = "middle";
            label.verticalCenter = "middle";
            label.adapter.add("text", function (text, target) {
                return "[font-size:18px]" + percentThang + "%[/]\n[bold font-size:18px]" + formatThang + " MWh[/]";
            })

            // Add cursor

        }); // end am4core.ready()
    }
    else {
        document.getElementById("chart1Label").innerHTML = "Không có dữ liệu để hiển thị";
    }

    if (duKienNam != null && duKienNam != 0) {
        if (chart2 != null) {
            chart2 = null;
        }
        $("chart2Label").val('');
        am4core.ready(function () {
            // Themes begin
            am4core.useTheme(am4themes_animated);
            // Themes end
            // Create chart instance
            var chart = am4core.create("insert_chart2", am4charts.RadarChart);
            // Add data
            chart.data = [{
                "category": "Năm",
                "value": thucTeNam,
                "full": duKienNam
            }];
            // Make chart not full circle
            chart.radius = am4core.percent(95);
            chart.startAngle = 160;
            chart.endAngle = 380;
            chart.innerRadius = am4core.percent(70);
            chart.logo.disabled = true;
            chart.paddingTop = 0;
            chart.marginTop = 0;
            // Set number format
            // Create axes
            var categoryAxis = chart.yAxes.push(new am4charts.CategoryAxis());
            categoryAxis.dataFields.category = "category";
            categoryAxis.renderer.grid.template.location = 0;
            categoryAxis.renderer.grid.template.strokeOpacity = 0;
            categoryAxis.renderer.minGridDistance = 10;
            categoryAxis.disabled = true;
            var valueAxis = chart.xAxes.push(new am4charts.ValueAxis());
            valueAxis.renderer.grid.template.strokeOpacity = 0;
            valueAxis.min = 0;
            valueAxis.max = duKienNam;
            valueAxis.strictMinMax = true;
            valueAxis.disabled = true;
            // Create series
            var series1 = chart.series.push(new am4charts.RadarColumnSeries());
            series1.dataFields.valueX = "full";
            series1.dataFields.categoryY = "category";
            series1.clustered = false;
            series1.columns.template.fill = new am4core.InterfaceColorSet().getFor("alternativeBackground");
            series1.columns.template.fillOpacity = 0.08;
            series1.columns.template.cornerRadiusTopLeft = 20;
            series1.columns.template.strokeWidth = 0;
            series1.columns.template.radarColumn.cornerRadius = 10;
            var series2 = chart.series.push(new am4charts.RadarColumnSeries());
            series2.dataFields.valueX = "value";
            series2.dataFields.categoryY = "category";
            series2.clustered = false;
            series2.columns.template.strokeWidth = 0;
            series2.columns.template.tooltipText = "{category}: [bold]{value}MW[/]";
            series2.columns.template.radarColumn.cornerRadius = 20;
            series2.columns.template.adapter.add("fill", function (fill, target) {
                return chart.colors.getIndex(target.dataItem.index);
            });
            var label = chart.seriesContainer.createChild(am4core.Label);
            label.textAlign = "middle";
            label.horizontalCenter = "middle";
            label.verticalCenter = "middle";
            label.adapter.add("text", function (text, target) {
                return "[font-size:18px]" + percentNam + "%[/]\n[bold font-size:18px]" + formatNam + " MWh[/]";
            })
            // Add cursor
        }); // end am4core.ready()
    }
    else {
        document.getElementById("chart2Label").innerHTML = "Không có dữ liệu để hiển thị";
    }
    if (sanLuongTrongNgay != null) {
        if (chart3 != null) {
            chart3 = null;
        }
        $("chart3Label").val('');
        am4core.ready(function () {

            // Themes begin
            am4core.useTheme(am4themes_animated);
            // Themes end

            /**
             * Chart design taken from Samsung health app
             */

            chart3 = am4core.create("sanLuongTrongNgay", am4charts.XYChart);
            chart3.hiddenState.properties.opacity = 0; // this creates initial fade-in
            var dataChart = sanLuongTrongNgay;
            chart3.data = dataChart;

            chart3.zoomOutButton.disabled = true;
            chart3.logo.disabled = true;

            var dateAxis = chart3.xAxes.push(new am4charts.ValueAxis());
            dateAxis.renderer.grid.template.strokeOpacity = 0;
            dateAxis.renderer.minGridDistance = 10;
            dateAxis.tooltip.hiddenState.properties.opacity = 1;
            dateAxis.tooltip.hiddenState.properties.visible = true;
            dateAxis.startLocation = 0.5;
            dateAxis.endLocation = 0.5;
            dateAxis.min = 0;
            dateAxis.max = 49;
            dateAxis.renderer.labels.template.adapter.add("text", function (text, target) {
                return text.match('49|50') ? "" : text;
            });
            dateAxis.cursorTooltipEnabled = false;



            var valueAxis = chart3.yAxes.push(new am4charts.ValueAxis());
            valueAxis.renderer.labels.template.fillOpacity = 0.3;
            valueAxis.renderer.grid.template.strokeOpacity = 0;
            valueAxis.min = 0;



            var series = chart3.series.push(new am4charts.ColumnSeries);
            series.dataFields.valueY = "GiaTri";
            series.dataFields.valueX = "ChuKy";
            series.columns.template.tooltipText = "[bold]{valueY}[/]";
            series.tooltip.pointerOrientation = "horizontal";
            series.sequencedInterpolation = true;
            series.tooltip.pointerOrientation = "vertical";
            series.tooltip.hiddenState.properties.opacity = 1;
            series.tooltip.hiddenState.properties.visible = true;
            var columnTemplate = series.columns.template;
            columnTemplate.width = 20;
            columnTemplate.strokeOpacity = 0;
            chart3.responsive.enabled = true;
            chart3.useDefault = false;
            chart3.responsive.rules.push({
                relevant: function (target) {
                    if (target.pixelWidth < 400) {
                        dateAxis.renderer.minGridDistance = 20;
                        columnTemplate.width = 2;
                        return false;
                    }
                    else if (target.pixelWidth >= 400 && target.pixelWidth <= 600) {
                        dateAxis.renderer.minGridDistance = 20;
                        columnTemplate.width = 5;
                        return false;
                    }
                    else if (target.pixelWidth > 600 && target.pixelWidth <= 1100) {
                        columnTemplate.width = 10;
                        return false;
                    }
                    else if (target.pixelWidth > 1100) {
                        columnTemplate.width = 20;
                        return false;
                    }
                    return false;
                }
            });
            var cursor = new am4charts.XYCursor();
            cursor.behavior = "";
            chart3.cursor = cursor;
            cursor.lineX.disabled = true;




            var label = chart3.plotContainer.createChild(am4core.Label);
        }); // end am4core.ready()
    }
    else {
        document.getElementById("chart3Label").innerHTML = "Không có dữ liệu để hiển thị";
    }
    if (daThuThap!=null && chuaThuThap!=null) {
        if (chart4 != null) {
            chart4 = null;
        }
        $("chart4Label").val('');
        am4core.ready(function () {

            // Themes begin
            am4core.useTheme(am4themes_animated);
            // Themes end

            // Create chart instance
            var chart4 = am4core.create("insert_chart4", am4charts.PieChart);

            // Add and configure Series
            var pieSeries = chart4.series.push(new am4charts.PieSeries());
            pieSeries.dataFields.value = "value";
            pieSeries.dataFields.category = "name";
            var colorSet = new am4core.ColorSet();
            colorSet.list = ["#28a745", "#dc3545"].map(function (color) {
                return new am4core.color(color);
            });
            pieSeries.colors = colorSet;

            // Let's cut a hole in our Pie chart the size of 30% the radius
            chart4.innerRadius = am4core.percent(55);
            chart4.logo.disabled = true;
            chart4.paddingLeft = 0;
            chart4.marginLeft = 0;

            // Put a thick white border around each Slice
            pieSeries.slices.template.stroke = am4core.color("#fff");
            pieSeries.slices.template.strokeWidth = 2;
            pieSeries.slices.template.strokeOpacity = 1;
            pieSeries.slices.template
                // change the cursor on hover to make it apparent the object can be interacted with
                .cursorOverStyle = [
                    {
                        "property": "cursor",
                        "value": "pointer"
                    }
                ];

            pieSeries.labels.template.disabled = true;

            pieSeries.ticks.template.disabled = true;

            // Create a base filter effect (as if it's not there) for the hover to return to
            var shadow = pieSeries.slices.template.filters.push(new am4core.DropShadowFilter);
            shadow.opacity = 0;

            // Create hover state
            var hoverState = pieSeries.slices.template.states.getKey("hover"); // normally we have to create the hover state, in this case it already exists

            // Slightly shift the shadow and make it more prominent on hover
            var hoverShadow = hoverState.filters.push(new am4core.DropShadowFilter);
            hoverShadow.opacity = 0.7;
            hoverShadow.blur = 5;

            // Add a legend
            chart4.legend = new am4charts.Legend();
            chart4.legend.position = "right";
            chart4.legend.valign = "middle";

            chart4.data = [{
                "name": "Đã thu thập",
                "value": daThuThap
            }, {
                "name": "Thu thập thiếu",
                "value": chuaThuThap
            }];
            var label = pieSeries.createChild(am4core.Label);
            label.text = daThuThap + chuaThuThap;
            label.horizontalCenter = "middle";
            label.verticalCenter = "middle";
            label.fontSize = 30;

        }); // end am4core.ready()
    }
    else {
        document.getElementById("chart4Label").innerHTML = "Không có dữ liệu để hiển thị";
    }
}