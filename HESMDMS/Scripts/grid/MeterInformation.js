$(() => {
    var MeterIDSelect = "";
    var pld = "";
    var aid = "";
    var eeid = "";
    var apidata = "";
    $('#MeterSerialNumber').dxTextBox({
        placeholder: 'Meter Serial Number',
    });

    $('#MeterID').dxTextBox({
        placeholder: 'Meter ID',
    });
    $('#FWVer').dxTextBox({
        placeholder: 'FW Ver No.',
    });
    $('#HWVer').dxTextBox({
        placeholder: 'HW Ver No.',
    });

    $('#HWSr').dxTextBox({
        placeholder: 'HW Sr No.',
    });
    $('#LegalSwVerNo').dxTextBox({
        placeholder: 'Legal Sw Ver No.',
    });
    $('#NonLegalSwVerNo').dxTextBox({
        placeholder: 'Non-Legal Sw Ver No.',
    });

    $('#AppName').dxTextBox({
        placeholder: 'App Name',
    });

    $('#SimIMSI').dxTextBox({
        placeholder: 'Sim IMSI',
    });
    $('#TotalDataRecord').dxTextBox({
        placeholder: 'Total Data Record',
    });
    $('#MCUNO').dxTextBox({
        placeholder: 'MCU NO',
    });
    $('#FlashID').dxTextBox({
        placeholder: 'Flash ID',
    });
    $('#ModemIMEI').dxTextBox({
        placeholder: 'Modem IMEI',
    });
    $('#PrivateKey').dxTextBox({
        placeholder: 'Private Key',
    });
    $('#ModemFW').dxTextBox({
        placeholder: 'Modem FW',
    });
    $('#ReadSystemParameters').dxButton({
        stylingMode: 'contained',
        text: 'Read System Parameters',
        type: 'normal',
        onClick() {
            $.ajax({
                url: 'MeterManagment/GetMeterParamaterAsync',// The URL of the server endpoint to send the POST request
                type: "POST", // The HTTP method to use
                data: { aid: aid, pld: pld, eid: eeid },
                success: function (response) {
                    // Handle the response from the server
                    console.log("Success:", response);
                },
                error: function (xhr, status, error) {
                    // Handle any errors that occur during the request
                    console.log("Error:", error);
                }
            });
            //DevExpress.ui.notify('The Contained button was clicked');
        },
    });
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
            MeterIDSelect = meterData[0].MeterID;
            $("#MeterID").dxTextBox("instance").option("value", meterData[0].MeterID);
            pld = meterData[0].PLD;
            aid = meterData[0].AID;
            eeid = meterData[0].EID;
            if (pld == null || aid == null) {
                DevExpress.ui.notify("AID or PID cannot be NULL check the Meter Master", 'warning', 1000);
                return false;
            }
            console.log(meterData);
            //var returnedData = $.grep(json.items, function (element) {
            //    return element.MeterID.includes(data.value);
            //});
            //$('.current-value > span').text(data.value);
            //DevExpress.ui.notify(`The value is changed to: "${data.value}"`);
        },
    });

});
