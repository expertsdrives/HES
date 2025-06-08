$(document).ready(function () {
    loadMeterGroupGrid();
    loadMeters();

    function loadMeterGroupGrid() {
        $("#MeterGroupGrid").dxDataGrid({
            dataSource: "/Admin/ATGLRates/GetMeterGroups",
            keyExpr: "ID",
            repaintChangesOnly: true,
            paging: { pageSize: 10 },
            columns: [

                { dataField: "GroupName", caption: "Group Name"},
                {
                    dataField: "Meters",
                    caption: "Selected Meters",
                    cellTemplate: function (container, options) {
                        container.text(options.value ? options.value.join(", ") : "");
                    }
                   
                },

                {
                    caption: "Actions",
                    type: "buttons",
                    buttons: [
                        {
                            hint: "Edit",
                            icon: "edit",
                            onClick: function (e) { EditMeterGroup(e.row.data); }
                        },
                        {
                            hint: "Delete",
                            icon: "trash",
                            onClick: function (e) { DeleteMeterGroup(e.row.data.ID); }
                        }
                    ]
                }
            ]
        });
    }

    function loadMeters() {
        $.getJSON("/Admin/ATGLRates/GetMeters", function (data) {
            let container = $("#MeterSelectionContainer");
            container.empty();
            $.each(data, function (index, item) {
                container.append(
                    `<div class="form-check">
                        <input class="form-check-input meter-checkbox" type="checkbox" value="${item.MeterID}" id="meter_${item.MeterID}">
                        <label class="form-check-label" for="meter_${item.MeterID}">${item.MeterName}</label>
                    </div>`
                );
            });
        });
    }

    function getSelectedMeterIds() {
        return $(".meter-checkbox:checked").map(function () {
            return parseInt($(this).val());
        }).get();
    }

    $("#MeterGroupForm").submit(function (e) {
        e.preventDefault();
        let id = $("#MeterGroupID").val();
        let url = id ? "/Admin/ATGLRates/EditMeterGroup" : "/Admin/ATGLRates/AddMeterGroup";

        let formData = {
            id: id || 0,
            groupName: $("#GroupName").val(),
            meterIds: getSelectedMeterIds()
        };

        if (formData.meterIds.length === 0) {
            alert("Please select at least one meter.");
            return;
        }

        $.post(url, formData, function (response) {
            alert(response.message);
            if (response.success) {
                $("#addModal").modal("hide");
                $("#MeterGroupGrid").dxDataGrid("instance").refresh();
            }
        });
    });

    $("#openaddModal").on("click", function () {
        $("#MeterGroupForm")[0].reset(); // clear all inputs
        $("#MeterGroupID").val(""); // clear ID so it's treated as 'add'
        $("#GroupName").val(""); // optionally reset group dropdown
        $("#addModal").modal("show");
    });

    function EditMeterGroup(record) {
        $("#MeterGroupID").val(record.ID);
        $("#GroupName").val(record.GroupName);

        $(".meter-checkbox").prop("checked", false);
        if (record.MeterIds) {
            let selectedIds = record.MeterIds.split(',').map(id => parseInt(id));
            selectedIds.forEach(id => {
                $(`#meter_${id}`).prop("checked", true);
            });
        }

        $("#addModal").modal("show");
    }

    function DeleteMeterGroup(id) {
        if (confirm("Are you sure you want to delete this record?")) {
            $.post("/Admin/ATGLRates/DeleteMeterGroup", { id: id }, function (response) {
                alert(response.message);
                if (response.success) {
                    $("#MeterGroupGrid").dxDataGrid("instance").refresh();
                }
            });
        }
    }
});