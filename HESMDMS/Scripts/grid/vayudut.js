$(function () {
    const dataGrid = $("#vayuduts").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/DisplayVayudut",
            insertUrl: "/InsertVayudut",
            updateUrl: "/UpdateVayudut",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        keyExpr: "ID",
        grouping: {
            autoExpandAll: false,
        },
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        showBorders: true,
        wordWrapEnabled: true,
        scrolling: {
            columnRenderingMode: 'virtual',
        },
        editing: {
            mode: "inline",
            allowUpdating: true,

            allowAdding: true,
            editEnabled: true,
            useIcons: true
        },
        paging: {
            pageSize: 300,
        },
        //filterRow: {
        //    visible: true,
        //    applyFilter: 'auto',
        //},
        export: {
            enabled: true,
        },
        columnFixing: {
            enabled: true
        },
        columns: [
            {
                dataField: "VayudutSrNo",
                caption: "Serial Number",
                dataType: 'number',
            },
            {
                dataField: "VayudutInstalledLocation",
                caption: "Installed Location",
               

            },
            {
                dataField: "VayudutInstalledDate",
                caption: "Insallted Date",
                dataType: "date",
                format: 'dd-MM-yyyy',
             
       
                sortOrder: 'desc',

            },

            {
                dataField: "PhoneNumber",
                caption: "Number",
                dataType: 'number',
            },
            {
                dataField: "IMSI",
                caption: "IMSI",
                dataType: 'number',
            },
            {
                dataField: "CreatedDate",
                caption: "Created Date",
                dataType: "date",
                format: 'dd-MM-yyyy',
            },
            {
                dataField: "IsAssigned",
                caption: "Is Assigned",
                dataType: 'boolean',
            },

        ],

        searchPanel: { visible: true },

        onEditingStart: function (e) {

        },

        onCellPrepared: function (e) {
            if (e.rowType == "header") {
                e.cellElement.css("text-align", "center");
                e.cellElement.css("font-weight", "bold");
            }
        },

    }).dxDataGrid('instance');
});