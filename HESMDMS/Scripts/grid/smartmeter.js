$(() => {
    $('#smartmeterdata').dxDataGrid({
        //dataSource: JSON.parse(dataj),
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/smartmeterdata",

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
                dataField: "BusinessPartnerNo",
                caption: "Business Partner No",

            },
            {
                dataField: "FullName",
                caption: "Customer Name",

            },
            {
                caption: "Address",
                calculateCellValue: function (data) {
                    return data.HouseNumber + ' , ' + data.Street;
                },
                calculateDisplayValue: function (data) {
                    return data.HouseNumber + ' , ' + data.Street;
                }
            },
            {
                dataField: "InstrumentID",
                caption: "Meter ID",
                width: 100
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
            },
            {
                caption: "Maganet Tamper",
                calculateCellValue: function (data) {
                    var input = data.TamperEvents;
                    var result = [];

                    for (var i = 0; i < input.length; i += 2) {
                        result.push(input.substring(i, i + 2));
                    }

                    var output = result.join(',');
                    var splitChunks = output.split(',');
                    return parseInt(splitChunks[0], 16).toString();
                },
                
            },
            {
                caption: "Case Tamper",
                calculateCellValue: function (data) {
                    var input = data.TamperEvents;
                    var result = [];

                    for (var i = 0; i < input.length; i += 2) {
                        result.push(input.substring(i, i + 2));
                    }

                    var output = result.join(',');
                    var splitChunks = output.split(',');
                    return parseInt(splitChunks[1], 16).toString();
                },

            },
            {
                caption: "Battery Removal Count",
                calculateCellValue: function (data) {
                    var input = data.TamperEvents;
                    var result = [];

                    for (var i = 0; i < input.length; i += 2) {
                        result.push(input.substring(i, i + 2));
                    }

                    var output = result.join(',');
                    var splitChunks = output.split(',');
                    return parseInt(splitChunks[2], 16).toString();
                },

            },
            {
                caption: "ExcessiveGas Flow",
                calculateCellValue: function (data) {
                    var input = data.TamperEvents;
                    var result = [];

                    for (var i = 0; i < input.length; i += 2) {
                        result.push(input.substring(i, i + 2));
                    }

                    var output = result.join(',');
                    var splitChunks = output.split(',');
                    return parseInt(splitChunks[3], 16).toString();
                },

            },
            {
                caption: "Excessive PushKey",
                calculateCellValue: function (data) {
                    var input = data.TamperEvents;
                    var result = [];

                    for (var i = 0; i < input.length; i += 2) {
                        result.push(input.substring(i, i + 2));
                    }

                    var output = result.join(',');
                    var splitChunks = output.split(',');
                    return parseInt(splitChunks[4], 16).toString();
                },

            },
            {
                caption: "SOV Tamper",
                calculateCellValue: function (data) {
                    var input = data.TamperEvents;
                    var result = [];

                    for (var i = 0; i < input.length; i += 2) {
                        result.push(input.substring(i, i + 2));
                    }

                    var output = result.join(',');
                    var splitChunks = output.split(',');
                    return parseInt(splitChunks[5], 16).toString();
                },

            },
            {
                caption: "Tilt Tamper",
                calculateCellValue: function (data) {
                    var input = data.TamperEvents;
                    var result = [];

                    for (var i = 0; i < input.length; i += 2) {
                        result.push(input.substring(i, i + 2));
                    }

                    var output = result.join(',');
                    var splitChunks = output.split(',');
                    return parseInt(splitChunks[6], 16).toString();
                },

            },
            {
                caption: "Invalid UserLogin",
                calculateCellValue: function (data) {
                    var input = data.TamperEvents;
                    var result = [];

                    for (var i = 0; i < input.length; i += 2) {
                        result.push(input.substring(i, i + 2));
                    }

                    var output = result.join(',');
                    var splitChunks = output.split(',');
                    return parseInt(splitChunks[7], 16).toString();
                },

            },
            {
                caption: "NBIoTModule Error",
                calculateCellValue: function (data) {
                    var input = data.TamperEvents;
                    var result = [];

                    for (var i = 0; i < input.length; i += 2) {
                        result.push(input.substring(i, i + 2));
                    }

                    var output = result.join(',');
                    var splitChunks = output.split(',');
                    return parseInt(splitChunks[8], 16).toString();
                },

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
            }, {
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
