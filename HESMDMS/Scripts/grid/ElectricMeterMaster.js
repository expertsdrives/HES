let loadPanel;
$(() => {
    $('#ElectricMeterMaster').dxDataGrid({
        //dataSource: JSON.parse(dataj),
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/ElectricMeterMaster",
            insertUrl: "/InsertElectricMeterMaster",
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
        paging: {
            pageSize: 20,
        },
        editing: {
            mode: "inline",
            allowUpdating: false,

            allowAdding: true,
            editEnabled: false,
            useIcons: true,
           
        },
        columns: [
            {
                dataField: "SystemTitle",
                caption: "Msn",
                width: 200

            },
            {
                dataField: "IPAddress",
                caption: "IP Address",
                width: 220

            },
          
            {

                caption: 'Action',

                //minWidth: 320,
                cellTemplate(container, options) {
                  
                        /*container.addClass('');*/
                    var dataButtonON = $('<button class="btn btn-outline-primary btn-sm bg-primary" onclick="getClickON(' + options.data.SystemTitle + ')">')
                            .text("ON");
                            dataButtonON.appendTo(container);
                    var dataButtonOFF = $('<button class="btn btn-outline-primary btn-sm bg-primary" onclick="getClickOFF(' + options.data.SystemTitle + ')">')
                        .text("OFF");
                    dataButtonOFF.appendTo(container);
                    
                },
                width: 250
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
     loadPanel = $('.loadpanel').dxLoadPanel({
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
});
function getClickON(data) {
    loadPanel.show();
    $.ajax({
        url: "ReConnectMeterAsync?ipaddress=" + data,
        success: function (result) {
            loadPanel.hide();
            alert(result);
        }
    });
}
function getClickOFF(data) {
    loadPanel.show();
    $.ajax({
        url: "DisConnectMeterAsync?ipaddress=" + data,
        success: function (result) {
            loadPanel.hide();
            alert(result);
        }
    });
}