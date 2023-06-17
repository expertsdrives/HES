$(function () {
    dataGrid = $("#UsersList").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/LoadSmartUser",
            insertUrl: "/InsertSmartUser",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        keyExpr: "ID",

        columns: [

            {
                dataField: "Username",
                caption: "Username",

            },
            {
                dataField: "Password",
                caption: "Password",
                visible: false,
              
            },

            {
                dataField: "FullName",
                caption: "Full Name",
            },
            {
                dataField: "MeterID",
                caption: "Meter ID",
                lookup: {
                    dataSource: DevExpress.data.AspNet.createStore({
                        key: "ID",
                        loadUrl: "/LoadSMeterLookup",
                        onBeforeSend: function (method, ajaxOptions) {
                            ajaxOptions.xhrFields = { withCredentials: true };
                        }
                    }),
                    displayExpr: "MeterID",
                    valueExpr: "MeterID"
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
                title: 'User Registration',
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

    });


});