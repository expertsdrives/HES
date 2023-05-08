$(() => {
    $('#D2C').dxDataGrid({
        //dataSource: JSON.parse(dataj),
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/d2c",

            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        filterRow: {
            visible: true,
            applyFilter: 'auto',
        },
        searchPanel: {
            visible: true,
            width: 240,
            placeholder: 'Search...',
        },
        headerFilter: {
            visible: true,
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
