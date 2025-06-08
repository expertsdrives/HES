$(document).ready(function () {
    loadGasSurcharge();
    loadGroups();

    function loadGasSurcharge() {
        $("#GasSurchargeGrid").dxDataGrid({
            dataSource: "/Admin/ATGLRates/GetGasSurcharge",
            keyExpr: "ID",
            paging: { pageSize: 10 },
            columns: [
                { dataField: "ID", caption: "ID", width: 50, visible: false },
                { dataField: "GroupName", caption: "Group Name" },
                { dataField: "Surcharge", caption: "Gas Surcharge", format: "#,##0.000" },
                { dataField: "Date", caption: "Date", dataType: "date", format: "yyyy-MM-dd HH:mm:ss" },
                {
                    caption: "Actions",
                    type: "buttons",
                    buttons: [
                        {
                            hint: "Edit",
                            icon: "edit",
                            onClick: function (e) { editGasSurcharge(e.row.data); }
                        },
                        {
                            hint: "Delete",
                            icon: "trash",
                            onClick: function (e) { deleteGasSurcharge(e.row.data.ID); }
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

   

    $("#gasSurchargeForm").submit(function (e) {
        e.preventDefault();

        let gasSurcharge = {
            ID: $("#GasSurchargeID").val() || 0,
            GroupName: $("#GroupName").val(),
            Surcharge: parseFloat($("#Surcharge").val()).toFixed(3),
            Date: new Date().toISOString()
        };

        let url = gasSurcharge.ID == 0 ? "/Admin/ATGLRates/AddGasSurcharge" : "/Admin/ATGLRates/UpdateGasSurcharge";

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(gasSurcharge),
            success: function (response) {
                alert(response.message);
                $("#addGasSurchargeModal").modal("hide");
                $("#GasSurchargeGrid").dxDataGrid("instance").refresh();
            }
        });
    });

    $("#openaddGasSurchargeModal").on("click", function () {
        $("#gasSurchargeForm")[0].reset(); // clear all inputs
        $("#GasSurchargeID").val(""); // clear ID so it's treated as 'add'
        $("#GroupName").val(""); // optionally reset group dropdown
        $("#addGasSurchargeModal").modal("show");
    });


    function editGasSurcharge(data) {
        $("#GasSurchargeID").val(data.ID);
        $("#GroupName").val(data.GroupName);
        $("#Surcharge").val(parseFloat(data.Surcharge).toFixed(3));
        $("#Date").val(data.Date.replace(" ", "T")); // Format for datetime-local input

        $("#addGasSurchargeModal").modal("show");
    }

    function deleteGasSurcharge(id) {
        if (confirm("Are you sure you want to delete this record?")) {
            $.post("/Admin/ATGLRates/DeleteGasSurcharge", { id: id }, function (response) {
                alert(response.message);
                loadGasSurcharge();
                $("#GasSurchargeGrid").dxDataGrid("instance").refresh();
            });
        }
    }
});
