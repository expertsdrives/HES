$(function () {
    const dataGrid = $("#AMRRawData").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/AMRRawData",

            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        keyExpr: "ID",
        allowColumnResizing: true,
        columnAutoWidth: true,
        showBorders: true,
        wordWrapEnabled: true,
        scrolling: {
            columnRenderingMode: 'virtual',
        },
        paging: {
            pageSize: 50,
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
                dataField: "Date",
                caption: "Date",
                format: 'dd-MM-yyyy',
            },
            {
                dataField: "Time",
                caption: "Time",
            },
            {
                dataField: "VAYUDUT_ID",
                caption: "VAYUDUT ID",
            },
            {
                dataField: "V_Voltage",
                caption: "Voltage",
            },
            {
                dataField: "VAYUDUT_TEMPERATURE",
                caption: "VAYUDUT TEMPERATURE",
            },
            {
                dataField: "MODULE_ERROR",
                caption: "MODULE ERROR",
                width:80
            },
            
            {
                dataField: "NW_ERROR",
                caption: "NW ERROR",
                width: 80
            },
            {
                dataField: "POST_ERROR",
                caption: "POST ERROR",
                width: 80
            },
            {
                dataField: "TX_ERRORS",
                caption: "TX ERRORS",
                width: 80

            },
            {
                dataField: "AMR_ID",
                caption: "AMR ID"

            },
            {
                dataField: "AMR_VERSION_NO",
                caption: "AMR VERSION NO"

            }
            , {
                dataField: "AMR_LOW_WEIGHTAGE_COUNT",
                caption: "LOW WEIGHTAGE COUNT"

            },{
                dataField: "AMR_HIGH_WEIGHTAGE_COUNT",
                caption: "HIGH WEIGHTAGE COUNT"

             }
               // ,{
            //    dataField: "DataCount",
            //    caption: "Data Count"

            //}
            , {
                dataField: "AMR_BATTERY_VOLTAGE",
                caption: "BATTERY VOLTAGE"

            },{
                dataField: "AMR_TEMPERATURE",
                caption: "TEMPERATURE"

            },{
                dataField: "AMR_MAGNET_TEMPER_LOGGER",
                caption: "MAGNET TAMPER"

            },{
                dataField: "AMR_CASE_TEMPER_LOGGER",
                caption: "CASE TAMPER"

            },{
                dataField: "AMR_CRC_STATUS",
                caption: "CRC"

            },
        ],

        searchPanel: { visible: true },

        //onEditingStart: function (e) {

        //},
        pager: {
            allowedPageSizes: [100, 200, 400, 500],
            showInfo: true,
            showNavigationButtons: true,
            showPageSizeSelector: true,
            visible: true,
        },
        onCellPrepared: function (e) {
            if (e.rowType == "header") {
                e.cellElement.css("text-align", "center");
                e.cellElement.css("font-weight", "bold");
            }
        },
        //onInitNewRow: function (e) {

        //},
        //onContentReady: function (e) {
        //    if (!e.component.isNotFirstLoad) {
        //        e.component.isNotFirstLoad = true; e.component.expandRow(e.component.getKeyByRowIndex(0));
        //    }
        //}
    }).dxDataGrid('instance');
});