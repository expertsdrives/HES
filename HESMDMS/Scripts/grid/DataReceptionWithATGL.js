var uniqueValues = [];
var dataGrid = null;
let prev = 0;
let next = 0;
$(function () {
    const statuses = ['All', 'Water Lily', 'Dreamland Appartment', 'Richmond Grand'];
    $('#selectStatus').dxSelectBox({
        dataSource: statuses,
        value: statuses[3],
        onValueChanged(data) {
            if (data.value === 'All') { dataGrid.clearFilter(); } else { dataGrid.filter(['Street', '=', data.value]); }
        },
    });
    const dataGrid = $("#DataReceptionWithCRC").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/AMRDataReceptionCRC?roleid=" + ro,

            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        keyExpr: "ID",
        grouping: {
            autoExpandAll: false,
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
        //filterRow: {
        //    visible: true,
        //    applyFilter: 'auto',
        //},
        export: {
            enabled: true,
        },
        columnFixing: {
            enabled: true
        },
        summary: {
            totalItems: [
                {
                    column: 'DailyConsumption',
                    summaryType: 'sum',
                    precision: 3,
                    customizeText(data) {
                        return "Total: " + data.value.toFixed(3) + "\n Revenue: Rs." + ((0.0353 * (data.value.toFixed(3))) * 1201).toFixed(2);
                    }
                },
                {
                    column: 'BusinessPartnerNo',
                    summaryType: 'count',
                    precision: 3,
                    customizeText(data) {
                        if (ro != 5) {
                            return "Count: " + distinctCount;
                        }
                    }
                },
                {
                    column: 'MeterNumber',
                    summaryType: 'count',
                    precision: 3,
                    customizeText(data) {
                        if (ro != 5) {
                            return "Count: " + distinctMeterCount;
                        }
                    }
                }
            ],
        },
        columns: [
            {
                dataField: "IndexValue",
                caption: "Sr.No",
                width: 80,
                allowSorting: false,

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
            {
                dataField: "Date",
                caption: "Rx Date",
                dataType: "date",
                format: 'dd-MM-yyyy',
                width: 115,
                sortOrder: 'desc',
            },
            {
                dataField: "Time",
                caption: "Time",
                width: 100,
            },
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
            {
                dataField: "Street",
                visible:false
              
            }
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
        onContentReady: function (e) {
            if (!e.component.isNotFirstLoad) {
                e.component.isNotFirstLoad = true; e.component.expandRow(e.component.getKeyByRowIndex(0));
            }
        }
    }).dxDataGrid('instance');
    
});

function getClick(data) {
    $.ajax({
        url: "ApproveMeter?ID=" + data,
        success: function (result) {
            dataGrid.dxDataGrid("instance").refresh();
        }
    });
}