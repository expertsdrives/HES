var uniqueValues = [];
var dataGrid = null;
let prev = 0;
let next = 0;
var startdate = moment();
var tostartdate = moment();
var fromValue = moment();
var Lo = POCL;

$(function () {


    $('#fromdate').dxDateBox({
        value: startdate.subtract(14, "days"),
        type: 'date',
        displayFormat: 'yyyy-MM-dd',
        max: tostartdate,
        min: new Date(2022, 03, 01),
        onValueChanged(data) {
            var fromdateBox = $("#fromdate").dxDateBox("instance");
            fromValue = fromdateBox.option('text');
            var todateBox = $("#todate").dxDateBox("instance");
            var toValue = todateBox.option('text');
            var loc = $("#selectStatus1").dxSelectBox("instance");
            dateDiff(fromValue, toValue, loc.option('text'));
        },
    });
    $('#todate').dxDateBox({
        value: tostartdate.subtract(1, "days"),
        type: 'date',
        displayFormat: 'yyyy-MM-dd',
        max: tostartdate,
        min: new Date(2022, 03, 01),
        onValueChanged(data) {
            var fromdateBox = $("#fromdate").dxDateBox("instance");
            fromValue = fromdateBox.option('text');
            var todateBox = $("#todate").dxDateBox("instance");
            var toValue = todateBox.option('text');
            var loc = $("#selectStatus1").dxSelectBox("instance");
            dateDiff(fromValue, toValue, loc.option('text'));
        },
    });

    const statuses = ['All', 'Water Lily', 'Dreamland Appartment', 'Richmond Grand'];
    $('#selectStatus1').dxSelectBox({
        dataSource: statuses,
        value: statuses[3],
        onValueChanged(data) {
            var fromdateBox = $("#fromdate").dxDateBox("instance");
            fromValue = fromdateBox.option('text');
            var todateBox = $("#todate").dxDateBox("instance");
            var toValue = todateBox.option('text');
            dateDiff(fromValue, toValue, data.value)
        },
    });
    function dateDiff(fromdate, todate, data) {

        const dataGrid = $("#15dayMeter").dxPivotGrid({
            allowFiltering: true,
            allowSorting: true,
            allowSortingBySummary: true,
            allowExpandAll: true,
            height: 570,
            showBorders: true,
            headerFilter: {
                allowSearch: true,
                showRelevantValues: true,

            },
            fieldChooser: {
                allowSearch: true,
            },
            fieldPanel: {
                visible: true,
            },
            showColumnGrandTotals: false,
            height: 550,
            showRowGrandTotals: false,
            showRowTotals: false,
            export: {
                enabled: true,
            },
            fieldChooser: {
                enabled: false,
            },
            onCellPrepared: function (e) {
                if (e.area == "row") {
                    e.cellElement.css("font-weight", "bold");
                }
                if (e.area == "column") {
                    e.cellElement.css("font-weight", "bold");
                }
            },
            dataSource: {
                // ...
                fields: [{
                    dataField: "FullName",
                    caption: "Customer Name",
                    area: "row",
                    width: 150,
                    expanded: true
                },

                {
                    dataField: "BusinessPartnerNo",
                    caption: "Business Partner",
                    area: "row",
                    width: 120,
                    expanded: true
                },
                {
                    dataField: "MeterNumber",
                    caption: "MeterNumber",
                    area: "row",

                    expanded: true
                },
                {
                    dataField: "Street",
                    area: "row",
                    expanded: true
                },
                {
                    dataField: "Date",
                    area: "column",
                    dataType: 'date',
                    groupName: null,
                    format: 'dd-MM-yyyy',
                    sortOrder: 'desc',
                    width: 120

                }, {
                    dataField: "ReadingCount",

                    caption: "Meter Reading",
                    dataType: 'number',
                    summaryType: 'sum',
                    area: "data"
                },


                {
                    dataField: "DailyConsumption",
                    caption: "Gas Sold",
                    dataType: 'number',
                    summaryType: 'sum',
                    area: "data"
                }

                ],
                store: DevExpress.data.AspNet.createStore({
                    key: "ID",
                    loadUrl: "/LastMeterReading?fromdate=" + fromdate + "&todate=" + todate + "&roleid=" + ro + "&data=" + data,

                    onBeforeSend: function (method, ajaxOptions) {
                        ajaxOptions.xhrFields = { withCredentials: true };
                    }
                }),
            },
        }).dxPivotGrid('instance');
    }
    var loc = $("#selectStatus1").dxSelectBox("instance");
    dateDiff(startdate.format("YYYY-MM-DD"), tostartdate.format("YYYY-MM-DD"), loc.option('text'));
    function dateFormat(dateObject) {
        var d = new Date(dateObject);
        var day = d.getDate();
        var month = d.getMonth() + 1;
        var year = d.getFullYear();
        if (day < 10) {
            day = "0" + day;
        }
        if (month < 10) {
            month = "0" + month;
        }
        var date = day + "/" + month + "/" + year;

        return date;
    };

});