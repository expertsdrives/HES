$(() => {
    var pld = "";
    var aid = "";
    var eeid = "";
    var apidata = "";
    $('#SelectMeter1').dxSelectBox({
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


    $("#btnAddBalance").click(function () {
        if (pld != "") {
            loadPanel.show();
            var balanceInput = $('#lblAddBalance').val();
            $.ajax({
                url: '/SmartMeter/Terminal/SendDataAddBalance',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Add Balance", balanceInput: balanceInput },
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
                        $("#lblAddBalance").val("No Response");
                    }
                }
            });
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
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Get Balance" },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "error") {
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
                    else {
                        loadPanel.hide();
                        $("#lblGetBalance").val("No Response");
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
            loadPanel.show();
            var balanceInput = $('#lblSetVAT').val();
            $.ajax({
                url: '/SmartMeter/Terminal/SendDataAddBalance',
                type: 'POST',
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Set Vat", balanceInput: balanceInput },
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
                        $("#lblSetVAT").val("No Response");
                    }
                }
            });
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
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Get Vat" },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "error") {
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
                    else {
                        loadPanel.hide();
                        $("#lblGetVat").val("No Response");
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
            loadPanel.show();
            var date = $('#lblTraiffDate').val();
            var time = $('#lblTraiffTime').val();
            var Tariff = $('#lblTraiffAmount').val();
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
                        $("#lblAddBalance").val("No Response");
                    }
                }
            });
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
                data: { aid: aid, pld: pld, eid: eeid, eventname: "Get Tariff" },
                success: function (result) {
                    loadPanel.hide();

                    if (result != "error") {
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
                        $("#lblGetTariff").val("No Response");
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
