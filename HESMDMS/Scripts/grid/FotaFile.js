$(() => {
    var pld = "";
    var aid = "";
    var eeid = "";
    var apidata = "";
    var mid = "";
    var modetype = "auto";
    
    $('#SelectMeterFota').dxSelectBox({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/SelectSmartMeter?roleid=7",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        displayExpr: 'TempMeterID',
        valueExpr: 'TempMeterID',
        searchEnabled: true, // ✅ Enables the search box
       
        
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

            
           
        },
    });

    $("#file-uploader").dxFileUploader({
        name: "uploadedFile",
        multiple: false,
        uploadMode: "useForm",
        uploadUrl: "Fota/UploadWithMeter",
        uploadCustomData: function () {
            const meter = selectBoxInstance.option("value");
            console.log("Meter sent: ", meter);
            return {
                meterNumber: meter
            };
        },
        onUploading: function (e) {
            const meter = selectBoxInstance.option("value");
            if (!meter) {
                e.cancel = true;
                DevExpress.ui.notify("Select a meter first!", "error", 2000);
            }
        },
        onUploaded: function () {
            DevExpress.ui.notify("Upload complete", "success", 2000);
        },
        onUploadError: function () {
            DevExpress.ui.notify("Upload failed", "error", 2000);
        }
    });

});
