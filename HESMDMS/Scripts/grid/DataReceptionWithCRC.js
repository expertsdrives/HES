var dataGrid = null;
$(function () {
    dataGrid = $("#DataReceptionWithCRC").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/AMRDataReceptionCRC",

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
                dataField: "FullName",
                caption: "Full Name",
               
            },{
                dataField: "MeterNumber",
                caption: "Meter Number",
              
            },
            {
                dataField: "Date",
                caption: "Date",
                dataType: "date",
                format: 'dd-MM-yyyy',
                width: 100,
                sortOrder: 'desc',

            },
            {
                dataField: "Date",
                caption: "Date",
                dataType: "date",
                format: 'dd-MM-yyyy',
                width: 120,

            },
            {
                dataField: "Time",
                caption: "Time",
                dataType: "time",
                width: 100
            },
            {
                dataField: "TXID",
                caption: "AMR TXID",
            },
            {
                dataField: "BusinessPartnerNo",
                caption: "Business PartnerNo",
            },
            {
                dataField: "OpeningMeterReading",
                caption: "Opening Meter Reading (m^3)",
            },
            {
                dataField: "OpeningAMRReading",
                caption: "Opening AMR Reading",
              
            },
            {
                dataField: "DataCount",
                caption: "Data Count",
            },
            {
                dataField: "InstallationDate",
                caption: "Installation Date",
                dataType: "date",
                format: 'dd-MM-yyyy',
            },{
                dataField: "ReadingCount",
                caption: "Current Reading Count",
            },
            {
                dataField: "AMRSerialNumber",
                caption: "AMR Serial Number",
            },
            {
                dataField: "DailyConsumption",
                caption: "Daily Consumption (m^3)",
            },
            
            //{
            //    dataField: "Time",
            //    caption: "Time",
            //},
            //{
            //    dataField: "DataCount",
            //    caption: "Data Count",
            //},
            //{
            //    dataField: "BatteryPower",
            //    caption: "Battery Power",
            //},
            //{
            //    dataField: "Status",
            //    caption: "Status",
            //    cellTemplate(container, options) {

            //        const fieldData = options.data;
            //        container.addClass(fieldData.Status == "Pending" ? 'redColor' : 'greenColor');
            //        //$('<span>')
            //        //    .addClass('redColor')
            //        //    .text(options.text)
            //        //    .appendTo(container);

            //        $('<span>')
            //            .text(options.text)
            //            .appendTo(container);

            //    }
            //},
            //{

            //    caption: 'Action',

            //    //minWidth: 320,
            //    cellTemplate(container, options) {
            //        if (role == 1 || role == 3) {
            //            /*container.addClass('');*/
            //            var dataButton = $('<button class="btn btn-outline-primary btn-sm bg-primary" onclick="getClick(' + options.data.ID + ')">')
            //                .text("Approve");
            //            if (options.data.Status == "Pending") {
            //                dataButton.appendTo(container);
            //            }
            //        }
            //    },
            //},
            // {
            //    type: "buttons",
            //    buttons: ["edit", "delete", {
            //        text: "Approve",
            //        /*icon: "/url/to/my/icon.ico",*/
            //        hint: "My Command",
            //        onClick: function (e) {
            //            var id = e.row.data.ID
            //            // Execute your command here
            //        }
            //    }]
            //}
            //{
            //    dataField: "ZoneID",
            //    caption: "Zone",
            //    lookup: {
            //        dataSource: zone,
            //        displayExpr: "ZoneName",
            //        valueExpr: "ID"
            //    }
            //},
            //{
            //    dataField: "SubZoneID",
            //    caption: "Inner Zone",
            //    lookup: {
            //        dataSource: subZonelist,
            //        displayExpr: "SubZoneName",
            //        valueExpr: "ID"
            //    }
            //},
            //{
            //    dataField: 'CreatedOn',
            //    allowEditing: false,
            //    dataType: "date",
            //    format: 'dd/MM/yyyy',
            //    visible: false
            //},

        ],

        searchPanel: { visible: true },

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