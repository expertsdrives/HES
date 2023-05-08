$(() => {
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
        //onClick() {
        //    DevExpress.ui.notify('The Contained button was clicked');
        //},
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

            $("#MeterID").dxTextBox("instance").option("value", meterData[0].MeterID);
            console.log(meterData);
            //var returnedData = $.grep(json.items, function (element) {
            //    return element.MeterID.includes(data.value);
            //});
            //$('.current-value > span').text(data.value);
            //DevExpress.ui.notify(`The value is changed to: "${data.value}"`);
        },
    });

});
