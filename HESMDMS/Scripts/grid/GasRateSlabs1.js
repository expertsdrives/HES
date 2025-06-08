$(document).ready(function () {
    loadGasRates();
    loadGroups();
   
    function loadGasRates() {
        $("#GasRateGrid").dxDataGrid({
            dataSource: "/Admin/ATGLRates/GetGasRates",
            keyExpr: "ID",
            columns: [
                { dataField: "ID", caption: "ID", visible:false },
                { dataField: "GroupName", caption: "Group Name" },
                { dataField: "GasRateSlab1", caption: "Gas Rate Slab 1", format: "#,##0.000" },
                { dataField: "Date", caption: "Date", dataType: "date", format: "yyyy-MM-dd HH:mm:ss" },
                {
                    caption: "Actions",
                    type: "buttons",
                    buttons: [
                        {
                            hint: "Edit",
                            icon: "edit",
                            onClick: function (e) { editGasRate(e.row.data); }
                        },
                        {
                            hint: "Delete",
                            icon: "trash",
                            onClick: function (e) { deleteGasRate(e.row.data.ID); }
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


    $("#gasRateForm").submit(function (e) {
        e.preventDefault();

        let gasRate = {
            ID: $("#GasRateID").val() || 0,
            GroupName: $("#GroupName").val(),
            GasRateSlab1: parseFloat($("#GasRateSlab1").val()).toFixed(3), // Ensure 3 decimal places
            Date: new Date().toISOString()
        };

        let url = gasRate.ID == 0 ? "/Admin/ATGLRates/AddGasRate" : "/Admin/ATGLRates/UpdateGasRate";

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(gasRate),
            success: function (response) {
                alert(response.message);
                $("#addModal").modal("hide");
                $("#GasRateGrid").dxDataGrid("instance").refresh();
            }
        });
    });


    $("#openAddGasRateModal").on("click", function () {
        $("#gasRateForm")[0].reset(); // clear all inputs
        $("#GasRateID").val(""); // clear ID so it's treated as 'add'
        $("#GroupName").val(""); // optionally reset group dropdown
        $("#addModal").modal("show");
    });

    function editGasRate(data) {
        $("#GasRateID").val(data.ID);
        $("#GroupName").val(data.GroupName);
        $("#GasRateSlab1").val(parseFloat(data.GasRateSlab1).toFixed(3)); // Ensure correct formatting
        $("#Date").val(data.Date.replace(" ", "T")); // Format for datetime-local input

        $("#addModal").modal("show");
    }

    function deleteGasRate(id) {
        if (confirm("Are you sure you want to delete this record?")) {
            $.post("/Admin/ATGLRates/DeleteGasRate", { id: id }, function (response) {
                alert(response.message);
                loadGasRates();
                $("#GasRateGrid").dxDataGrid("instance").refresh();
            });
        }
    }
});
