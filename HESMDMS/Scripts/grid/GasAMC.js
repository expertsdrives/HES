$(document).ready(function () {
    loadGasAMC();
    loadGroups();

    function loadGasAMC() {
        $("#GasAMCGrid").dxDataGrid({
            dataSource: "/Admin/ATGLRates/GetGasAMC",
            keyExpr: "ID",
            paging: { pageSize: 10 },
            columns: [
                { dataField: "ID", caption: "ID", width: 50, visible: false },
                { dataField: "GroupName", caption: "Group Name" },
                { dataField: "AMC", caption: "AMC", format: "#,##0.000" },
                { dataField: "Date", caption: "Date", dataType: "date", format: "yyyy-MM-dd HH:mm:ss" },
                {
                    caption: "Actions",
                    type: "buttons",
                    buttons: [
                        {
                            hint: "Edit",
                            icon: "edit",
                            onClick: function (e) { editGasAMC(e.row.data); }
                        },
                        {
                            hint: "Delete",
                            icon: "trash",
                            onClick: function (e) { deleteGasAMC(e.row.data.ID); }
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

    $("#gasAMCForm").submit(function (e) {
        e.preventDefault();

        let gasAMC = {
            ID: $("#GasAMCID").val() || 0,
            GroupName: $("#GroupName").val(),
            AMC: parseFloat($("#AMC").val()).toFixed(3),
            Date: new Date().toISOString()
        };

        let url = gasAMC.ID == 0 ? "/Admin/ATGLRates/AddGasAMC" : "/Admin/ATGLRates/UpdateGasAMC";

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(gasAMC),
            success: function (response) {
                alert(response.message);
                $("#addGasAMCModal").modal("hide");
                $("#GasAMCGrid").dxDataGrid("instance").refresh();
            }
        });
    });

    $("#openaddGasAMCModal").on("click", function () {
        $("#gasAMCForm")[0].reset(); // clear all inputs
        $("#GasAMCID").val(""); // clear ID so it's treated as 'add'
        $("#GroupName").val(""); // optionally reset group dropdown
        $("#addGasAMCModal").modal("show");
    });


    function editGasAMC(data) {
        $("#GasAMCID").val(data.ID);
        $("#GroupName").val(data.GroupName);
        $("#AMC").val(parseFloat(data.AMC).toFixed(3));
        $("#Date").val(data.Date.replace(" ", "T")); // Format for datetime-local input

        $("#addGasAMCModal").modal("show");
    }

    function deleteGasAMC(id) {
        if (confirm("Are you sure you want to delete this record?")) {
            $.post("/Admin/ATGLRates/DeleteGasAMC", { id: id }, function (response) {
                alert(response.message);
                loadGasAMC();
                $("#GasAMCGrid").dxDataGrid("instance").refresh();
            });
        }
    }
});
