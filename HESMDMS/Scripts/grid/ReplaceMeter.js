
$(function () {
    function logEvent(eventName) {
        var logList = $("#events ul"),
            newItem = $("<li>", { text: eventName });

        logList.prepend(newItem);
    }
    $("#ReplaceMeterGrid").dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: "ID",
            loadUrl: "/ReplaceAMR",
            insertUrl: "/InsertReplaceAMR",
            updateUrl: "/UpdateReplaceAMR",
            deleteUrl: "/DeleteReplaceAMR",
            onBeforeSend: function (method, ajaxOptions) {
                ajaxOptions.xhrFields = { withCredentials: true };
            }
        }),
        keyExpr: "ID",
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        showBorders: true,
        columnChooser: {
            enabled: true
        },
        columnFixing: {
            enabled: true
        },
        columns: [
            {
                dataField: "Photo",
                width: 100,
                allowFiltering: false,
                allowSorting: false,
                editCellTemplate: imageEditorTemplate,
                visible: false,
                cellTemplate: function (container, options) {
                    $("<div>")
                        .append($("<img>", { "src": options.value }))
                        .appendTo(container);
                }
            },
            {
                dataField: "Customer_Name",
                caption: "Customer Name",
                validationRules: [{ type: "required" }]
            },
            {
                dataField: "Customer_Mobile_No",
                caption: "Customer No",
                validationRules: [{ type: "required" }]
            },
            {
                dataField: "Block_House_No",
                caption: "Block House No",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "House_NO",
                caption: "House NO",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "Block",
                caption: "Block",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "Floor_No",
                caption: "Floor No",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "Flat_No",
                caption: "Flat No",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "Address",
                caption: "Address",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "Meter_Make",
                caption: "Meter Make",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "Meter_No",
                caption: "Meter No",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "Business_Partner_no",
                caption: "Business Partnerno",
                validationRules: [{ type: "required" }],
                visible: false
            },
            {
                dataField: "AMR_Sr_No",
                caption: "AMR SrNo",
                validationRules: [{ type: "required" }],

            },

            {
                dataField: "Tx_ID_DEC",
                caption: "Tx ID",
                validationRules: [{ type: "required" }],

            },
            {
                dataField: "AMR_Install_Date",
                caption: "Install Date",
                validationRules: [{ type: "required" }],
                dataType: "date",
                format: 'dd-MM-yyyy',

            },
            {
                dataField: "Op_Meter_Reading",
                caption: "Op Meter Reading",
                validationRules: [{ type: "required" }],

            },
            {
                dataField: "REMARK",
                caption: "Remarks",

                visible: false

            },
            {
                dataField: "SealNo",
                caption: "SealNo",

                visible: false

            },
            {
                dataField: "ZoneID",
                caption: "Zone",
                lookup: {
                    dataSource: zone,
                    displayExpr: "ZoneName",
                    valueExpr: "ID"
                }
            },
            {
                dataField: "SubZoneID",
                caption: "Inner Zone",
                lookup: {
                    dataSource: subZonelist,
                    displayExpr: "SubZoneName",
                    valueExpr: "ID"
                }
            },
            {
                dataField: 'CreatedOn',
                allowEditing: false,
                dataType: "date",
                format: 'dd/MM/yyyy',
                visible: false
            },

        ],

        searchPanel: { visible: true },
        editing: {
            mode: "popup",
            allowUpdating: true,
            allowDeleting: true,
            allowAdding: true,
            editEnabled: true,
            searchPanel: true,
            useIcons: true
        }, onEditingStart: function (e) {
            logEvent("EditingStart");
        },
        onInitNewRow: function (e) {
            logEvent("InitNewRow");
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