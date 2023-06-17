var dataGrid = null;
var pld = "";
$(function () {
    dataGrid = $("#SmeterLogs").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/SmartMeterBackLogs",

            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        keyExpr: "ID",
        //grouping: {
        //    autoExpandAll: true,
        //},
        allowColumnReordering: true,
        /* allowColumnResizing: true,*/
        columnAutoWidth: true,
        //groupPanel: {
        //    visible: true,
        //},
        showBorders: true,
        scrolling: {
            columnRenderingMode: 'virtual',
        },
        paging: {
            pageSize: 10,
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
            //{
            //    dataField: "Image",
            //    allowFiltering: false,
            //    width: 150,
            //    allowSorting: false,
            //    /*    editCellTemplate: imageEditorTemplate,*/

            //    cellTemplate: function (container, options) {
            //        $("<div>")
            //            .append($("<img>", { "width": "100%", "src": "../images/" + options.value }))
            //            .appendTo(container);
            //    }
            //},
            //{
            //    dataField: "TXID",
            //    caption: "TXID",
            //},  {
            //    dataField: "VayudutName",
            //    caption: "Vayudut Name",
            //},
            {
                dataField: 'pld',
                caption: 'pld',
                cellTemplate(container, options) {
                    pld = options.data.pld;
                    const fieldData = options.data;
                    container.addClass(fieldData.Status == "Pending" ? 'redColor' : 'greenColor');
                    //$('<span>')
                    //    .addClass('redColor')
                    //    .text(options.text)
                    //    .appendTo(container);

                    $('<span>')
                        .text(options.text)
                        .appendTo(container);
                },
                
            },
            {
                dataField: 'pld',
                caption: 'Meter ID',
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: 'ID',
                        loadUrl: '/MeterFromPLD?pld=' + pld,
                    }),
                    displayExpr: 'MeterID',
                    valueExpr: 'MeterID',
                },

            },
            
            {
                dataField: "EventName",
                caption: "Event Name",

            },
            {
                dataField: "LogDate",
                caption: "Date",
                dataType: "date",
                format: 'dd-MM-yyyy',

                sortOrder: 'desc',

            },
            {
                dataField: "CompletedLogDate",
                caption: "Date",
                dataType: "date",
                format: 'dd-MM-yyyy',


            },
            {
                dataField: "Status",
                caption: "Status",
            },
        ],

        /*  searchPanel: { visible: true },*/

        onEditingStart: function (e) {

        },
        pager: {
            allowedPageSizes: [10, 25, 50, 100],
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