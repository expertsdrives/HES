var uniqueValues = [];
var dataGrid = null;
let prev = 0;
let next = 0;
$(function () {
    dataGrid = $("#15DayConsumption").dxDataGrid({
        dataSource: JSON.parse(dayreport),
        keyExpr: "MeterNumber",
        allowColumnResizing: true,
        columnAutoWidth: true,
        showBorders: true,
        columnResizingMode: 'widget',
        columnMinWidth: 150,
        remoteOperations: true,
        wordWrapEnabled: true,
        paging: {
            pageSize: 10,
        },
      
        filterRow: {
            visible: true,
            applyFilter: 'auto',
        },
        export: {
            enabled: true,
        },
        columnFixing: {
            enabled: true
        },
        //columns: [
           
        //    {
        //        dataField: "FullName",
        //        caption: "Customer Name",
                
        //    },
        //    {
        //        dataField: "BusinessPartnerNo",
        //        caption: "Business Partner",

        //    },
            
        //    {
        //        dataField: "MeterNumber",
        //        caption: "IsActive",
        //        dataType: "boolean"
        //    },
        //],
        searchPanel: { visible: true },
        onContentReady: function (e) {
            var saveButton = $(".dx-button[aria-label='Save']");
            if (saveButton.length > 0)
                saveButton.click(function (event) {
                    if (!isUpdateCanceled) {
                        DoSomething(e.component);
                        event.stopPropagation();
                    }
                });
        },
        onEditingStart: function (e) {

        },
        onCellPrepared: function (e) {
            if (e.rowType == "header") {
                e.cellElement.css("text-align", "center");
                e.cellElement.css("font-weight", "bold");
                e.cellElement.css("white-space", "nowrap");
            }
            if (e.column.dataField == "FullName") {
                e.cellElement.css("min-width", "250px");
            }
            if (e.column.dataField == "MeterNumber") {
                e.cellElement.css("min-width", "100px");
            }
        },
        onInitNewRow: function (e) {

        },
        //onContentReady: function (e) {
        //    var saveButton = $(".dx-button[aria-label='Save']");
        //    if (saveButton.length > 0)
        //        saveButton.click(function (event) {
        //            if (!isUpdateCanceled) {
        //                DoSomething(e.component);
        //                event.stopPropagation();
        //            }
        //        });
        //}
    });
   
    
});