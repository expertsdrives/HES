var role = roleid;
var dataGrid = null;
$(function () {
    dataGrid = $("#customerDetails").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/CustomerLoad",

            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        keyExpr: "ID",
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        showBorders: true,
        scrolling: {
            columnRenderingMode: 'virtual',
        },
        paging: {
            pageSize: 5,
        },
        export: {
            enabled: true,
        },
        columnChooser: {
            enabled: true
        },
        columnFixing: {
            enabled: true
        },
        columns: [
            {
                dataField: "Image",
                allowFiltering: false,
                width: 100,
                height:100,
                allowSorting: false,
                 editCellTemplate: imageEditorTemplate,

                cellTemplate: function (container, options) {
                    $("<div id='krutish'>")
                        .append($("<img>", { "width": "100%", "src": "../images/" + options.value }))
                        .appendTo(container);
                }
            },
            {
                dataField: "FullName",
                caption: "Customer Name",
                validationRules: [{ type: "required" }],
                width: 100
            },
            {
                dataField: "BusinessPartnerNo",
                caption: "Business Partner No",
                validationRules: [{ type: "required" }],
                width: 150,
            },
            {
                dataField: "Address",
                caption: "Address",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "MeterNumber",
                caption: "Meter Number",
                validationRules: [{ type: "required" }],
                width: 110
            },
            {
                dataField: "OpeningMeterReading",
                caption: "Opening Meter reading",
                validationRules: [{ type: "required" }],


            },
            {
                dataField: "OpeningAMRReading",
                caption: "Opening AMR count",
                validationRules: [{ type: "required" }],
            },
            {
                dataField: "SerialNumber",
                caption: "AMR Serial Number",
                validationRules: [{ type: "required" }],

            },
            {
                dataField: "TXID",
                caption: "AMR TXID",
                validationRules: [{ type: "required" }],
            },
            {
                dataField: "Altitude",
                caption: "Altitude",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "InstallationDate",
                caption: "AMR Installation date",
                validationRules: [{ type: "required" }],
                dataType: "date",
                format: 'dd-MM-yyyy',
            },


            {
                dataField: "Country",
                caption: "Country",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "MobileNumber",
                caption: "Mobile Number",
                validationRules: [{ type: "required" }],
                visible: false
            },

            {
                dataField: "Email",
                caption: "Email",
                validationRules: [{ type: "required" }],
                visible: false

            },
            {
                dataField: "PinCode",
                caption: "Pin Code",
                validationRules: [{ type: "required" }],
                visible: false

            },
            {
                dataField: "Town",
                caption: "Town",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "Latitude",
                caption: "Latitude",

                visible: false

            },
            {
                dataField: "Longitude",
                caption: "Longitude",

                visible: false

            },
            {
                dataField: "CreatedBy",
                caption: "Created By",
            },
            {
                dataField: "ApprovedBy",
                caption: "Approved By",
            },
            {
                dataField: "Status",
                caption: "Status",
                cellTemplate(container, options) {

                    const fieldData = options.data;
                    container.addClass(fieldData.Status == "Pending" ? 'redColor' : 'greenColor');
                    //$('<span>')
                    //    .addClass('redColor')
                    //    .text(options.text)
                    //    .appendTo(container);

                    $('<span>')
                        .text(options.text)
                        .appendTo(container);

                }
            },
            {

                caption: 'Action',

                //minWidth: 320,
                cellTemplate(container, options) {
                    if (role == 1 || role == 3) {
                        /*container.addClass('');*/
                        var dataButton = $('<button class="btn btn-outline-primary btn-sm bg-primary" onclick="getClick(' + options.data.ID + ')">')
                            .text("Approve");
                        if (options.data.Status == "Pending") {
                            dataButton.appendTo(container);
                        }
                    }
                },
            },
            // {
            //    type: "buttons",
            //    buttons: ["edit", "delete", {
            //        text: "Approve",
            //        /*icon: "/url/to/my/icon.ico",*/
            //        hint: "My Command",
            //        onClick: function (e) {
            //            var id = e.row.data.ID
            //            // Execute your command here
            //        }
            //    }]
            //}
            //{
            //    dataField: "ZoneID",
            //    caption: "Zone",
            //    lookup: {
            //        dataSource: zone,
            //        displayExpr: "ZoneName",
            //        valueExpr: "ID"
            //    }
            //},
            //{
            //    dataField: "SubZoneID",
            //    caption: "Inner Zone",
            //    lookup: {
            //        dataSource: subZonelist,
            //        displayExpr: "SubZoneName",
            //        valueExpr: "ID"
            //    }
            //},
            //{
            //    dataField: 'CreatedOn',
            //    allowEditing: false,
            //    dataType: "date",
            //    format: 'dd/MM/yyyy',
            //    visible: false
            //},

        ],
        //editing: {
        //    mode: "popup",
        //    allowUpdating: true,
        //    allowDeleting: true,
        //    allowAdding: true,
        //    editEnabled: true,
        //    searchPanel: true,
        //    useIcons: true
        //},
        searchPanel: { visible: true },

        onEditingStart: function (e) {

        },
        onInitNewRow: function (e) {

        },
        onCellPrepared: function (e) {
            if (e.rowType == "header") {
                e.cellElement.css("text-align", "left");
                e.cellElement.css("font-weight", "bold");

            }
        },
        onContentReady: function (e) {
            var saveButton = $(".dx-button[aria-label='Save']");
            if (saveButton.length > 0)
                saveButton.click(function (event) {
                    if (!isUpdateCanceled) {
                        DoSomething(e.component);
                        event.stopPropagation();
                    }
                });
        }
    });
    function imageEditorTemplate(cellElement, cellInfo) {
        return $("<div>").dxFileUploader({
            selectButtonText: "Select photo",
            labelText: "",
            name: "myFile",

            uploadMode: "instantly",
            uploadUrl: "Upload",
            accept: "image/*",

            onValueChanged: function (e) {
                var files = e.value;
                $.each(files, function (i, file) {
                    var fileURL = URL.createObjectURL(file);
                    cellInfo.setValue(file.name);
                });

            }
        });
    }

});
function getClick(data) {
    $.ajax({
        url: "ApproveMeter?ID=" + data,
        success: function (result) {
            dataGrid.dxDataGrid("instance").refresh();
        }
    });
}