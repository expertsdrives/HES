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
                dataField: "VayadutNo",
                caption: "Gateway Number",
               
            },
            {
                dataField: "PcbNo",
                caption: "Pcb No",
               

            },
            {
                dataField: "CardNo",
                caption: "Card No",
            },

            {
                dataField: "VayadutId",
                caption: "Gateway ID",
                
            },
            {
                dataField: "Latitude",
                caption: "Latitude",

            },
            {
                dataField: "Longitude",
                caption: "Longitude",
              
            },
            {
                dataField: "SealNo1",
                caption: "SealNo1",
    
            }, {
                dataField: "SealNo2",
                caption: "SealNo2",

            }, {
                dataField: "Area",
                caption: "Area",

            },
            {
                dataField: "Remark",
                caption: "Remark",

            },
            {
                dataField: "Date",
                caption: "Date",
                dataType: "date",
                format: 'dd-MM-yyyy',
                width:120
            },
            {
                dataField: "TpiName",
                caption: "Tpi Name",

            }, {
                dataField: "GatewayStatus",
                caption: "Gateway Status",

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