$(() => {
    var pld = "";
    var aid = "";
    var eeid = "";
    var apidata = "";
    $('#SelectMeter').dxSelectBox({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/SelectSmartMeter",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        displayExpr: 'MeterID',
        valueExpr: 'MeterID',
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
            var meterData = getObjects(json, 'MeterID', data.value);
            /* var apiJson = {"idType":"PLD","id":"'" + meterData[0].PLD +"' ","transactionId":"245647898489","retentionTime":"2023-02-10","data":[{"aid":"'" + meterData[0].AID + "'","dataformat":"cp"}]};*/
            pld = meterData[0].PLD;
            aid = meterData[0].AID;
            eeid = meterData[0].EID;
            if (pld == null || aid == null) {
                DevExpress.ui.notify("AID or PID cannot be NULL check the Meter Master", 'warning', 1000);
                return false;
            }
            console.log(meterData);
        },
    });
    function updateDateTime() {
        var date = new Date();
        var formattedDateTime = formatDate(date) + ' ' + formatTime(date);
        $('#clock').val(formattedDateTime);
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


    $("#btnPresentVolume").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Present Volume" },
                success: function (result) {
                    //loadPanel.hide();
                    //$("#lblPresentVolume").val(result+" m3");
                    ////textareaLogs.option('value', result);
                    ////const myArray = result.split("/");
                    ////var data = myArray[1];
                    //DevExpress.ui.notify(result, "warning", 2000);
                    if (result != null) {

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
                        $("#lblPresentVolume").val("No Response");
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
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Volume (NVM)" },
                success: function (result) {
                    loadPanel.hide();

                    if (result != null) {
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
                        $("#lblVolumeNVM").val("No Response");
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




    $("#btnSOVOpen").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Open Valve" },
                success: function (result) {
                    loadPanel.hide();

                    if (result == "00000000") {
                        loadPanel.hide();
                        $("#lblSOVOpen").val("SOV Opened");
                    }
                    else {
                        loadPanel.hide();
                        $("#lblSOVOpen").val("No Response");
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

    $("#btnSOVClose").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Close Valve" },
                success: function (result) {
                    loadPanel.hide();

                    if (result == "00000000") {
                        loadPanel.hide();
                        $("#lblSOVClose").val("SOV Closed");
                    }
                    else {
                        loadPanel.hide();
                        $("#lblSOVClose").val("No Response");
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


    $("#btnSOVStatus").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Get Valve Position" },
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
                        $("#lblSOVStatus").val("No Response");
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



    $("#btnBatteryVoltage").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Battery Voltage" },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "error") {

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
                        $("#lblBatteryVoltage").val("No Response");
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
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Battery Life" },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "error") {

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
                        $("#lblBatteryLife").val("No Response");
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



    $("#btnTamperStatus").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: {
                    aid: aid, pld: pld, eid: eeid, eventname: "Tamper Status"
                },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "error") {

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
                        $("#lblBatteryLife").val("No Response");
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
                    aid: aid, pld: pld, eid: eeid, eventname: "Get RTC"
                },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "error") {

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
                        $("#lblGetRTC").val("No Response");
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

    // Update date and time every second
    setInterval(updateDateTime, 1000);
});