var uniqueValues = [];
var dataGrid = null;
let prev = 0;
let next = 0;
$(function () {
    dataGrid = $("#MagneticTemper").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/MagneticTemper",

            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        keyExpr: "ID",
        grouping: {
            autoExpandAll: true,
        },
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        groupPanel: {
            visible: true,
        },
        showBorders: true,
        wordWrapEnabled: true,
        scrolling: {
            columnRenderingMode: 'virtual',
        },
        paging: {
            pageSize: 300,
        },
        filterRow: {
            visible: true,
            applyFilter: 'auto',
        },
        export: {
            enabled: true,
        },
        columnFixing: {
            enabled: true
        },
    
        columns: [
            {
                dataField: "IndexValue",
                caption: "Sr.No",
                width: 80

            },
            {
                dataField: "FullName",
                caption: "Customer Name",
                width: 250

            },
            {
                dataField: "BusinessPartnerNo",
                caption: "Business Partner",
                width: 140,

            },
            
            {
                dataField: "DailyConsumption",
                headerCellTemplate: $('<span>Gas Sold (m</span><span class="sub">3</span><span>)</span>'),
                width: 110
            },
            {
                dataField: "ReadingCount",
                headerCellTemplate: $('<span>Meter Reading (m</span><span class="sub">3</span><span>)</span>'),
                width: 120,
            },
            {
                dataField: "MagneticTemper",
                caption: "Magnetic Temper",
                width: 100,
            },
            {
                dataField: "CaseTemper",
                caption: "Case Temper",
                width: 100,
            },
            {
                dataField: "ReadingCount",
                width: 100,
            },
            {
                dataField: "MeterNumber",
                caption: "Meter Number",
                width: 120

            },
            {
                dataField: "Date",
                caption: "Rx Date",
                dataType: "date",
                format: 'dd-MM-yyyy',
                width: 100,
                groupIndex: 0,
                sortOrder: 'desc',

            },
            //{
            //    dataField: "Date",
            //    caption: "Rx Date",
            //    dataType: "date",
            //    format: 'dd-MM-yyyy',
            //    width: 115,
            //    sortOrder: 'desc',
            //},

            {
                dataField: "AMRSerialNumber",
                caption: "AMR UID",
                width: 140,
            },
            {
                dataField: "TXID",
                caption: "AMR TXID",
                width: 120,
            },
        ],

        searchPanel: { visible: true },
     
        onEditingStart: function (e) {

        },
        pager: {
            allowedPageSizes: [200, 300, 400, 500],
            showInfo: true,
            showNavigationButtons: true,
            showPageSizeSelector: true,
            visible: true,
        },
        onCellPrepared: function (e) {
            if (e.rowType == "header") {
                e.cellElement.css("text-align", "center");
                e.cellElement.css("font-weight", "bold");
            }
        },
        onInitNewRow: function (e) {

        },
        //onContentReady: function (e) {
        //    var saveButton = $(".dx-button[aria-label='Save']");
        //    if (saveButton.length > 0)
        //        saveButton.click(function (event) {
        //            if (!isUpdateCanceled) {
        //                DoSomething(e.component);
        //                event.stopPropagation();
        //            }
        //        });
        //}
    });


});

function getClick(data) {
    $.ajax({
        url: "ApproveMeter?ID=" + data,
        success: function (result) {
            dataGrid.dxDataGrid("instance").refresh();
        }
    });
}