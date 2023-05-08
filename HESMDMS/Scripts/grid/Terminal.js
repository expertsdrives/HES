
$(() => {
    var pld = "";
    var aid = "";
    var eeid = "";
    var apidata = "";

    const d2c = $('#d2cLogs').dxTextArea({
        placeholder: 'Logs',
        height: 200,
        readOnly: true

    }).dxTextArea('instance');
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

            //textarea.option('value', meterData[0].MeterID);
            console.log(meterData);
            //var returnedData = $.grep(json.items, function (element) {
            //    return element.MeterID.includes(data.value);
            //});
            //$('.current-value > span').text(data.value);
            //DevExpress.ui.notify(`The value is changed to: "${data.value}"`);
            // Declare a proxy to reference the hub.
            //var notifications = $.connection.myHub;
            //debugger;
            // Create a function that the hub can call to broadcast messages.
            var notifications = $.connection.myHub;
            //debugger;
            // Create a function that the hub can call to broadcast messages.
            
            notifications.client.updateMessages = function () {
                getAllMessages()
            };
            // Start the connection.
            $.connection.hub.start().done(function () {
                console.log("connection started")
                //notifications.onconn();
                getAllMessages();
            }).fail(function (e) {
                alert(e);
            });
            function getAllMessages() {
                //var tbl = $('#messagesTable');
                $.ajax({
                    url: 'Terminal/GetMessages',
                    data: { pld: pld},
                    contentType: 'application/html ; charset:utf-8',
                    type: 'GET',
                    dataType: 'html',
                    success: function (result) {
                        console.log(result);
                        d2c.option('value', result);
                        //var a2 = JSON.parse(result);
                        //tbl.empty();
                        //var i = 1;
                        //$.each(a2, function (key, value) {
                        //    tbl.append('<tr>' + '<td>' + i + '</td>' + '<td>' + value.empName + '</td>' + '<td>' + value.Salary + '</td>' + '<td>' + value.DeptName + '</td>' + '<td>' + value.Designation + '</td>' + '</tr>');
                        //    i = i + 1;
                        //});
                    }
                });
            }




        },
    });
    const textarea = $('#terminalArea').dxTextArea({
        placeholder: 'Enter Data comma seprated',
        height: 200,
        onValueChanged(data) {
            apidata = data.value;
        },
    }).dxTextArea('instance');

    const textareaLogs = $('#LogsSend').dxTextArea({
        placeholder: 'Logs',
        height: 200,
        readOnly: true

    }).dxTextArea('instance');

   

    $('#Send').dxButton({
        stylingMode: 'contained',
        text: 'Send Cloud to Device',
        type: 'normal',
        onClick() {

            $.ajax({
                url: 'Terminal/SendToAPIAsync',
                type: 'POST',
                data: { aid: aid, pld: pld, data: apidata, eid: eeid },
                success: function (result) {
                    textareaLogs.option('value', result);
                    const myArray = result.split("/");
                    var data = myArray[1];
                    DevExpress.ui.notify(result, "warning", 2000);
                }
            });
            //DevExpress.ui.notify('The Contained button was clicked');
        },
    });
});
