
$(() => {
    if (ro == "9") {
        $('#D2C').dxDataGrid({
            //dataSource: JSON.parse(dataj),
            dataSource: DevExpress.data.AspNet.createStore({
                key: "ID",
                loadUrl: "/d2cCustomer",

                onBeforeSend: function (method, ajaxOptions) {
                    ajaxOptions.xhrFields = { withCredentials: true };
                }
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
                    dataField: "InstrumentID",
                    caption: "Meter ID",
                    width: 100,
                    sortOrder: "desc"
                },
                {
                    dataField: "Date",
                    caption: "Date",
                    width: 100,
                    sortOrder: "desc"
                },
                {
                    dataField: "Time",
                    caption: "Time",
                    sortOrder: "desc"
                },
                {
                    dataField: "Record",
                    caption: "Record No",
                    sortOrder: "desc"
                },
                {
                    dataField: "MeasurementValue",
                    caption: "Gas Consumption (m3)",
                },
                {
                    dataField: "TotalConsumption",
                    caption: "Total Consumption (m3)",
                },
                {
                    dataField: "BatteryVoltage",
                    caption: "Battery Voltage (V)",
                }, {
                    dataField: "MagnetTamper",
                    caption: "Magnet Tamper",
                }, {
                    dataField: "CaseTamper",
                    caption: "Case Tamper",
                }, {
                    dataField: "BatteryRemovalCount",
                    caption: "Battery Remove Tamper",
                }, {
                    dataField: "ExcessiveGasFlow",
                    caption: "Excessive Gas Flow",
                }, {
                    dataField: "ExcessivePushKey",
                    caption: "Excessive Push Key Usage Tamper",
                }, {
                    dataField: "SOVTamper",
                    caption: "SOV Tamper",
                }, {
                    dataField: "InvalidUserLoginTamper",
                    caption: "Invalid User Login Tamper",
                },  {
                    dataField: "AccountBalance",
                    caption: "Account Balance (₹)",
                }, {
                    dataField: "eCreditBalance",
                    caption: "E-Balance (₹)",
                }, {
                    dataField: "StandardCharge",
                    caption: "Tariff (₹ / MMBTU)",
                }, {
                    dataField: "ValvePosition",
                    caption: "SOV Status",
                },  {
                    dataField: "GasCalorific",
                    caption: "Gas Calorific Value (Kcal/SCM)",
                }

            ],
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
    }
    else {

        $('#D2C').dxDataGrid({
            //dataSource: JSON.parse(dataj),
            dataSource: DevExpress.data.AspNet.createStore({
                key: "ID",
                loadUrl: "/d2cCustomer",

                onBeforeSend: function (method, ajaxOptions) {
                    ajaxOptions.xhrFields = { withCredentials: true };
                }
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
                    dataField: "FullName",
                    caption: "FullName",
                    width: 250
                },
                {
                    dataField: "InstrumentID",
                    caption: "Meter ID",
                    width: 100
                },
                {
                    dataField: "Date",
                    caption: "Date",
                    width: 150
                },
                {
                    dataField: "Time",
                    caption: "Time",
                },
                {
                    dataField: "Record",
                    caption: "Record No",
                },
                {
                    dataField: "MeasurementValue",
                    caption: "Gas Consumption (m3)",
                },
                {
                    dataField: "TotalConsumption",
                    caption: "Total Consumption (m3)",
                },
                {
                    dataField: "BatteryVoltage",
                    caption: "Battery Voltage (V)",
                },
                 {
                    dataField: "AccountBalance",
                    caption: "Account Balance (₹)",
                }, {
                    dataField: "eCreditBalance",
                    caption: "E-Balance (₹)",
                }, {
                    dataField: "StandardCharge",
                    caption: "Tariff (₹ / MMBTU)",
                }, {
                    dataField: "ValvePosition",
                    caption: "SOV Status",
                }, {
                    dataField: "GasCalorific",
                    caption: "Gas Calorific Value (Kcal/SCM)",
                }

            ],
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
    }
});
