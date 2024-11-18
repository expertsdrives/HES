$(document).ready(function () {

    function initializeGrid(startDate, endDate) {

        $(() => {

            if (ro == "9") {

                $('#D2C').dxDataGrid({

                    dataSource: DevExpress.data.AspNet.createStore({

                        key: "ID",

                        loadUrl: "/d2c?roleid=" + ro,

                        onBeforeSend: function (method, ajaxOptions) {

                            ajaxOptions.xhrFields = { withCredentials: true };

                            if (startDate) {

                                ajaxOptions.url += (ajaxOptions.url.indexOf('?') === -1 ? '?' : '&') + 'startDate=' + encodeURIComponent(startDate);

                            }

                            if (endDate) {

                                ajaxOptions.url += (ajaxOptions.url.indexOf('?') === -1 ? '?' : '&') + 'endDate=' + encodeURIComponent(endDate);

                            }

                            ajaxOptions.url += (ajaxOptions.url.indexOf('?') === -1 ? '?' : '&') + 'roleid=' + encodeURIComponent(ro);

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

                        },

                        {

                            dataField: "MagnetTamper",

                            caption: "Magnet Tamper",

                        },

                        {

                            dataField: "CaseTamper",

                            caption: "Case Tamper",

                        },

                        {

                            dataField: "BatteryRemovalCount",

                            caption: "Battery Remove Tamper",

                        },

                        {

                            dataField: "ExcessiveGasFlow",

                            caption: "Excessive Gas Flow",

                        },

                        {

                            dataField: "ExcessivePushKey",

                            caption: "Excessive Push Key Usage Tamper",

                        },

                        {

                            dataField: "SOVTamper",

                            caption: "SOV Tamper",

                        },

                        {

                            dataField: "InvalidUserLoginTamper",

                            caption: "Invalid User Login Tamper",

                        },

                        {

                            dataField: "AccountBalance",

                            caption: "Account Balance (₹)",

                        },

                        {

                            dataField: "eCreditBalance",

                            caption: "E-Balance (₹)",

                        },

                        {

                            dataField: "StandardCharge",

                            caption: "Tariff (₹ / MMBTU)",

                        },

                        {

                            dataField: "ValvePosition",

                            caption: "SOV Status",

                        },

                        {

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

                    dataSource: DevExpress.data.AspNet.createStore({

                        key: "ID",

                        loadUrl: "/d2c?roleid=" + ro,

                        onBeforeSend: function (method, ajaxOptions) {

                            ajaxOptions.xhrFields = { withCredentials: true };

                            if (startDate) {

                                ajaxOptions.url += (ajaxOptions.url.indexOf('?') === -1 ? '?' : '&') + 'startDate=' + encodeURIComponent(startDate);

                            }

                            if (endDate) {

                                ajaxOptions.url += (ajaxOptions.url.indexOf('?') === -1 ? '?' : '&') + 'endDate=' + encodeURIComponent(endDate);

                            }

                            ajaxOptions.url += (ajaxOptions.url.indexOf('?') === -1 ? '?' : '&') + 'roleid=' + encodeURIComponent(ro);

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
                            dataField: "TiltTamper",
                            caption: "Unmounted Tamper",
                        }, {
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
                        }, {
                            dataField: "DateRx",
                            caption: "Date Rx",
                            dataType: "date",
                            format: 'dd-MM-yyyy',
                            width: 100,
                            sortOrder: "desc"
               
                        }, {
                            dataField: "TimeRx",
                            caption: "TimeRx",
                        }

                    ],
                    // your columns configuration goes here

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

    }

    // Calculate last 7 days

    var today = new Date();

    var last7Days = new Date(today.getTime() - (7 * 24 * 60 * 60 * 1000));

    var formattedToday = today.toISOString().slice(0, 19).replace('T', ' ');

    var formattedLast7Days = last7Days.toISOString().slice(0, 19).replace('T', ' ');

    initializeGrid(formattedLast7Days, formattedToday);

    // Apply filter button click event

    $("#applyFilter").on("click", function () {

        var startDate = $("#startDate").val();

        var endDate = $("#endDate").val();

        if (!startDate || !endDate) {

            alert("Please select both start and end dates.");

            return;

        }

        console.log("Applying filter: Start Date = " + startDate + ", End Date = " + endDate);

        initializeGrid(startDate, endDate);

        // Refresh the grid to apply the filter

        var gridInstance = $("#D2C").dxDataGrid("instance");

        gridInstance.refresh();

    });

});
