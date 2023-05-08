var uniqueValues = [];
var dataGrid = null;
let prev = 0;
let next = 0;
$(function () {
    dataGrid = $("#MeterMaster").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/LoadMeterMaster",
            insertUrl: "/InsertMeterMaster",
            updateUrl: "/UpdateMeterMaster",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        keyExpr: "ID",
        allowColumnResizing: true,
        columnAutoWidth: true,
   
        showBorders: true,
        wordWrapEnabled: true,
        remoteOperations: true,

        paging: {
            pageSize: 20,
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
                dataField: "Number",
                caption: "Meter Number",
                
            },
            {
                dataField: "Type",
                caption: "Meter Type",

            },
            
            {
                dataField: "IsActive",
                caption: "IsActive",
                dataType: "boolean"
            },
        ],
        editing: {
            mode: "inline",
            allowUpdating: true,
         
            allowAdding: true,
            editEnabled: true,
            useIcons: true
        },
        searchPanel: { visible: true },
        onContentReady: function (e) {
            var saveButton = $(".dx-button[aria-label='Save']");
            if (saveButton.length > 0)
                saveButton.click(function (event) {
                    if (!isUpdateCanceled) {
                        DoSomething(e.component);
                        event.stopPropagation();
                    }
                });
        },
        onEditingStart: function (e) {

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