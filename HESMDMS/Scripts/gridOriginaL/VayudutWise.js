﻿var uniqueValues = [];
var dataGrid = null;
let prev = 0;
let next = 0;
$(function () {
    dataGrid = $("#VayudutWise").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            load: function (loadOptions) {
                var deferred = $.Deferred(),
                    args = {};

                // Sorting
                if (loadOptions.sort) {
                    args.sortField = loadOptions.sort[0].selector;
                    args.sortOrder = loadOptions.sort[0].desc ? "desc" : "asc";
                }

                // Pagination
                args.skip = loadOptions.skip || 0;
                args.take = loadOptions.take || 300;

                // Filtering
                if (loadOptions.filter) {
                    args.filter = JSON.stringify(loadOptions.filter);
                }

          
               

                $.ajax({
                    url: "/Admin/Reports/GetData",
                    dataType: "json",
                    data: args,
                    success: function (result) {
                        deferred.resolve(result.data, { totalCount: result.totalCount });
                    },
                    error: function () {
                        deferred.reject("Data loading error");
                    }
                });

                return deferred.promise();
            },
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
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
        grouping: {
            autoExpandAll: false,
        },
        showBorders: true,
        wordWrapEnabled: true,
        scrolling: {
            columnRenderingMode: 'virtual',
        },
        paging: {
            pageSize: 300,
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
        
        columns: [
            {
                dataField: "IndexValue",
                caption: "Sr.No",
                width: 80

            },
            {
                dataField: "FullName",
                caption: "Customer Name",
                width: 250

            },
            {
                dataField: "BusinessPartnerNo",
                caption: "Business Partner",
                width: 140,

            },
           
            {
                dataField: "ReadingCount",
                headerCellTemplate: $('<span>Meter Reading (m</span><span class="sub">3</span><span>)</span>'),
                width: 120,
            },
            {
                dataField: "MeterNumber",
                caption: "Meter Number",
                width: 120

            },
            {
                dataField: "VAYUDUT_ID",
                groupIndex: 0,
            },
            {
                dataField: "Date",
                caption: "Rx Date",
                dataType: "date",
                format: 'dd-MM-yyyy',
                width: 115,
                groupIndex: 1,
                sortOrder: 'desc',
            },
            {
                dataField: "Date",
                caption: "Rx Date",
                dataType: "date",
                format: 'dd-MM-yyyy',
                width: 115,
                sortOrder: 'desc',
            },
            {
                dataField: "Time",
                caption: "Time",
                width: 100,
                sortOrder: 'asc',
            },
            {
                dataField: "SerialNumber",
                caption: "AMR UID",
                width: 140,
            },
            {
                dataField: "TXID",
                caption: "AMR TXID",
                width: 120,
            },
        ],

        searchPanel: { visible: true },
     
        onEditingStart: function (e) {

        },
        pager: {
            allowedPageSizes: [200, 300, 400, 500],
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

function getClick(data) {
    $.ajax({
        url: "ApproveMeter?ID=" + data,
        success: function (result) {
            dataGrid.dxDataGrid("instance").refresh();
        }
    });
}