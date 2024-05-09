var bpnuum = "";
$(() => {
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
    $('#fromDate').dxDateBox({
        type: 'date',
        displayFormat: 'yyyy-MM-dd',
        min: new Date(2022, 03, 01),
        onValueChanged(data) {
           
        },
    });
    $('#toDate').dxDateBox({
        type: 'date',
        displayFormat: 'yyyy-MM-dd',
        min: new Date(2022, 03, 01),
    });
    $('#CustomersGenerateBill').dxDataGrid({
        //dataSource: JSON.parse(dataj),
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/GetDataBilling",

            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        filterRow: {
            visible: true,
            applyFilter: 'auto',
        },
        columnChooser: {
            enabled: true,
        },
        columns: [
            {
                dataField: "BusinessPatner",
                caption: "Business Partner",
                width: 150
            },
            {
                dataField: "Installation",
                caption: "Installation",
                width: 150
            },
            {
                dataField: "ContractAcct",
                caption: "Contract Acct",
            },
            {
                dataField: "FullName",
                caption: "Full Name",
            },
            {
                dataField: "MeterType",
                caption: "Meter Type",
            },
            {
                dataField: "MeterNumber",
                caption: "Meter Number",
            },
            {
                dataField: "ScheduleDate",
                caption: "Schedule Date",
            }, {
                dataField: "Street",
                caption: "Street",
            }, {
                dataField: "Street2",
                caption: "Street2",
            }, {
                dataField: "Street3",
                caption: "Street3",
            }, {
                dataField: "Street4",
                caption: "Street4",
            }, {
                dataField: "Street5",
                caption: "Street5",
            }, {

                caption: 'Action',

                //minWidth: 320,
                cellTemplate(container, options) {

                    /*container.addClass('');*/
                    var dataButton = $('<button class="btn btn-outline-primary btn-sm bg-primary" onclick="getClick(' + options.data.BusinessPatner + ')">')
                        .text("Generate");
                    dataButton.appendTo(container);

                },
            },

        ],
        searchPanel: {
            visible: true,
            width: 240,
            placeholder: 'Search...',
        },
        headerFilter: {
            visible: true,
        },
        onCellPrepared: function (e) {
            if (e.rowType == "header") {
                e.cellElement.css("text-align", "center");
                e.cellElement.css("font-weight", "bold");
            }
        },
        showBorders: true,
        wordWrapEnabled: true,
        columnAutoWidth: true,
        scrolling: {
            columnRenderingMode: 'virtual',
        },
        export: {
            enabled: true,
        },
        columnFixing: {
            enabled: true
        },
    });
    $('#billGenerate').click(function () {
        loadPanel.show();
        var fromdateBox = $("#fromDate").dxDateBox("instance");
        var fromValue = fromdateBox.option('text');
        var TodateBox = $("#toDate").dxDateBox("instance");
        var ToValue = TodateBox.option('text');
        $.ajax({
            url: "GenerateBillAsync?bpnumber=" + bpnuum + "&fromdate=" + fromValue + "&todate=" + ToValue,
            success: function (result) {
                loadPanel.hide();
                $('#exampleModal').modal('hide');
                alert('Bill generated for ' + result);
            }
        });
    });
});
function getClick(data) {
    bpnuum = data;
    $('#bpnumber').val(bpnuum);
    $('#exampleModal').modal('show');
    //$.ajax({
    //    url: "ApproveMeter?ID=" + data,
    //    success: function (result) {
    //        dataGrid.dxDataGrid("instance").refresh();
    //    }
    //});
}