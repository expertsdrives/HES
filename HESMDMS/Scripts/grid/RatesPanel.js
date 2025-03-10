$(() => {
    $('#rates').dxDataGrid({
        //dataSource: JSON.parse(dataj),
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/ATGLRates",
            insertUrl: "/ATGLRatesInsert",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            },
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
                dataField: "GCV",
                caption: "GCV",
                
            },
            {
                dataField: "Tariff",
                caption: "Tariff",
               
            },
            {
                dataField: "ApplicableDate",
                caption: "ApplicableDate",
                editorType: "dxDateBox",
                editorOptions: {
                    value: new Date().toLocaleString('en-IN', { timeZone: 'Asia/Kolkata' }),
                displayFormat: "yyyy-MM-dd HH:mm", // Display format for both date and time
                type: "datetime",  // Ensure both date and time selection
                showClearButton: true,
                pickerType: "calendar", // Optional: you can specify the picker type (calendar)
                
                }
            },


        ],
        editing: {
            mode: "inline",
            allowUpdating: true,

            allowAdding: true,
            editEnabled: true,
            useIcons: true,
           
        },
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
