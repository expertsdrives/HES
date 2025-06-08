$(document).ready(function () {
    loadGasVat();
    loadGroups();

    function loadGasVat() {
        $("#GasVatGrid").dxDataGrid({
            dataSource: "/Admin/ATGLRates/GetGasVAT",
            keyExpr: "ID",
            paging: { pageSize: 10 },
            columns: [
                { dataField: "ID", caption: "ID", width: 50, visible: false },
                { dataField: "GroupName", caption: "Group Name" },
                { dataField: "VAT", caption: "Gas VAT", format: "#,##0.000" },
                { dataField: "Date", caption: "Date", dataType: "date", format: "yyyy-MM-dd HH:mm:ss" },
                {
                    caption: "Actions",
                    type: "buttons",
                    buttons: [
                        {
                            hint: "Edit",
                            icon: "edit",
                            onClick: function (e) { editGasVat(e.row.data); }
                        },
                        {
                            hint: "Delete",
                            icon: "trash",
                            onClick: function (e) { deleteGasVAt(e.row.data.ID); }
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

    $("#gasVatForm").submit(function (e) {
        e.preventDefault();

        let gasvat = {
            ID: $("#GasVatID").val() || 0,
            GroupName: $("#GroupName").val(),
            VAT: parseFloat($("#VAT").val()).toFixed(3),
            Date: new Date().toISOString()
        };

        let url = gasvat.ID == 0 ? "/Admin/ATGLRates/AddGasVat" : "/Admin/ATGLRates/UpdateGasVat";

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(gasvat),
            success: function (response) {
                alert(response.message);
                $("#addGasVatModal").modal("hide");
                $("#GasVatGrid").dxDataGrid("instance").refresh();
            }
        });
    });


    $("#openAddGasVatModal").on("click", function () {
        $("#gasVatForm")[0].reset(); // clear all inputs
        $("#GasVatID").val(""); // clear ID so it's treated as 'add'
        $("#GroupName").val(""); // optionally reset group dropdown
        $("#addModal").modal("show");
    });

    function editGasVat(data) {
        $("#GasVatID").val(data.ID);
        $("#GroupName").val(data.GroupName);
        $("#VAT").val(parseFloat(data.VAT).toFixed(3));
        $("#Date").val(data.Date.replace(" ", "T")); // Format for datetime-local input

        $("#addGasVatModal").modal("show");
    }

    function deleteGasVAt(id) {
        if (confirm("Are you sure you want to delete this record?")) {
            $.post("/Admin/ATGLRates/DeleteGasVat", { id: id }, function (response) {
                alert(response.message);
                loadGasVat();
                $("#GasVatGrid").dxDataGrid("instance").refresh();
            });
        }
    }
});
