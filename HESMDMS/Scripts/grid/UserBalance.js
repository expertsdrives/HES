$(() => {

    var apidata = "";
    var modetype = "auto";
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
                url: '/SmartMeter/User/SendData',
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
                        $("#lblAddBalance").val("Balance Added");
                    }
                }
            });
        }
        else {
            alert("Please Select Meter");
        }


    });
});