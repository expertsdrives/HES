$(document).ready(function () {

    loadGasAMC();
    function loadGasAMC() {
        $("#atgltran").dxDataGrid({
            dataSource: "/atgltran",
            keyExpr: "ID",
            grouping: {
                autoExpandAll: true,
            },
            allowColumnReordering: true,
            allowColumnResizing: true,
            columnAutoWidth: true,
            groupPanel: {
                visible: true,
            },
            showBorders: true,
            scrolling: {
                columnRenderingMode: 'virtual',
            },
            paging: {
                pageSize: 50,
            },
            columnHidingEnabled: true,
            filterRow: {
                visible: true,
                applyFilter: 'auto',
            },
            export: {
                enabled: true,
            },
            columnChooser: {
                enabled: true
            },
            columnFixing: {
                enabled: true
            }, searchPanel: { visible: true },

            onEditingStart: function (e) {

            },
            pager: {
                allowedPageSizes: [10, 25, 50, 100],
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: true,
            },
            onCellPrepared: function (e) {
                if (e.rowType == "header") {
                    e.cellElement.css("text-align", "left");
                    e.cellElement.css("font-weight", "bold");
                }
            },
            onInitNewRow: function (e) {

            },
            columns: [
                { dataField: "ID", caption: "ID", width: 50, visible: false },
                { dataField: "FullName", caption: "Name" },
                { dataField: "CustomerID", caption: "Customer ID" },
                { dataField: "BusinessPatner", caption: "Business Patner" },
                { dataField: "SmartMeterSerialNumber", caption: "Meter Number" },
                { dataField: "PaymentAmount", caption: "Payment Amount", format: "#,##0.000"},
                { dataField: "PaymentMode", caption: "Payment Mode" },
                { dataField: "TransactionID", caption: "Transaction ID" },
              
                { dataField: "TransactionDate", caption: "Date", dataType: "date", format: "yyyy-MM-dd HH:mm:ss" },
                
            ]
        });
    }




    

   
});
