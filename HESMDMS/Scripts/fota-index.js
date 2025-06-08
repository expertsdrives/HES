var FotaPage = (function () {

    var selectedMeterId;
    var urls;

    function init(meterId, urlConfig) {
        selectedMeterId = meterId;
        urls = urlConfig;
        initializeControls();
    }

    function initializeControls() {
        var selectBox = $('#SelectMeterFota').dxSelectBox({
            dataSource: DevExpress.data.AspNet.createStore({
                key: "ID",
                loadUrl: urls.selectSmartMeter,
                onBeforeSend: function (method, ajaxOptions) {
                    ajaxOptions.xhrFields = { withCredentials: true };
                }
            }),
            displayExpr: 'TempMeterID',
            valueExpr: 'TempMeterID',
            searchEnabled: true,
            name: 'myDropdown',
            onValueChanged: onMeterChanged
        }).dxSelectBox("instance");

        if (selectedMeterId) {
            selectBox.option("value", selectedMeterId);
        }
    }

    function onMeterChanged(data) {
        var meterId = data.value;
        $('#selectedMeter').val(meterId);
        
        loadFileHistory(meterId);
        validateMeterDetails(meterId);
    }

    function loadFileHistory(meterId) {
        $("#filehistory").dxDataGrid({
            dataSource: `${urls.fotaFileHistory}?meterid=${meterId}`,
            keyExpr: "ID",
            paging: { pageSize: 10 },
            columns: [
                { dataField: "ID", caption: "ID", width: 50, visible: false },
                { dataField: "MeterID", caption: "Meter Number" },
                { dataField: "FileName", caption: "FileName" },
                { dataField: "CreatedDate", caption: "CreatedDate", dataType: "date", format: "yyyy-MM-dd HH:mm:ss" },
            ]
        });
    }

    function validateMeterDetails(meterId) {
        $.ajax({
            url: urls.getMeterDetails,
            type: 'GET',
            data: { meterId: meterId },
            success: function (response) {
                if (response.success) {
                    var pld = response.data.PLD;
                    var aid = response.data.AID;

                    if (!pld || !aid) {
                        DevExpress.ui.notify("AID or PLD cannot be NULL. Please check the Meter Master.", 'warning', 2000);
                    }
                } else {
                    DevExpress.ui.notify(response.message, 'error', 2000);
                }
            },
            error: function () {
                DevExpress.ui.notify("An error occurred while fetching meter details.", 'error', 2000);
            }
        });
    }

    return {
        init: init
    };

})();