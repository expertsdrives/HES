
$(() => {
    $('#D2C').dxDataGrid({
        //dataSource: JSON.parse(dataj),
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/d2c?roleid=" + ro,

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
                width:100
            }, 
            {
                dataField: "Date",
                caption: "Date",
                width: 100
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
            },{
                dataField: "MagnetTamper",
                caption: "Magnet Tamper",
            },{
                dataField: "CaseTamper",
                caption: "Case Tamper",
            },{
                dataField: "BatteryRemovalCount",
                caption: "Battery Remove Tamper",
            },{
                dataField: "ExcessiveGasFlow",
                caption: "Excessive Gas Flow",
            },{
                dataField: "ExcessivePushKey",
                caption: "Excessive Push Key Usage Tamper",
            },{
                dataField: "SOVTamper",
                caption: "SOV Tamper",
            },{
                dataField: "TiltTamper",
                caption: "Unmounted Tamper",
            },{
                dataField: "InvalidUserLoginTamper",
                caption: "Invalid User Login Tamper",
            }, {
                dataField: "NBIoTModuleError",
                caption: "NB-IoT Module Error",
            }, {
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
                dataField: "NBIoTRSSI",
                caption: "NB-IoT RSSI",
            }, {
                dataField: "ContinuousConsumption",
                caption: "Continuous Consumption (Hrs)",
            }, {
                dataField: "MWT",
                caption: "MWT (Secs)",
            }, {
                dataField: "Temperature",
                caption: "Meter Internal Temperature (°C)",
            }, {
                dataField: "GasCalorific",
                caption: "Gas Calorific Value (Kcal/SCM)",
            }, {
                dataField: "TarrifName",
                caption: "Tarrif Name",
            }
            , {
                dataField: "Checksum",
                caption: "Checksum",
            } ,{
                dataField: "DateRx",
                caption: "Date Rx",
                dataType: "date",
                format: 'dd-MM-yyyy',
                width: 100
            }, {
                dataField: "TimeRx",
                caption: "TimeRx",
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
});
