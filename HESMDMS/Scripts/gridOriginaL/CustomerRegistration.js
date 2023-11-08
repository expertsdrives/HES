var uniqueValues = [];
var dataGrid = null;
let prev = 0;
let next = 0;
$(function () {
    dataGrid = $("#CustomerRegistration").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/LoadCustomerRegistration",
            insertUrl: "/InsertCustomerRegistration",
            updateUrl: "/UpdateCustomerRegistration",
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
                dataField: "BusinessPartnerNo",
                caption: "Business Partner",
                
            },
            {
                dataField: "ContractAccount",
                caption: "Contract Account",
                visible: false
            },
            
            {
                dataField: "MeterReadingUnit",
                caption: "Meter Reading Unit",
            },
            {
                dataField: "ConnectionObject",
                caption: "Connection Object",
                visible: false
            },
            {
                dataField: "FullName",
                caption: "Full Name",
            },
            {
                dataField: "HouseNumber",
                caption: "House Number",
            },
            {
                dataField: "Street",
                caption: "Street",
            },
            {
                dataField: "Street2",
                caption: "Street2",
            },
            {
                dataField: "Street3",
                caption: "Street3",
            },
            {
                dataField: "Street4",
                caption: "Street4",
            },
            {
                dataField: "Street5",
                caption: "Street5",
            },
            {
                dataField: "City",
                caption: "City",
            },
            {
                dataField: "PostalCode",
                caption: "PostalCode",
            },
            {
                dataField: "SerialNumber",
                caption: "Serial Number",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "ID",
                        loadUrl: "/LoadMeterLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                        }
                    }),
                    displayExpr: "Number",
                    valueExpr: "Number"
                }
            },
        ],
        editing: {
            mode: "popup",
            allowUpdating: true,
         
            allowAdding: true,
            editEnabled: true,
            useIcons: true,
            popup: {
                title: 'Customer Registration',
                showTitle: true,
                width: 700,
                height: 525,
                position: {
                    my: "top",
                    at: "top",
                    of: window
                }
            },
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