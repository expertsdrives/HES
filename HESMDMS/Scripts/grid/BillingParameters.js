$(() => {
    var pld = "";
    var aid = "";
    var eeid = "";
    var apidata = "";
    var mid = "";
    var modetype = "auto";
    function convertHexToDouble(hexValue) {
        var longValue = BigInt("0x" + hexValue);
        var buffer = new ArrayBuffer(8);
        var view = new DataView(buffer);
        view.setBigUint64(0, longValue);
        var doubleValue = view.getFloat64(0);
        var bal = doubleValue.toFixed(2);
        return bal;
    }
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
    function loadgetbal(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i <= data3.length; i++) {
            var d = data3[i].Data;
            if (d.split(',')[8].includes("EB")) {
                csting = d.split(',');
                data3date = convertTicksToDate(data3[i].LogDate);
                break;
            }
        }
        var balancestring = csting[9] + csting[10] + csting[11] + csting[12] + csting[13] + csting[14] + csting[15] + csting[16];
        var pendingData = getObjects1(data2, 'Status', "Pending");
        var CompletedData = getObjects1(data2, 'Status', "Completed");
        //var ResponseData = 
        //Get Balance Started
        var CompletedGetBalance = getObjects1(CompletedData, 'EventName', "Get Balance");
        var PendingGetBalance = getObjects1(pendingData, 'EventName', "Get Balance");

        var date1 = convertTicksToDate(data1.LogDate);
        var date2 = CompletedGetBalance.length > 0 ? convertTicksToDate(CompletedGetBalance[0].LogDate) : "";
        var date3 = PendingGetBalance.length > 0 ? convertTicksToDate(PendingGetBalance[0].LogDate) : "";

        if (date1 > date2 && date1 > date3) {
            $("#lblGetBalance").val("Rs. " + data1.AccountBalance);
        } else if (date2 > date1 && date2 > date3) {
            $.ajax({
                url: '/SmartMeter/Terminal/GetBalance',
                type: 'POST',
                data: { data: balancestring },
                success: function (result1) {

                    $("#lblGetBalance").val("Rs " + result1);
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];

                }
            });
        } else if (date3 > date1 && date3 > date2) {
            // date3 is greater
            $("#lblGetBalance").val("Command In Queue");
        }

    }


    function loadgetvat(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i <= data3.length; i++) {
            var d = data3[i].Data;
            if (d.split(',')[8].includes("35")) {
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
        var CompletedGetBalance = getObjects1(CompletedData, 'EventName', "Get Vat");
        var PendingGetBalance = getObjects1(pendingData, 'EventName', "Get Vat");

        var date1 = convertTicksToDate(data1.LogDate);
        var date2 = CompletedGetBalance.length > 0 ? convertTicksToDate(CompletedGetBalance[0].LogDate) : "";
        var date3 = PendingGetBalance.length > 0 ? convertTicksToDate(PendingGetBalance[0].LogDate) : "";

        if (date2 > date3) {
            $.ajax({
                url: '/SmartMeter/Terminal/GetVat',
                type: 'POST',
                data: { data: balancestring },
                success: function (result1) {
                    loadPanel.hide();
                    $("#lblGetVat").val(result1 + "%");
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];

                }
            });
        } else if (date3 > date2) {
            // date3 is greater
            $("#lblGetVat").val("Command In Queue");
        }

    }


    function loadGetTariff(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i <= data3.length; i++) {
            var d = data3[i].Data;
            if (d.split(',')[8].includes("2B")) {
                csting = d.split(',');
                data3date = convertTicksToDate(data3[i].LogDate);
                break;
            }
        }
        var balancestring = csting[14] + csting[15] + csting[16] + csting[17];
        var pendingData = getObjects1(data2, 'Status', "Pending");
        var CompletedData = getObjects1(data2, 'Status', "Completed");
        //var ResponseData = 
        //Get Balance Started
        var CompletedGetBalance = getObjects1(CompletedData, 'EventName', "Get Tariff");
        var PendingGetBalance = getObjects1(pendingData, 'EventName', "Get Tariff");

        var date1 = convertTicksToDate(data1.LogDate);
        var date2 = CompletedGetBalance.length > 0 ? convertTicksToDate(CompletedGetBalance[0].LogDate) : "";
        var date3 = PendingGetBalance.length > 0 ? convertTicksToDate(PendingGetBalance[0].LogDate) : "";

        if (date1 > date2 && date1 > date3) {
            $("#lblGetTariff").val("Rs. " + data1.StandardCharge);
        } else if (date2 > date1 && date2 > date3) {
            $.ajax({
                url: '/SmartMeter/Terminal/GetTariff',
                type: 'POST',
                data: { data: balancestring },
                success: function (result1) {
                    loadPanel.hide();
                    $("#lblGetTariff").val("Rs " + result1);
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];

                }
            });
        } else if (date3 > date1 && date3 > date2) {
            // date3 is greater
            $("#lblGetTariff").val("Command In Queue");
        }

    }



    function loadsetbal(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i <= data3.length; i++) {
            var d = data3[i].Data;
            if (d.includes("29")) {
                csting = d.split(',');
                data3date = convertTicksToDate(data3[i].LogDate);
                break;
            }
        }
        var balancestring = csting[9] + csting[10] + csting[11] + csting[12] + csting[13] + csting[14] + csting[15] + csting[16];
        var pendingData = getObjects1(data2, 'Status', "Pending");
        var CompletedData = getObjects1(data2, 'Status', "Completed");
        //var ResponseData = 
        //Get Balance Started
        var CompletedGetBalance = getObjects1(CompletedData, 'EventName', "Add Balance");
        var PendingGetBalance = getObjects1(pendingData, 'EventName', "Add Balance");

        var date1 = convertTicksToDate(data1.LogDate);
        var date2 = CompletedGetBalance.length > 0 ? convertTicksToDate(CompletedGetBalance[0].LogDate) : "";
        var date3 = PendingGetBalance.length > 0 ? convertTicksToDate(PendingGetBalance[0].LogDate) : "";

        if (date2 > date3) {
            $("#lblAddBalanceStatus").text("Delivered (" + date2 + ")");
        } else if (date3 > date2) {
            // date3 is greater
            $("#lblAddBalanceStatus").text("Command In Queue");
        }

    }




    function loadsetVat(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i <= data3.length; i++) {
            var d = data3[i].Data;
            if (d.split(',')[8].includes("37")) {
                csting = d.split(',');
                data3date = convertTicksToDate(data3[i].LogDate);
                break;
            }
        }
        var balancestring = csting[9] + csting[10] + csting[11] + csting[12] + csting[13] + csting[14] + csting[15] + csting[16];
        var pendingData = getObjects1(data2, 'Status', "Pending");
        var CompletedData = getObjects1(data2, 'Status', "Completed");
        //var ResponseData = 
        //Get Balance Started
        var CompletedGetBalance = getObjects1(CompletedData, 'EventName', "Set Vat");
        var PendingGetBalance = getObjects1(pendingData, 'EventName', "Set Vat");

        var date1 = convertTicksToDate(data1.LogDate);
        var date2 = CompletedGetBalance.length > 0 ? convertTicksToDate(CompletedGetBalance[0].LogDate) : "";
        var date3 = PendingGetBalance.length > 0 ? convertTicksToDate(PendingGetBalance[0].LogDate) : "";

        if (date2 > date3) {
            $("#lblSetVatStatus").text("Delivered (" + date2 + ")");
        } else if (date3 > date2) {
            // date3 is greater
            $("#lblSetVatStatus").text("Command In Queue");
        }

    }





    function loadGetKCal(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i < data3.length; i++) {
            console.log(data3);
            var d = data3[i].Data;

            if (d.split(',')[8].includes("EE")) {
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
        var CompletedGetBalance = getObjects1(CompletedData, 'EventName', "Get Average Gas Calorific Value");
        var PendingGetBalance = getObjects1(pendingData, 'EventName', "Get Average Gas Calorific Value");

        var date1 = convertTicksToDate(data1.LogDate);
        var date2 = CompletedGetBalance.length > 0 ? convertTicksToDate(CompletedGetBalance[0].LogDate) : "";
        var date3 = PendingGetBalance.length > 0 ? convertTicksToDate(PendingGetBalance[0].LogDate) : "";

        if (date1 > date2 && date1 > date3) {
            $("#lblGetAvgGasCal").val("Rs. " + data1.GasCalorific);
        } else if (date2 > date1 && date2 > date3) {
            $.ajax({
                url: '/SmartMeter/Terminal/GetCal',
                type: 'POST',
                data: { data: balancestring },
                success: function (result1) {
                    $("#lblGetAvgGasCal").val("Rs " + result1);
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];

                }
            });
        } else if (date3 > date1 && date3 > date2) {
            // date3 is greater
            $("#lblGetAvgGasCal").val("Command In Queue");
        }

    }




    function loadKCAL(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i < data3.length; i++) {
            var d = data3[i].Data;
            if (d.split(',')[8].includes("E6")) {
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
        var CompletedGetBalance = getObjects1(CompletedData, 'EventName', "Set Average Gas Calorific Value");
        var PendingGetBalance = getObjects1(pendingData, 'EventName', "Set Average Gas Calorific Value");

        var date1 = convertTicksToDate(data1.LogDate);
        var date2 = CompletedGetBalance.length > 0 ? convertTicksToDate(CompletedGetBalance[0].LogDate) : "";
        var date3 = PendingGetBalance.length > 0 ? convertTicksToDate(PendingGetBalance[0].LogDate) : "";

        if (date2 > date3) {
            $("#lblSKCALStatus").text("Delivered (" + date2 + ")");
        } else if (date3 > date2) {
            // date3 is greater
            $("#lblSKCALStatus").text("Command In Queue");
        }

    }


    function loadgetEbal(result) {
        var i = 0;
        var csting = "";
        var data3date = "";
        const myArray = JSON.parse(result);
        var data1 = myArray[0].Resposne;
        var data2 = myArray[1].Resposne1;
        var data3 = myArray[2].CommandResponse;
        for (i = 0; i <= data3.length; i++) {
            var d = data3[i].Data;
            if (d.split(',')[8].includes("F2")) {
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
        var CompletedGetBalance = getObjects1(CompletedData, 'EventName', "Read E-Credit Threshold");
        var PendingGetBalance = getObjects1(pendingData, 'EventName', "Read E-Credit Threshold");

        var date1 = convertTicksToDate(data1.LogDate);
        var date2 = CompletedGetBalance.length > 0 ? convertTicksToDate(CompletedGetBalance[0].LogDate) : "";
        var date3 = PendingGetBalance.length > 0 ? convertTicksToDate(PendingGetBalance[0].LogDate) : "";

        if (date1 > date2 && date1 > date3) {
            $("#lblReadECredit").val("Rs. " + data1.eCreditBalance);
        } else if (date2 > date1 && date2 > date3) {
            $.ajax({
                url: '/SmartMeter/Terminal/GetCal',
                type: 'POST',
                data: { data: balancestring },
                success: function (result1) {
                    loadPanel.hide();
                    $("#lblReadECredit").val("Rs " + result1);
                    //textareaLogs.option('value', result);
                    //const myArray = result.split("/");
                    //var data = myArray[1];

                }
            });
        } else if (date3 > date1 && date3 > date2) {
            // date3 is greater
            $("#lblReadECredit").val("Command In Queue");
        }

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
    }
    $('#SelectMeter1').dxSelectBox({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/SelectSmartMeter",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        displayExpr: 'TempMeterID',
        valueExpr: 'TempMeterID',
        value: mid,
        onValueChanged: function (data) {
            $("#mtrRefresh").attr("href", "/Smartmeter/Terminal/BillingParameters?mid=" + data.value);
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
            aid = meterData[0].AID;
            eeid = meterData[0].EID;
            if (pld == null || aid == null) {
                DevExpress.ui.notify("AID or PID cannot be NULL check the Meter Master", 'warning', 1000);
                return false;
            }

            $.ajax({
                url: '/SmartMeter/Terminal/LoadAllBilling',
                data: { pld: pld, mid: mid },
                contentType: 'application/html ; charset:utf-8',
                type: 'GET',
                dataType: 'html',
                success: function (result) {
                    loadgetbal(result);
                    loadsetbal(result);
                    loadgetvat(result);
                    loadsetVat(result);
                    loadGetTariff(result);
                    loadGetKCal(result);
                    loadKCAL(result);
                    loadgetEbal(result);
                }
            });
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

    $("#btnAddBalance").click(function () {
        if (pld != "") {

            var balanceInput = $('#lblAddBalance').val();
            if (balanceInput != "") {
                loadPanel.show();
                $.ajax({
                    url: '/SmartMeter/Terminal/SendData',
                    type: 'POST',
                    data: { aid: aid, pld: pld, eid: eeid, eventname: "Add Balance", modetype: modetype, balanceInput: balanceInput },
                    success: function (result) {
                        //loadPanel.hide();
                        //$("#lblPresentVolume").val(result+" m3");
                        ////textareaLogs.option('value', result);
                        ////const myArray = result.split("/");
                        ////var data = myArray[1];
                        //DevExpress.ui.notify(result, "warning", 2000);
                        if (result == "00000000") {
                            loadPanel.hide();
                            alert("Balance Added");
                        }
                        else {
                            loadPanel.hide();
                            $("#lblAddBalanceStatus").text("");
                            $("#lblAddBalance").val(result);
                        }
                    }
                });
            }
            else {
                alert("Kindly Input Balance to Proceed");
            }
        }
        else {
            alert("Please Select Meter");
        }


    });


    $("#btnGetBalance").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Get Balance", modetype: modetype, balanceInput: "" },
                success: function (result) {
                    loadPanel.hide();

                    if (result == "error") {
                        loadPanel.hide();
                        $("#lblGetBalance").val("No Response");

                    }
                    else if (result == "No data found") {
                        loadPanel.hide();
                        $("#lblGetBalance").val("No data found");
                    }
                    else {
                        if (result == "Command Added In Queue") {
                            loadPanel.hide();
                            $("#lblGetBalance").val(result);
                        }
                        //if (modetype == "auto") {
                        //    loadPanel.hide();
                        //    $("#lblGetBalance").val(result);
                        //}
                        /*  else {*/
                        else {
                            $.ajax({
                                url: '/SmartMeter/Terminal/GetBalance',
                                type: 'POST',
                                data: { data: result },
                                success: function (result1) {
                                    loadPanel.hide();
                                    $("#lblGetBalance").val("Rs " + result1);
                                    //textareaLogs.option('value', result);
                                    //const myArray = result.split("/");
                                    //var data = myArray[1];

                                }
                            });
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


    $("#btnSetVAT").click(function () {
        if (pld != "") {

            var balanceInput = $('#lblSetVAT').val();
            if (balanceInput != "") {
                loadPanel.show();
                $.ajax({
                    url: '/SmartMeter/Terminal/SendData',
                    type: 'POST',
                    data: { aid: aid, pld: pld, eid: eeid, eventname: "Set Vat", modetype: modetype, balanceInput: balanceInput },
                    success: function (result) {
                        //loadPanel.hide();
                        //$("#lblPresentVolume").val(result+" m3");
                        ////textareaLogs.option('value', result);
                        ////const myArray = result.split("/");
                        ////var data = myArray[1];
                        //DevExpress.ui.notify(result, "warning", 2000);
                        if (result == "00000000") {
                            loadPanel.hide();
                            alert("Vat Changed");
                        }
                        else {
                            loadPanel.hide();
                            $("#lblSetVatStatus").text("");
                            $("#lblSetVAT").val(result);
                        }
                    }
                });
            }
            else {
                alert("Kindly Input VAT to Proceed");
            }
        }
        else {
            alert("Please Select Meter");
        }


    });





    $("#btnGetVat").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Get Vat", modetype: modetype, balanceInput: "" },
                success: function (result) {
                    loadPanel.hide();

                    if (result == "error") {
                        loadPanel.hide();
                        $("#lblGetVat").val("No Response");
                    }
                    else if (result == "Command Added In Queue") {
                        loadPanel.hide();
                        $("#lblGetVat").val("Command Added In Queue");
                    }
                    else {
                        $.ajax({
                            url: '/SmartMeter/Terminal/GetVat',
                            type: 'POST',
                            data: { data: result },
                            success: function (result1) {
                                loadPanel.hide();
                                $("#lblGetVat").val(result1 + "%");
                                //textareaLogs.option('value', result);
                                //const myArray = result.split("/");
                                //var data = myArray[1];

                            }
                        });

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



    $("#btnSetTraiff").click(function () {
        if (pld != "") {

            var date = $('#lblTraiffDate').val();
            var time = $('#lblTraiffTime').val();
            var Tariff = $('#lblTraiffAmount').val();
            if (Tariff != "") {
                loadPanel.show();
                $.ajax({
                    url: '/SmartMeter/Terminal/SendDataAddTariff',
                    type: 'POST',
                    data: { aid: aid, pld: pld, eid: eeid, eventname: "Set Tariff", date: date, time: time, Tariff: Tariff },
                    success: function (result) {
                        //loadPanel.hide();
                        //$("#lblPresentVolume").val(result+" m3");
                        ////textareaLogs.option('value', result);
                        ////const myArray = result.split("/");
                        ////var data = myArray[1];
                        //DevExpress.ui.notify(result, "warning", 2000);
                        if (result == "00000000") {
                            loadPanel.hide();
                            alert("Tariff Added");
                        }
                        else {
                            loadPanel.hide();
                            $("#lblTraiffAmount").val(result);
                        }
                    }
                });
            }
            else {
                alert("Kindly Input Tariff to Proceed");
            }
        }
        else {
            alert("Please Select Meter");
        }


    });





    $("#btnGetTariff").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Get Tariff", modetype: modetype, balanceInput: "" },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "Command Added In Queue") {
                        $.ajax({
                            url: '/SmartMeter/Terminal/GetTariff',
                            type: 'POST',
                            data: { data: result },
                            success: function (result1) {
                                loadPanel.hide();
                                $("#lblGetTariff").val("Rs " + result1);
                                //textareaLogs.option('value', result);
                                //const myArray = result.split("/");
                                //var data = myArray[1];

                            }
                        });
                    }
                    else {
                        loadPanel.hide();
                        $("#lblGetTariff").val("Command Added In Queue");
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
    $("#btnSetAvgGasCal").click(function () {
        if (pld != "") {

            var balanceInput = $('#lblSetAvgGasCal').val();
            if (balanceInput != "") {
                loadPanel.show();
                $.ajax({
                    url: '/SmartMeter/Terminal/SendData',
                    type: 'POST',
                    data: { aid: aid, pld: pld, eid: eeid, eventname: "Set Average Gas Calorific Value", modetype: modetype, balanceInput: balanceInput },
                    success: function (result) {
                        //loadPanel.hide();
                        //$("#lblPresentVolume").val(result+" m3");
                        ////textareaLogs.option('value', result);
                        ////const myArray = result.split("/");
                        ////var data = myArray[1];
                        //DevExpress.ui.notify(result, "warning", 2000);
                        if (result == "FFFFFFFF") {
                            loadPanel.hide();
                            $("#lblSetAvgGasCal").val("Value out of Limit");
                        }
                        else if (result == "00000000") {
                            loadPanel.hide();
                            $("#lblSKCALStatus").text("");
                            $("#lblSetAvgGasCal").val("Gas Calorific Value Added");
                        }
                        else {
                            loadPanel.hide();
                            $("#lblSKCALStatus").text("");
                            $("#lblSetAvgGasCal").val(result);
                        }
                    }
                });
            }
            else {
                alert("Kindly Input KCAL/SCM to Proceed");
            }
        }
        else {
            alert("Please Select Meter");
        }


    });




    $("#btnGetAvgGasCal").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Get Average Gas Calorific Value", modetype: modetype, balanceInput: "" },
                success: function (result) {
                    loadPanel.hide();

                    if (result == "error") {
                        loadPanel.hide();
                        $("#lblGetAvgGasCal").val("No Response");

                    }
                    else if (result == "No data found") {
                        loadPanel.hide();
                        $("#lblGetAvgGasCal").val("No data found");
                    }
                    else {
                        if (result == "Command Added In Queue") {
                            loadPanel.hide();
                            $("#lblGetAvgGasCal").val(result);
                        }
                        //if (modetype == "auto") {
                        //    loadPanel.hide();
                        //    $("#lblGetBalance").val(result);
                        //}
                        else {
                            $.ajax({
                                url: '/SmartMeter/Terminal/GetCal',
                                type: 'POST',
                                data: { data: result },
                                success: function (result1) {
                                    loadPanel.hide();
                                    $("#lblGetAvgGasCal").val("Rs " + result1);
                                    //textareaLogs.option('value', result);
                                    //const myArray = result.split("/");
                                    //var data = myArray[1];

                                }
                            });
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




    $("#btnSetECredit").click(function () {
        if (pld != "") {

            var balanceInput = $('#lblSetECredit').val();
            if (balanceInput != "") {
                loadPanel.show();
                $.ajax({
                    url: '/SmartMeter/Terminal/SendData',
                    type: 'POST',
                    data: { aid: aid, pld: pld, eid: eeid, eventname: "Set E-Credit Threshold", modetype: modetype, balanceInput: balanceInput },
                    success: function (result) {
                        //loadPanel.hide();
                        //$("#lblPresentVolume").val(result+" m3");
                        ////textareaLogs.option('value', result);
                        ////const myArray = result.split("/");
                        ////var data = myArray[1];
                        //DevExpress.ui.notify(result, "warning", 2000);
                        if (result == "00000000") {
                            loadPanel.hide();
                            $("#lblSetECredit").val("E-Credit Threshold Updated");
                        }
                        else {
                            loadPanel.hide();
                            $("#lblSetECredit").val(result);
                        }
                    }
                });
            }
            else {
                alert("Kindly Input E-Credit to Proceed");
            }
        }
        else {
            alert("Please Select Meter");
        }


    });



    $("#btnReadECredit").click(function () {
        if (pld != "") {
            loadPanel.show();
            $.ajax({
                url: '/SmartMeter/Terminal/SendData',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Read E-Credit Threshold", modetype: modetype, balanceInput: "" },
                success: function (result) {
                    loadPanel.hide();

                    if (result == "error") {
                        loadPanel.hide();
                        $("#lblReadECredit").val("No Response");

                    }
                    else if (result == "No data found") {
                        loadPanel.hide();
                        $("#lblReadECredit").val("No data found");
                    }
                    else {
                        if (result == "Command Added In Queue") {
                            loadPanel.hide();
                            $("#lblReadECredit").val(result);
                        }
                        //if (modetype == "auto") {
                        //    loadPanel.hide();
                        //    $("#lblGetBalance").val(result);
                        //}
                        else {
                            $.ajax({
                                url: '/SmartMeter/Terminal/GetCal',
                                type: 'POST',
                                data: { data: result },
                                success: function (result1) {
                                    loadPanel.hide();
                                    $("#lblReadECredit").val("Rs " + result1);
                                    //textareaLogs.option('value', result);
                                    //const myArray = result.split("/");
                                    //var data = myArray[1];

                                }
                            });
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

});
