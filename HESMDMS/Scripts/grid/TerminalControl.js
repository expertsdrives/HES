$(() => {
    var pld = "";
    var aid = "";
    var eeid = "";
    var apidata = "";
    var modetype = "auto";
    function getObjects1(obj, key, val) {
        var objects = [];
        for (var i in obj) {
            if (!obj.hasOwnProperty(i)) continue;
            if (typeof obj[i] == 'object') {
                objects = objects.concat(getObjects1(obj[i], key, val));
            } else if (i == key && obj[key] == val) {
                objects.push(obj);
            }
        }
        return objects;
    }
    function convertTicksToDate(ticks) {
        // Remove non-numeric characters from the string
        var numericValue = ticks.replace(/[^0-9]/g, '');

        // Convert the numeric value to a JavaScript Date object
        var dateObject = new Date(parseInt(numericValue));
        var istOffset = 5.5 * 60 * 60 * 1000;
        // Format the date into "yyyy-mm-dd hh:mm:ss" format
        var formattedDate = dateObject.toISOString().slice(0, 19).replace('T', ' ');
        var istTime = formattedDate + istOffset;
        return formattedDate;
    } if (ro == 9) {
        $('#SelectMeter').dxSelectBox({
            dataSource: DevExpress.data.AspNet.createStore({
                key: "ID",
                loadUrl: "/SelectSmartMeter?roleid=" + ro,
                onBeforeSend: function (method, ajaxOptions) {
                    ajaxOptions.xhrFields = { withCredentials: true };
                }
            }),
            displayExpr: 'TempMeterID',
            valueExpr: 'TempMeterID',
            onValueChanged(data) {

                var json = Jsondata
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
                /* var apiJson = {"idType":"PLD","id":"'" + meterData[0].PLD +"' ","transactionId":"245647898489","retentionTime":"2023-02-10","data":[{"aid":"'" + meterData[0].AID + "'","dataformat":"cp"}]};*/
                pld = meterData[0].PLD;
                aid = meterData[0].AID;
                eeid = meterData[0].EID;
                if (pld == null || aid == null) {
                    DevExpress.ui.notify("AID or PID cannot be NULL check the Meter Master", 'warning', 1000);
                    return false;
                }
                $.ajax({
                    url: '/SmartMeter/Terminal/LoadAllBilling',
                    data: { pld: pld },
                    contentType: 'application/html ; charset:utf-8',
                    type: 'GET',
                    dataType: 'html',
                    success: function (result) {

                        const myArray = JSON.parse(result);
                        loadgetVolume(result);
                        GetRTC(result);
                        SOVPosition(result);
                        //$("#lblPresentVolume").val(parseFloat(myArray[0].Resposne.MeasurementValue).toFixed(2) + " m3");
                        $("#lblVolumeNVM").val(parseFloat(myArray[0].Resposne.TotalConsumption).toFixed(2) + " m3");
                        ValvePosition(result);
                        batteryLife(result);
                        $("#lblBatteryVoltage").val(myArray[0].Resposne.BatteryVoltage / 1000 + " V");
                        var tamper = myArray[0].Resposne.TamperEvents;
                        var substrings = tamper.match(/.{1,2}/g);
                        var titlt = substrings[0] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var caseT = substrings[1] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var managetT = substrings[2] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var ExcessPush = substrings[3] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var Excessgas = substrings[4] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var SovStuck = substrings[5] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var Invalid = substrings[6] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var main = substrings[7] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var Rf = substrings[8] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        $("#lblTilt").val(titlt);
                        $("#lblCase").val(caseT);
                        $("#lblMaganet").val(managetT);
                        $("#lblExcessPush").val(ExcessPush);
                        $("#lblExcessGas").val(Excessgas)
                        $("#lblSOVTamper").val(SovStuck);
                        $("#lblLoginError").val(Invalid);
                        $("#lblBatteryTamper").val(main);
                        $("#lblRFTamper").val(Rf);
                        //var a2 = JSON.parse(result);
                        //tbl.empty();
                        //var i = 1;
                        //$.each(a2, function (key, value) {
                        //    tbl.append('<tr>' + '<td>' + i + '</td>' + '<td>' + value.empName + '</td>' + '<td>' + value.Salary + '</td>' + '<td>' + value.DeptName + '</td>' + '<td>' + value.Designation + '</td>' + '</tr>');
                        //    i = i + 1;
                        //});
                    }
                });
                console.log(meterData);
            },
        });
    }
    else {
        $('#SelectMeter').dxSelectBox({
            dataSource: DevExpress.data.AspNet.createStore({
                key: "ID",
                loadUrl: "/SelectSmartMeter?roleid=0",
                onBeforeSend: function (method, ajaxOptions) {
                    ajaxOptions.xhrFields = { withCredentials: true };
                }
            }),
            displayExpr: 'TempMeterID',
            valueExpr: 'TempMeterID',
            onValueChanged(data) {

                var json = Jsondata
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
                /* var apiJson = {"idType":"PLD","id":"'" + meterData[0].PLD +"' ","transactionId":"245647898489","retentionTime":"2023-02-10","data":[{"aid":"'" + meterData[0].AID + "'","dataformat":"cp"}]};*/
                pld = meterData[0].PLD;
                aid = meterData[0].AID;
                eeid = meterData[0].EID;
                if (pld == null || aid == null) {
                    DevExpress.ui.notify("AID or PID cannot be NULL check the Meter Master", 'warning', 1000);
                    return false;
                }
                $.ajax({
                    url: '/SmartMeter/Terminal/LoadAllBilling',
                    data: { pld: pld },
                    contentType: 'application/html ; charset:utf-8',
                    type: 'GET',
                    dataType: 'html',
                    success: function (result) {

                        const myArray = JSON.parse(result);
                        loadgetVolume(result);
                        GetRTC(result);
                        SOVPosition(result);
                        //$("#lblPresentVolume").val(parseFloat(myArray[0].Resposne.MeasurementValue).toFixed(2) + " m3");
                        $("#lblVolumeNVM").val(parseFloat(myArray[0].Resposne.TotalConsumption).toFixed(2) + " m3");
                        ValvePosition(result);
                        batteryLife(result);
                        $("#lblBatteryVoltage").val(myArray[0].Resposne.BatteryVoltage / 1000 + " V");
                        var tamper = myArray[0].Resposne.TamperEvents;
                        var substrings = tamper.match(/.{1,2}/g);
                        var titlt = substrings[0] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var caseT = substrings[1] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var managetT = substrings[2] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var ExcessPush = substrings[3] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var Excessgas = substrings[4] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var SovStuck = substrings[5] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var Invalid = substrings[6] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var main = substrings[7] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        var Rf = substrings[8] + " (" + convertTicksToDate(myArray[0].Resposne.LogDate) + ")";
                        $("#lblTilt").val(titlt);
                        $("#lblCase").val(caseT);
                        $("#lblMaganet").val(managetT);
                        $("#lblExcessPush").val(ExcessPush);
                        $("#lblExcessGas").val(Excessgas)
                        $("#lblSOVTamper").val(SovStuck);
                        $("#lblLoginError").val(Invalid);
                        $("#lblBatteryTamper").val(main);
                        $("#lblRFTamper").val(Rf);
                        //var a2 = JSON.parse(result);
                        //tbl.empty();
                        //var i = 1;
                        //$.each(a2, function (key, value) {
                        //    tbl.append('<tr>' + '<td>' + i + '</td>' + '<td>' + value.empName + '</td>' + '<td>' + value.Salary + '</td>' + '<td>' + value.DeptName + '</td>' + '<td>' + value.Designation + '</td>' + '</tr>');
                        //    i = i + 1;
                        //});
                    }
                });
                console.log(meterData);
            },
        });
    }

    function updateDateTime() {
        var date = new Date();
        var formattedDateTime = formatDate(date) + ' ' + formatTime(date);
        $('#clock').val(formattedDateTime);
    }
    function batteryLife(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i <= data3.length; i++) {
            var d = data3[i].Data;
            if (d.split(',')[8].includes("D0")) {
                csting = d.split(',');
                data3date = convertTicksToDate(data3[i].LogDate);
                break;
            }
        }
        var balancestring = csting[13] + csting[14] + csting[15] + csting[16];
        var pendingData = getObjects1(data2, 'Status', "Pending");
        var CompletedData = getObjects1(data2, 'Status', "Completed");
        //var ResponseData = 
        //Get Balance Started
       

        $.ajax({
            url: '/SmartMeter/Terminal/BatteryLife',
            type: 'POST',
            data: { data: balancestring },
            success: function (result1) {
                loadPanel.hide();
                $("#lblBatteryLife").val(result1);
                //textareaLogs.option('value', result);
                //const myArray = result.split("/");
                //var data = myArray[1];
               
            }
        });

    }
    
    function formatDate(date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();

        // Padding single digits with leading zeros
        day = (day < 10) ? '0' + day : day;
        month = (month < 10) ? '0' + month : month;

        return day + ':' + month + ':' + year;
    }
    function loadgetVolume(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i <= data3.length; i++) {
            var d = data3[i].Data;
            if (d.split(',')[8].includes("EC")) {
                csting = d.split(',');
                data3date = convertTicksToDate(data3[i].LogDate);
                break;
            }
        }
        var balancestring = csting[9] + csting[10] + csting[11] + csting[12];
        var pendingData = getObjects1(data2, 'Status', "Pending");
        var CompletedData = getObjects1(data2, 'Status', "Completed");
        //var ResponseData = 
        //Get Balance Started
        var CompletedGetBalance = getObjects1(CompletedData, 'EventName', "Present Volume");
        var PendingGetBalance = getObjects1(pendingData, 'EventName', "Present Volume");

        var date1 = convertTicksToDate(data1.LogDate);
        var date2 = CompletedGetBalance.length > 0 ? convertTicksToDate(CompletedGetBalance[0].LogDate) : "";
        var date3 = PendingGetBalance.length > 0 ? convertTicksToDate(PendingGetBalance[0].LogDate) : "";

        if (date2 > date3) {
            $.ajax({
                url: '/SmartMeter/Terminal/StringToHex',
                type: 'POST',
                data: { data: balancestring },
                success: function (result1) {
                    loadPanel.hide();
                    $("#lblPresentVolume").val(result1 + " m3");
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];
                    //DevExpress.ui.notify(result, "warning", 2000);
                }
            });
        } else if (date3 > date2) {
            // date3 is greater
            $("#lblPresentVolume").val("Command In Queue");
        }

    }

    function GetRTC(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i <= data3.length; i++) {
            var d = data3[i].Data;
            if (d.split(',')[8].includes("1D")) {
                csting = d.split(',');
                data3date = convertTicksToDate(data3[i].LogDate);
                break;
            }
        }
       
        var balancestring = csting[13] + csting[14] + csting[15] + csting[16] + csting[17] + csting[18] + csting[19] + csting[20] + csting[21] + csting[22] + csting[23] + csting[24];
        var pendingData = getObjects1(data2, 'Status', "Pending");
        var CompletedData = getObjects1(data2, 'Status', "Completed");
        //var ResponseData = 
        //Get Balance Started


        $.ajax({
            url: '/SmartMeter/Terminal/GetRTC',
            type: 'POST',
            data: { data: balancestring.replace(",", "") },
            success: function (result1) {
                loadPanel.hide();
                $("#lblGetRTC").val(result1);
                //textareaLogs.option('value', result);
                //const myArray = result.split("/");
                //var data = myArray[1];

            }
        });

    }


    function SOVPosition(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i <= data3.length; i++) {
            var d = data3[i].Data;
            if (d.split(',')[8].includes("D4")) {
                csting = d.split(',');
                data3date = convertTicksToDate(data3[i].LogDate);
                break;
            }
        }
       
        var balancestring = csting[9];
        var pendingData = getObjects1(data2, 'Status', "Pending");
        var CompletedData = getObjects1(data2, 'Status', "Completed");
        //var ResponseData = 
        //Get Balance Started
        $("#lblLive").val(balancestring == "AA" ? "Stuck" : balancestring == "BB" ? "Open" : "Close");



    }
    function ValvePosition(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i <= data3.length; i++) {
            var d = data3[i].Data;
            if (d.split(',')[8].includes("07")) {
                csting = d.split(',');
                data3date = convertTicksToDate(data3[i].LogDate);
                break;
            }
        }
 
        var balancestring = csting[13];
        var pendingData = getObjects1(data2, 'Status', "Pending");
        var CompletedData = getObjects1(data2, 'Status', "Completed");
        //var ResponseData = 
        //Get Balance Started
        $("#lblSOVStatus").val(balancestring == "AA" ? "Stuck" : balancestring == "00" ? "Open" : "Close");



    }
    function formatTime(date) {
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();

        // Padding single digits with leading zeros
        hours = (hours < 10) ? '0' + hours : hours;
        minutes = (minutes < 10) ? '0' + minutes : minutes;
        seconds = (seconds < 10) ? '0' + seconds : seconds;

        return hours + ':' + minutes + ':' + seconds;
    }
    const loadPanel = $('.loadpanel').dxLoadPanel({
        shadingColor: 'rgba(0,0,0,0.4)',
        position: { of: '.container' },
        visible: false,
        showIndicator: true,
        showPane: true,
        shading: true,
        hideOnOutsideClick: false,
        onShown() {
        },
        onHidden() {
            /* showEmployeeInfo(employee);*/
        },
    }).dxLoadPanel('instance');

    $("#chkb").change(function () {
        if ($(this).is(":checked")) {
            modetype = "manual";
            //console.log("auto");
            // Perform actions when checkbox is checked
        } else {
            modetype = "auto";
            //console.log("manual");
            // Perform actions when checkbox is unchecked
        }
    });
    $("#btnPresentVolume").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Present Volume", modetype: modetype, balanceInput: "" },
                success: function (result) {
                    //loadPanel.hide();
                    //$("#lblPresentVolume").val(result+" m3");
                    ////textareaLogs.option('value', result);
                    ////const myArray = result.split("/");
                    ////var data = myArray[1];
                    //DevExpress.ui.notify(result, "warning", 2000);
                    if (result != "error" && result != "Command Added In Queue") {

                        $.ajax({
                            url: '/SmartMeter/Terminal/StringToHex',
                            type: 'POST',
                            data: { data: result },
                            success: function (result1) {
                                loadPanel.hide();
                                $("#lblPresentVolume").val(result1 + " m3");
                                //textareaLogs.option('value', result);
                                //const myArray = result.split("/");
                                //var data = myArray[1];
                                DevExpress.ui.notify(result, "warning", 2000);
                            }
                        });
                    }
                    else {
                        loadPanel.hide();
                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#lblPresentVolume").val("Command Added In Queue");
                        }
                    }
                }
            });
        }
        else {
            alert("Please Select Meter");
        }


    });



    $("#btnVolumeNVM").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Volume (NVM)", modetype: modetype, balanceInput: "" },
                success: function (result) {
                    loadPanel.hide();
                    if (result != "error" && result != "Command Added In Queue") {
                        $.ajax({
                            url: '/SmartMeter/Terminal/StringToHexNVE',
                            type: 'POST',
                            data: { data: result },
                            success: function (result1) {
                                loadPanel.hide();
                                $("#lblVolumeNVM").val(result1 + " m3");
                                //textareaLogs.option('value', result);
                                //const myArray = result.split("/");
                                //var data = myArray[1];
                                DevExpress.ui.notify(result, "warning", 2000);
                            }
                        });
                    }
                    else {
                        loadPanel.hide();

                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#lblVolumeNVM").val("Command Added In Queue");
                        }
                    }
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];

                }
            });
        }
        else {
            alert("Please Select Meter");
        }
    });




    $("#btnSOVOpen").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Open Valve", modetype: modetype, balanceInput: "" },
                success: function (result) {
                    loadPanel.hide();

                    if (result == "00000000") {
                        loadPanel.hide();
                        $("#lblSOVOpen").val("SOV Opened");
                    }
                    else {
                        loadPanel.hide();
                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#lblSOVOpen").val("Command Added In Queue");
                        }
                    }
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];

                }
            });
        }
        else {
            alert("Please Select Meter");
        }
    });

    $("#btnSOVClose").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Close Valve", modetype: modetype, balanceInput: "" },
                success: function (result) {
                    loadPanel.hide();

                    if (result == "00000000") {
                        loadPanel.hide();
                        $("#lblSOVClose").val("SOV Closed");
                    }
                    else {
                        loadPanel.hide();
                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#lblSOVClose").val("Command Added In Queue");
                        }
                    }
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];

                }
            });
        }
        else {
            alert("Please Select Meter");
        }
    });


    $("#btnSOVStatus").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Get Valve Position", modetype: modetype, balanceInput: "" },
                success: function (result) {
                    loadPanel.hide();

                    if (result == "01") {
                        loadPanel.hide();
                        $("#lblSOVStatus").val("SOV Closed");
                    }
                    else if (result == "00") {
                        loadPanel.hide();
                        $("#lblSOVStatus").val("SOV Open");
                    }
                    else {
                        loadPanel.hide();
                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#lblSOVStatus").val("Command Added In Queue");
                        }
                    }
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];

                }
            });
        } else {
            alert("Please Select Meter");
        }
    });



    $("#btnBatteryVoltage").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Battery Voltage", modetype: modetype, balanceInput: "" },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "Command Added In Queue") {

                        $.ajax({
                            url: '/SmartMeter/Terminal/VolategCalc',
                            type: 'POST',
                            data: { data: result },
                            success: function (result1) {
                                loadPanel.hide();
                                $("#lblBatteryVoltage").val(result1 + " Volts");
                                //textareaLogs.option('value', result);
                                //const myArray = result.split("/");
                                //var data = myArray[1];
                                DevExpress.ui.notify(result, "warning", 2000);
                            }
                        });
                    }
                    else {
                        loadPanel.hide();
                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#lblBatteryVoltage").val("Command Added In Queue");
                        }
                    }
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];
                    DevExpress.ui.notify(result, "warning", 2000);
                }
            });
        }
        else {
            alert("Please Select Meter");
        }
    });


    $("#btnBatteryLife").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Battery Life", modetype: modetype, balanceInput: "" },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "Command Added In Queue") {

                        $.ajax({
                            url: '/SmartMeter/Terminal/BatteryLife',
                            type: 'POST',
                            data: { data: result },
                            success: function (result1) {
                                loadPanel.hide();
                                $("#lblBatteryLife").val(result1);
                                //textareaLogs.option('value', result);
                                //const myArray = result.split("/");
                                //var data = myArray[1];
                                DevExpress.ui.notify(result, "warning", 2000);
                            }
                        });
                    }
                    else {
                        loadPanel.hide();
                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#lblBatteryLife").val("Command Added In Queue");
                        }
                    }
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];
                    DevExpress.ui.notify(result, "warning", 2000);
                }
            });
        }
        else {
            alert("Please Select Meter");
        }
    });

    $("#btnTamperReset").click(function () {
        var checkedNames = [];
        $('#TamperCheck:checked').each(function () {
            checkedNames.push($(this).attr('name'));
        });
        var TamperSelected = checkedNames.join(',');
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: {
                    aid: aid, pld: pld, eid: eeid, eventname: TamperSelected, modetype: modetype, balanceInput: ""
                },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "Command Added In Queue") {

                        $.ajax({
                            url: '/SmartMeter/Terminal/TamperStatus',
                            type: 'POST',
                            data: { data: result },
                            success: function (result1) {
                                loadPanel.hide();
                                var output = result1.split(",");
                                $("#lblTilt").val(output[0]);
                                $("#lblCase").val(output[1]);
                                $("#lblMaganet").val(output[2]);
                                $("#lblExcessPush").val(output[3]);
                                $("#lblExcessGas").val(output[4]);
                                $("#lblSOVTamper").val(output[5]);
                                $("#lblLoginError").val(output[6]);
                                $("#lblBatteryTamper").val(output[7]);
                                $("#lblRFTamper").val(output[8]);

                                //textareaLogs.option('value', result);
                                //const myArray = result.split("/");
                                //var data = myArray[1];
                                DevExpress.ui.notify(result, "warning", 2000);
                            }
                        });
                    }
                    else {
                        loadPanel.hide();
                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#TamperStatuslabel").text("Command Added In Queue");
                        }
                    }
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];
                    DevExpress.ui.notify(result, "warning", 2000);
                }
            });
        } else {
            alert("Please Select Meter");
        }
    });

    $("#btnTamperStatus").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: {
                    aid: aid, pld: pld, eid: eeid, eventname: "Tamper Status", modetype: modetype, balanceInput: ""
                },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "Command Added In Queue") {

                        $.ajax({
                            url: '/SmartMeter/Terminal/TamperStatus',
                            type: 'POST',
                            data: { data: result },
                            success: function (result1) {
                                loadPanel.hide();
                                var output = result1.split(",");
                                $("#lblTilt").val(output[0]);
                                $("#lblCase").val(output[1]);
                                $("#lblMaganet").val(output[2]);
                                $("#lblExcessPush").val(output[3]);
                                $("#lblExcessGas").val(output[4]);
                                $("#lblSOVTamper").val(output[5]);
                                $("#lblLoginError").val(output[6]);
                                $("#lblBatteryTamper").val(output[7]);
                                $("#lblRFTamper").val(output[8]);

                                //textareaLogs.option('value', result);
                                //const myArray = result.split("/");
                                //var data = myArray[1];
                                DevExpress.ui.notify(result, "warning", 2000);
                            }
                        });
                    }
                    else {
                        loadPanel.hide();
                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#TamperStatuslabel").text("Command Added In Queue");
                        }
                    }
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];
                    DevExpress.ui.notify(result, "warning", 2000);
                }
            });
        } else {
            alert("Please Select Meter");
        }
    });


    $("#btnGetRTC").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: {
                    aid: aid, pld: pld, eid: eeid, eventname: "Get RTC", modetype: modetype, balanceInput: ""
                },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "Command Added In Queue") {

                        $.ajax({
                            url: '/SmartMeter/Terminal/GetRTC',
                            type: 'POST',
                            data: { data: result },
                            success: function (result1) {
                                loadPanel.hide();
                                $("#lblGetRTC").val(result1);
                                //textareaLogs.option('value', result);
                                //const myArray = result.split("/");
                                //var data = myArray[1];
                                DevExpress.ui.notify(result, "warning", 2000);
                            }
                        });
                    }
                    else {
                        loadPanel.hide();
                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#lblGetRTC").val("Command Added In Queue");
                        }
                    }
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];
                    DevExpress.ui.notify(result, "warning", 2000);
                }
            });
        }
        else {
            alert("Please Select Meter");
        }
    });
    $("#btnSovCalibration").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: {
                    aid: aid, pld: pld, eid: eeid, eventname: "SOV Calibration", modetype: modetype, balanceInput: ""
                },
                success: function (result) {
                    loadPanel.hide();

                    if (result == "00000000") {
                        loadPanel.hide();
                        $("#lblSovCalb").val("SOV Calibrated");
                    }
                    else {
                        loadPanel.hide();
                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#lblSovCalb").val("Command Added In Queue");
                        }
                    }
                }
            });
        }
        else {
            alert("Please Select Meter");
        }
    }); $("#btnSovCalibration").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: {
                    aid: aid, pld: pld, eid: eeid, eventname: "SOV Calibration", modetype: modetype, balanceInput: ""
                },
                success: function (result) {
                    loadPanel.hide();

                    if (result == "00000000") {
                        loadPanel.hide();
                        $("#lblSovCalb").val("SOV Calibrated");
                    }
                    else {
                        loadPanel.hide();
                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#lblSovCalb").val("Command Added In Queue");
                        }
                    }
                }
            });
        }
        else {
            alert("Please Select Meter");
        }
    });

    $("#btnSovLive").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: {
                    aid: aid, pld: pld, eid: eeid, eventname: "SOV LiVe Position Read", modetype: modetype, balanceInput: ""
                },
                success: function (result) {
                    loadPanel.hide();

                    if (result == "AA") {
                        loadPanel.hide();
                        $("#lblSovCalb").val("SOV Stuck");
                    }
                    if (result == "BB") {
                        loadPanel.hide();
                        $("#lblSovCalb").val("SOV Open");
                    }
                    if (result == "CC") {
                        loadPanel.hide();
                        $("#lblSovCalb").val("SOV Close");
                    }
                    else {
                        loadPanel.hide();
                        if (result == "Manual Mode Not Activated") {
                            alert("Manual Mode Not Activated")
                        }
                        else {
                            $("#lblSovCalb").val("Command Added In Queue");
                        }
                    }
                }
            });
        }
        else {
            alert("Please Select Meter");
        }
    });



    // Update date and time every second
    setInterval(updateDateTime, 1000);
});