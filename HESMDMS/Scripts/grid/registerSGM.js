$(() => {
    $('#registerSGM').dxDataGrid({
        //dataSource: JSON.parse(dataj),
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/SGMReg",
            insertUrl: "/InsertSGMReg",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            },
        }),
        filterRow: {
            visible: true,
            applyFilter: 'auto',
        },
        columnChooser: {
            enabled: true,
        },
        columns: [
            {
                dataField: "CustomreID",
                caption: "CustomerID",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "ID",
                        loadUrl: "/DeltaCustomers",

                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                        }
                    }),
                    displayExpr: 'ContractAcct',
                    valueExpr: 'ContractAcct',
                },
            },
            {
                dataField: "SmartMeterSerialNumber",
                caption: "Meter Number",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "ID",
                        loadUrl: "/SGMMeterFetch",

                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                        }
                    }),
                    displayExpr: 'MeterSerialNumber',
                    valueExpr: 'MeterSerialNumber',
                },
            },
            

        ],
        editing: {
            mode: "inline",
            allowUpdating: true,

            allowAdding: true,
            editEnabled: true,
            useIcons: true,
           
        },
        searchPanel: {
            visible: true,
            width: 240,
            placeholder: 'Search...',
        },
        headerFilter: {
            visible: true,
        },
        onCellPrepared: function (e) {
            if (e.rowType == "header") {
                e.cellElement.css("text-align", "center");
                e.cellElement.css("font-weight", "bold");
            }
        },
        showBorders: true,
        wordWrapEnabled: true,
        columnAutoWidth: true,
        scrolling: {
            columnRenderingMode: 'virtual',
        },
        export: {
            enabled: true,
        },
        columnFixing: {
            enabled: true
        },
    });
});
