var uniqueValues = [];
var dataGrid = null;
let prev = 0;
let next = 0;
$(function () {
    dataGrid = $("#controlpanel").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/SmartMeterCommands",
            insertUrl: "/InsertSmartMeterCommands",
            updateUrl: "/UpdateSmartMeterCommands",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        keyExpr: "ID",
        allowColumnResizing: true,
        columnAutoWidth: true,
        showBorders: true,
        columnResizingMode: 'widget',
        columnMinWidth: 150,
        remoteOperations: true,
        wordWrapEnabled: true,
        paging: {
            pageSize: 10,
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
                dataField: "EventName",
                caption: "Event Name",

            },
            {
                dataField: "ParameterName",
                caption: "Parameter Name",

            },

            {
                dataField: "NoOfBytes",
                caption: "No of Bytes",

            },
            {
                dataField: "type",
                caption: "Type",

            },
            {
                dataField: "Addr",
                caption: "Address",

            },
            {
                dataField: "Remarks",
                caption: "Remarks",

            }, {
                type: "buttons",
                buttons: [
                    {
                        text: "Execute",
                        hint: "Execute",
                        elementAttr: {
                            class: "btn btn-outline-primary btn-sm bg-primary"
                        },
                        onClick: function (e) {
                            var id = e.row.data.ID;
                            redirectToPage(id);
                        }
                    }
                ]
            }
        ],
        editing: {
            mode: "inline",
            allowUpdating: true,

            allowAdding: true,
            editEnabled: true,
            useIcons: true,
           
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
                e.cellElement.css("white-space", "nowrap");
            }
            if (e.column.dataField == "FullName") {
                e.cellElement.css("min-width", "250px");
            }
            if (e.column.dataField == "MeterNumber") {
                e.cellElement.css("min-width", "100px");
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
    function redirectToPage(id) {
        // Replace 'yourPageUrl' with the actual URL you want to redirect to
        window.location.href = "CommandExecution?id=" + id;
    }

    
});