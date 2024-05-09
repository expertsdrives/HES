var bpnuum = "";
var selectedRowKeys = [];
var selectedRowDate = [];
var selectedRowReading = [];
var selectedRowBP = [];
$(() => {
  
    $('#CustomersGenerateBillAMR').dxDataGrid({
        //dataSource: JSON.parse(dataj),
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/GetDataBillingAMR",

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
        selection: {
            mode: "multiple"
        },
        onSelectionChanged: function (selectedItems) {
            selectedRowKeys = selectedItems.selectedRowsData;
        },
        columns: [
            {
                dataField: "BusinessPartnerNo",
                caption: "Business Partner",
                width: 150
            },
            {
                dataField: "FullName",
                caption: "Full Name",
                width: 150
            },
            {
                dataField: "AMRSerialNumber",
                caption: "AMR SerialNumber",
                width: 150
            },
            {
                dataField: "MeterNumber",
                caption: "Meter Number",
                width: 150
            },
            {
                dataField: "Installation",
                caption: "Installation",
                width: 150
            },
            {
                dataField: "Date",
                caption: "Date",
                width: 200
            },
            {
                dataField: "ReadingCount",
                caption: "Meter Reading",
                width: 200
            },
            {

                caption: 'Action',
                width: 150,
                //minWidth: 320,
                cellTemplate(container, options) {

                    /*container.addClass('');*/
                    var dataButton = $('<button>')
                        .addClass('btn btn-outline-primary btn-sm bg-primary')
                        .text("Generate")
                        .attr("onclick", "getClick('" + options.data.BusinessPartnerNo + "','" + options.data.Date + "','" + options.data.ReadingCount + "')");
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
    $("#getSelectedIdsBtn").dxButton({
        text: "Get Selected IDs",
        onClick: function () {
            // Send selectedRowKeys to the controller
            $.ajax({
                type: "POST",
                url: "/YourController/YourAction",
                data: { selectedRowKeys: selectedRowKeys },
                success: function (response) {
                    console.log(response);
                    // Handle success response
                },
                error: function (xhr, textStatus, errorThrown) {
                    console.error(xhr.responseText);
                    // Handle error
                }
            });
        }
    });

});


function getClick(data, date, ReadingCount) {
    bpnuum = data;
   
    $.ajax({
        url: "GenerateBillAsyncAMRAsync?bpnumber=" + JSON.parse(selectedRowKeys) + "&Date=" + date + "&ReadingCount=" + ReadingCount,
        success: function (result) {
            
            $('#exampleModal').modal('hide');
            alert('Bill generated for ' + result);
        }
    });
    //$.ajax({
    //    url: "ApproveMeter?ID=" + data,
    //    success: function (result) {
    //        dataGrid.dxDataGrid("instance").refresh();
    //    }
    //});
}