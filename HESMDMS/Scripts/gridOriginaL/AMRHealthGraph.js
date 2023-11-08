$(() => {
    const chart = $('#AMRHealth').dxChart({
        palette: 'Violet',
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/GetAMRHealth",

            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        commonSeriesSettings: {
            argumentField: 'C01_May_2022',
            
        },
        zoomAndPan: {
      valueAxis: 'both',
      argumentAxis: 'both',
      dragToZoom: true,
      allowMouseWheel: true,
      panKey: 'shift',
    },
        margin: {
            bottom: 20,
        },
        argumentAxis: {
            valueMarginsEnabled: false,
            discreteAxisDivisionMode: 'crossLabels',
            grid: {
                visible: true,
            },
        },
        series: [
            { valueField: 'AMRSerialNumber', name: 'May 2022' },
        ],
        legend: {
            verticalAlignment: 'bottom',
            horizontalAlignment: 'center',
            itemTextPosition: 'bottom',
        },
        title: {
            text: 'Energy Consumption in 2004',
            subtitle: {
                text: '(Millions of Tons, Oil Equivalent)',
            },
        },
        export: {
            enabled: true,
        },
        tooltip: {
            enabled: true,
        },
    }).dxChart('instance');
});