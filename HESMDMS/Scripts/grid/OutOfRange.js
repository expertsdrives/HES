var dataGrid = null;
$(function () {
    dataGrid = $("#OutofRange").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/OutofRange",

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
        scrolling: {
            columnRenderingMode: 'virtual',
        },
        paging: {
            pageSize: 50,
        },
        columnHidingEnabled: true,
        filterRow: {
            visible: true,
            applyFilter: 'auto',
        },
        export: {
            enabled: true,
        },
        columnChooser: {
            enabled: true
        },
        columnFixing: {
            enabled: true
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
                visible: false

            }
        ],

        searchPanel: { visible: true },

        onEditingStart: function (e) {

        },
        pager: {
            allowedPageSizes: [50, 100, 150, 200],
            showInfo: true,
            showNavigationButtons: true,
            showPageSizeSelector: true,
            visible: true,
        },
        onCellPrepared: function (e) {
            if (e.rowType == "header") {
                e.cellElement.css("text-align", "left");
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
    //function imageEditorTemplate(cellElement, cellInfo) {
    //    return $("<div>").dxFileUploader({
    //        selectButtonText: "Select photo",
    //        labelText: "",
    //        name: "myFile",

    //        uploadMode: "instantly",
    //        uploadUrl: "Upload",
    //        accept: "image/*",

    //        onValueChanged: function (e) {
    //            var files = e.value;
    //            $.each(files, function (i, file) {
    //                var fileURL = URL.createObjectURL(file);
    //                cellInfo.setValue(file.name);
    //            });

    //        }
    //    });
    //}

});
function getClick(data) {
    $.ajax({
        url: "ApproveMeter?ID=" + data,
        success: function (result) {
            dataGrid.dxDataGrid("instance").refresh();
        }
    });
}