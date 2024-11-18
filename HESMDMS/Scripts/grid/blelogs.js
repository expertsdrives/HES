$(() => {
    $('#blelogs').dxDataGrid({
        //dataSource: JSON.parse(dataj),
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/blelogs",

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
                dataField: "MeterID",
                caption: "Meter ID",
                width: 200

            },
            {
                dataField: "Currentkw",
                caption: "Current",
                width: 200

            },

            {
                dataField: "CapturedBy",
                caption: "Captured By",
                width: 200
            },
            {
                dataField: "User_Type",
                caption: "User Type",
                width: 200
            },
            {
                dataField: "CreatedDate",
                caption: "Date",
                width: 250
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
