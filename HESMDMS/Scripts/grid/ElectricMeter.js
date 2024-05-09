$(() => {
    $('#smartmeterdata').dxDataGrid({
        //dataSource: JSON.parse(dataj),
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/electricsmartmeterdata",

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
            mode: 'Select',
            position: {
                my: 'right top',
                at: 'right bottom',
                of: '.dx-datagrid-column-chooser-button',
            },
            search: {
                enabled: true,
                editorOptions: { placeholder: 'Search column' },
            },
            selection: {
                recursive: true,
                selectByClick: true,
                allowSelectAll: true,
            },
        },
        columns: [
            {
                dataField: "MeterName",
                caption: "Msn",
                width: 200

            },
            {
                dataField: "Clock",
                caption: "Meter TimeStamp",
                width: 200

            },
          
            {
                dataField: "Voltage",
                caption: "Voltage",
                width: 100
            },
            {
                dataField: "PhaseCurrent",
                caption: "Current",
                width: 100
            },
            {
                dataField: "NeutralCurrent",
                caption: "Neutral Current",
                width: 120
            }, {
                dataField: "ApparentPower",
                caption: "Apparent Power",
                width: 120
            },
            {
                dataField: "ActivePower",
                caption: "Active Power",
                width: 120
            },
            {
                dataField: "ActiveEnergy",
                caption: "Active Energy",
                width: 120
            },
            {
                dataField: "ApparentEnergy",
                caption: "Apparent Energy",
                width: 120
            },
            {
                dataField: "Frequency",
                caption: "Frequency",
                width: 120
            }
            

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
});
