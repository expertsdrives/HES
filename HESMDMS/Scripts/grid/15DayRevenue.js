var uniqueValues = [];
var dataGrid = null;
let prev = 0;
let next = 0;
var startdate = moment();
var tostartdate = moment();
var fromValue = moment();
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
            dateDiff(fromValue, toValue);
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
            dateDiff(fromValue, toValue);
        },
    });
    function dateDiff(fromdate, todate) {
        $("#15dayRevenue").dxPivotGrid({
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
            showColumnGrandTotals: true,
            height: 550,
            showRowGrandTotals: true,
            showRowTotals: false,
            export: {
                enabled: true,
            },
            fieldChooser: {
                enabled: false,
            },
            onContentReady: function (e) {
                e.element.find(".dx-row-total dx-grandtotal dx-last-cell").first().text("My Caption");

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
                    dataField: "Date",
                    area: "column",
                    dataType: 'date',
                    groupName: null,
                    format: 'dd-MM-yyyy',
                    sortOrder: 'desc',
                    width: 120

                },
                {
                    dataField: "DailyConsumption",
                    caption: "Gas Sold",
                    dataType: 'number',
                    summaryType: 'sum',
                    format: { precision: 2 },
                    precision: 2,
                    area: "data"
                    },
                    {
                        dataField: "MMBTU",
                        caption: "MMBTU",
                        dataType: 'number',
                        summaryType: 'sum',
                        format: { precision: 2 },
                        precision: 2,
                        area: "data"
                    },
                    {
                    dataField: "Revenue",
                    caption: "Revenue",
                    dataType: 'number',
                    summaryType: 'sum',
                    format: { precision: 2 },
                    customizeText: function (arg) {
                        return "₹ " + arg.valueText;
                    },
                    area: "data"
                }
                ],
                store: DevExpress.data.AspNet.createStore({
                    key: "ID",
                    loadUrl: "/LastMeterRevenue?fromdate=" + fromdate + "&todate=" + todate + "&roleid=" + ro,

                    onBeforeSend: function (method, ajaxOptions) {
                        ajaxOptions.xhrFields = { withCredentials: true };
                    }
                }),
            },
        });
    }
    dateDiff(startdate.format("YYYY-MM-DD"), tostartdate.format("YYYY-MM-DD"));
});