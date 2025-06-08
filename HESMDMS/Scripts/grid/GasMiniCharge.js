$(document).ready(function () {
    loadGasMiniCharge();
    loadGroups();

    function loadGasMiniCharge() {
        $("#GasMiniChargeGrid").dxDataGrid({
            dataSource: "/Admin/ATGLRates/GetGasMiniCharge",
            keyExpr: "ID",
            paging: { pageSize: 10 },
            columns: [
                { dataField: "ID", caption: "ID", width: 50, visible: false },
                { dataField: "GroupName", caption: "Group Name" },
                { dataField: "Minimumchargerate", caption: "Minimumchargerate", format: "#,##0.000" },
                { dataField: "Date", caption: "Date", dataType: "date", format: "yyyy-MM-dd HH:mm:ss" },
                {
                    caption: "Actions",
                    type: "buttons",
                    buttons: [
                        {
                            hint: "Edit",
                            icon: "edit",
                            onClick: function (e) { editGasMiniCharge(e.row.data); }
                        },
                        {
                            hint: "Delete",
                            icon: "trash",
                            onClick: function (e) { deleteGasMiniCharge(e.row.data.ID); }
                        }
                    ]
                }
            ]
        });
    }

    function loadGroups() {
        $.getJSON("/Admin/ATGLRates/GetGroups", function (data) {
            let meterDropdown = $("#GroupName");
            meterDropdown.empty();
            $.each(data, function (index, item) {
                meterDropdown.append($("<option>", { value: item.GroupName, text: item.GroupName }));
            });
        });
    }

    $("#gasMiniChargeForm").submit(function (e) {
        e.preventDefault();

        let gasMiniCharge = {
            ID: $("#GasMiniChargeID").val() || 0,
            GroupName: $("#GroupName").val(),
            Minimumchargerate: parseFloat($("#Minimumchargerate").val()).toFixed(3),
            Date: new Date().toISOString()
        };

        let url = gasMiniCharge.ID == 0 ? "/Admin/ATGLRates/AddGasMiniCharge" : "/Admin/ATGLRates/UpdateGasMiniCharge";

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(gasMiniCharge),
            success: function (response) {
                alert(response.message);
                $("#addGasMiniChargeModal").modal("hide");
                $("#GasMiniChargeGrid").dxDataGrid("instance").refresh();
            }
        });
    });

    $("#openaddGasMiniChargeModal").on("click", function () {
        $("#gasMiniChargeForm")[0].reset(); // clear all inputs
        $("#GasMiniChargeID").val(""); // clear ID so it's treated as 'add'
        $("#GroupName").val(""); // optionally reset group dropdown
        $("#addGasMiniChargeModal").modal("show");
    });


    function editGasMiniCharge(data) {
        $("#GasMiniChargeID").val(data.ID);
        $("#GroupName").val(data.GroupName);
        $("#Minimumchargerate").val(parseFloat(data.Minimumchargerate).toFixed(3));
        $("#Date").val(data.Date.replace(" ", "T")); // Format for datetime-local input

        $("#addGasMiniChargeModal").modal("show");
    }

    function deleteGasMiniCharge(id) {
        if (confirm("Are you sure you want to delete this record?")) {
            $.post("/Admin/ATGLRates/DeleteGasMiniCharge", { id: id }, function (response) {
                alert(response.message);
                loadGasMiniCharge();
                $("#GasMiniChargeGrid").dxDataGrid("instance").refresh();
            });
        }
    }
});
