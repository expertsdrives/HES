$(document).ready(function () {
    loadGasQtySlabs();
    loadGroups();

    function loadGasQtySlabs() {
        $("#GasQtySlabGrid").dxDataGrid({
            dataSource: "/Admin/ATGLRates/GetGasQtySlabs2",
            keyExpr: "ID",
            paging: { pageSize: 10 },
            columns: [
                { dataField: "ID", caption: "ID", width: 50, visible: false },
                { dataField: "GroupName", caption: "Group Name" },
                { dataField: "GasqtySlab2", caption: "Gas Quantity Slab 2", format: "#,##0.000" },
                { dataField: "Date", caption: "Date", dataType: "date", format: "yyyy-MM-dd HH:mm:ss" },
                {
                    caption: "Actions",
                    type: "buttons",
                    buttons: [
                        {
                            hint: "Edit",
                            icon: "edit",
                            onClick: function (e) { editGasQtySlab(e.row.data); }
                        },
                        {
                            hint: "Delete",
                            icon: "trash",
                            onClick: function (e) { deleteGasQtySlab(e.row.data.ID); }
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


    $("#gasQtySlabForm").submit(function (e) {
        e.preventDefault();

        let gasQtySlab = {
            ID: $("#GasQtySlabID").val() || 0,
            GroupName: $("#GroupName").val(),
            GasqtySlab2: parseFloat($("#GasqtySlab2").val()).toFixed(3),
            Date: new Date().toISOString()
        };

        let url = gasQtySlab.ID == 0 ? "/Admin/ATGLRates/AddGasQtySlab2" : "/Admin/ATGLRates/UpdateGasQtySlab2";

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(gasQtySlab),
            success: function (response) {
                alert(response.message);
                $("#addGasQtyModal").modal("hide");
                $("#GasQtySlabGrid").dxDataGrid("instance").refresh();
            }
        });
    });

    $("#openaddGasQtyModal").on("click", function () {
        $("#gasQtySlabForm")[0].reset(); // clear all inputs
        $("#GasQtySlabID").val(""); // clear ID so it's treated as 'add'
        $("#GroupName").val(""); // optionally reset group dropdown
        $("#addGasQtyModal").modal("show");
    });


    function editGasQtySlab(data) {
        $("#GasQtySlabID").val(data.ID);
        $("#GroupName").val(data.GroupName);
        $("#GasqtySlab2").val(parseFloat(data.GasqtySlab2).toFixed(3));
        $("#Date").val(data.Date.replace(" ", "T")); // Format for datetime-local input

        $("#addGasQtyModal").modal("show");
    }

    function deleteGasQtySlab(id) {
        if (confirm("Are you sure you want to delete this record?")) {
            $.post("/Admin/ATGLRates/DeleteGasQtySlab2", { id: id }, function (response) {
                alert(response.message);
                loadGasQtySlabs();
                $("#GasQtySlabGrid").dxDataGrid("instance").refresh();
            });
        }
    }
});
