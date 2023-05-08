var slideIndex = 1;
showDivs(slideIndex);

function plusDivs(n) {
    showDivs(slideIndex += n);
}

function showDivs(n) {
    var i;
    var x = document.getElementsByClassName("mySlides");
    if (n > x.length) { slideIndex = 1 }
    if (n < 1) { slideIndex = x.length }
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
    x[slideIndex - 1].style.display = "block";
}
if (jQuery('#chart-TotalCustomer').length) {

    var options = {
        series: [@ViewBag.AMRWithSales, @ViewBag.totalZeroCinsumption],
    chart: {
        height: 288,
            type: 'donut',
            },

    labels: ["Running", "No Sales"],
        colors: ['#F56692', '#04237D', '#f1ddf2'],
            plotOptions: {
        pie: {
            startAngle: -90,
                endAngle: 270,
                    donut: {
                labels: {
                    show: true,
                        total: {
                        show: true
                    }
                }
            }
        }
    },
    dataLabels: {
        enabled: false
    },
    grid: {
        padding: {

            bottom: 0,
                }
    },
    legend: {
        position: 'bottom',
            offsetY: 8,
                show: true,
            },
    responsive: [{
        breakpoint: 480,
        options: {
            chart: {
                height: 268
            }
        }
    }]
};

var chart = new ApexCharts(document.querySelector("#chart-TotalCustomer"), options);
chart.render();

const body = document.querySelector('body')
if (body.classList.contains('dark')) {
    apexChartUpdate(chart, {
        dark: true
    })
}

document.addEventListener('ChangeColorMode', function (e) {
    apexChartUpdate(chart, e.detail)
})

    };



var options = {
    series: [{
        name: 'Running',
        data: @Html.Raw(Json.Encode(@ViewBag.IntArray))
        },
{
    name: 'No Sales',
        data: @Html.Raw(Json.Encode(@ViewBag.ZeroArray))
}],
chart: {
    type: 'bar',
        height: 350,
            stacked: true,
                toolbar: {
        show: true
    },
    zoom: {
        enabled: true
    }
},
responsive: [{
    breakpoint: 480,
    options: {
        legend: {
            position: 'bottom',
            offsetX: -10,
            offsetY: 0
        }
    }
}],
    colors: ['#F56692', '#04237D', '#f1ddf2'],
        plotOptions: {
    bar: {
        horizontal: false,
            borderRadius: 10
    },
},
xaxis: {

    categories: @Html.Raw(Json.Encode(@ViewBag.MonthArray)),
},
legend: {
    position: 'right',
        offsetY: 40
},
fill: {
    opacity: 1
}
        };

var chart = new ApexCharts(document.querySelector("#chart"), options);
chart.render();