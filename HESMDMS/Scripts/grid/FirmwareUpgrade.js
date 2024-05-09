$(() => {
    var pld = "";
    var aid = "";
    var eeid = "";
    var apidata = "";
    var mid = "";
    $('#SelectMeter1').dxSelectBox({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/SelectSmartMeter?roleid=" + ro,
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        displayExpr: 'TempMeterID',
        valueExpr: 'TempMeterID',
        value: mid,
        name: 'TempMeterID',
        id: 'TempMeterID',
        onValueChanged: function (data) {
            var json = Jsondata;
            function getObjects(obj, key, val) {
                var objects = [];
                for (var i in obj) {
                    if (!obj.hasOwnProperty(i)) continue;
                    if (typeof obj[i] == 'object') {
                        objects = objects.concat(getObjects(obj[i], key, val));
                    } else if (i == key && obj[key] == val) {
                        objects.push(obj);
                    }
                }
                return objects;
            }

            var meterData = getObjects(json, 'TempMeterID', data.value);
            mid = data.value;
            pld = meterData[0].PLD;
            console.log(pld);
            aid = meterData[0].AID;
            eeid = meterData[0].EID;
            if (pld == null || aid == null) {
                DevExpress.ui.notify("AID or PID cannot be NULL check the Meter Master", 'warning', 1000);
                return false;
            }
            var grid = $("#gridContainer").dxDataGrid({
                dataSource: $.connection.gridHub,
                remoteOperations: true,
                repaintChangesOnly: true,
                highlightChanges: true,
                showBorders: true,
                paging: {
                    pageSize: 10,
                },
                export: {
                    enabled: true,
                },
                columnFixing: {
                    enabled: true
                }, searchPanel: { visible: true },
                columns: [
                    // Define your columns here
                    { dataField: "MeterID", caption: "Meter ID" },
                    { dataField: "pld", caption: "PLD" },
                    { dataField: "PacketNumber", caption: "Packet Number" },
                    { dataField: "PacketData", caption: "Packet Data" },
                    { dataField: "JioResponse", caption: "Jio Response" },
                    {
                        dataField: "CreatedDate", caption: "Created Date",
                        cellTemplate: function (container, options) {
                            // Input date string
                            var dateString = options.value;

                            // Extract the numeric part of the date string
                            var numericPart = dateString.match(/\d+/)[0];

                            // Create a Date object from the numeric timestamp
                            var date = new Date(parseInt(numericPart));

                            //// Check if the date is valid
                            //if (!isNaN(date.getTime())) {
                            //    // Format the date as yyyy-MM-dd
                            //    var formattedDate =
                            //    console.log(formattedDate);
                            //} else {
                            //    console.log("Invalid date");
                            //}
                            //// Parse the date and format it as needed (e.g., 'dd-MM-yyyy')
                            //const dateValue = new Date(parseInt(options.value.replace('/Date(', '').replace(')/', '')));
                            //const formattedDate = dateValue.toLocaleDateString('en-US', { year: 'numeric', month: '2-digit', day: '2-digit' });
                            $("<div>")
                                .text(date.toISOString().split('T')[0])
                                .appendTo(container);
                        }
                    },
                ],

                onContentReady: function (e) {
                    // No need for custom loading here
                    $.getJSON("Firmware/GetData?pld=" + pld, function (data) {
                        // Assuming "gridElementId" is the ID of your DevExtreme DataGrid container
                        e.component.option("dataSource", data);
                    });
                },
                onCellPrepared: function (e) {
                    if (e.rowType == "header") {
                        e.cellElement.css("text-align", "left");
                        e.cellElement.css("font-weight", "bold");
                    }
                },
                pager: {
                    allowedPageSizes: [10, 25, 50, 100],
                    showInfo: true,
                    showNavigationButtons: true,
                    showPageSizeSelector: true,
                    visible: true,
                },
            }).dxDataGrid("instance");
            //$.ajax({
            //    url: '/SmartMeter/Firmware/CheckStatus',
            //    data: { pld: pld },
            //    contentType: 'application/html ; charset:utf-8',
            //    type: 'GET',
            //    dataType: 'html',
            //    success: function (result) {

            //    }
            //});
            console.log(meterData);
        },
    });
    $('#SelectMeter2').dxSelectBox({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/SelectSmartMeter",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        displayExpr: 'MeterID',
        valueExpr: 'MeterID',
        value: mid,
        name: 'MeterID1',
        id: 'MeterID',
        onValueChanged: function (data) {
            var json = Jsondata;
            function getObjects(obj, key, val) {
                var objects = [];
                for (var i in obj) {
                    if (!obj.hasOwnProperty(i)) continue;
                    if (typeof obj[i] == 'object') {
                        objects = objects.concat(getObjects(obj[i], key, val));
                    } else if (i == key && obj[key] == val) {
                        objects.push(obj);
                    }
                }
                return objects;
            }

            var meterData = getObjects(json, 'MeterID', data.value);
            mid = data.value;
            pld = meterData[0].PLD;
            console.log(pld);
            aid = meterData[0].AID;
            eeid = meterData[0].EID;
            if (pld == null || aid == null) {
                DevExpress.ui.notify("AID or PID cannot be NULL check the Meter Master", 'warning', 1000);
                return false;
            }
            var grid = $("#gridContainer").dxDataGrid({
                dataSource: $.connection.gridHub,
                remoteOperations: true,
                repaintChangesOnly: true,
                highlightChanges: true,
                showBorders: true,
                paging: {
                    pageSize: 10,
                },
                export: {
                    enabled: true,
                },
                columnFixing: {
                    enabled: true
                }, searchPanel: { visible: true },
                columns: [
                    // Define your columns here
                    { dataField: "MeterID", caption: "Meter ID" },
                    { dataField: "pld", caption: "PLD" },
                    { dataField: "PacketNumber", caption: "Packet Number" },
                    { dataField: "PacketData", caption: "Packet Data" },
                    { dataField: "JioResponse", caption: "Jio Response" },
                    {
                        dataField: "CreatedDate", caption: "Created Date",
                        cellTemplate: function (container, options) {
                            // Input date string
                            var dateString = options.value;

                            // Extract the numeric part of the date string
                            var numericPart = dateString.match(/\d+/)[0];

                            // Create a Date object from the numeric timestamp
                            var date = new Date(parseInt(numericPart));

                            //// Check if the date is valid
                            //if (!isNaN(date.getTime())) {
                            //    // Format the date as yyyy-MM-dd
                            //    var formattedDate =
                            //    console.log(formattedDate);
                            //} else {
                            //    console.log("Invalid date");
                            //}
                            //// Parse the date and format it as needed (e.g., 'dd-MM-yyyy')
                            //const dateValue = new Date(parseInt(options.value.replace('/Date(', '').replace(')/', '')));
                            //const formattedDate = dateValue.toLocaleDateString('en-US', { year: 'numeric', month: '2-digit', day: '2-digit' });
                            $("<div>")
                                .text(date.toISOString().split('T')[0])
                                .appendTo(container);
                        }
                    },
                ],

                onContentReady: function (e) {
                    // No need for custom loading here
                    $.getJSON("Firmware/GetData?pld=" + pld, function (data) {
                        // Assuming "gridElementId" is the ID of your DevExtreme DataGrid container
                        e.component.option("dataSource", data);
                    });
                },
                onCellPrepared: function (e) {
                    if (e.rowType == "header") {
                        e.cellElement.css("text-align", "left");
                        e.cellElement.css("font-weight", "bold");
                    }
                },
                pager: {
                    allowedPageSizes: [10, 25, 50, 100],
                    showInfo: true,
                    showNavigationButtons: true,
                    showPageSizeSelector: true,
                    visible: true,
                },
            }).dxDataGrid("instance");
            //$.ajax({
            //    url: '/SmartMeter/Firmware/CheckStatus',
            //    data: { pld: pld },
            //    contentType: 'application/html ; charset:utf-8',
            //    type: 'GET',
            //    dataType: 'html',
            //    success: function (result) {

            //    }
            //});
            console.log(meterData);
        },
    });
    $('#Cancel').click(function () {
        $("#Cancel").hide();
        $.ajax({
            url: 'Firmware/StopBackgroundFunction?pld=' + pld, // Replace with your controller action URL
            type: 'POST',
            success: function (result) {
                console.log("called");
                // Handle the success response
                // You can update the page or perform other actions here
            },
            error: function (xhr, status, error) {
                // Handle the error
            }
        });

    });
});
