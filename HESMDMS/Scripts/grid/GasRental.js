$(document).ready(function () {
    loadGasRental();
    loadGroups();

    function loadGasRental() {
        $("#GasRentalGrid").dxDataGrid({
            dataSource: "/Admin/ATGLRates/GetGasRental",
            keyExpr: "ID",
            paging: { pageSize: 10 },
            columns: [
                { dataField: "ID", caption: "ID", width: 50, visible: false },
                { dataField: "GroupName", caption: "Group Name" },
                { dataField: "Rental", caption: "Rental", format: "#,##0.000" },
                { dataField: "Date", caption: "Date", dataType: "date", format: "yyyy-MM-dd HH:mm:ss" },
                {
                    caption: "Actions",
                    type: "buttons",
                    buttons: [
                        {
                            hint: "Edit",
                            icon: "edit",
                            onClick: function (e) { editGasRental(e.row.data); }
                        },
                        {
                            hint: "Delete",
                            icon: "trash",
                            onClick: function (e) { deleteGasRental(e.row.data.ID); }
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


    $("#gasRentalForm").submit(function (e) {
        e.preventDefault();

        let gasRental = {
            ID: $("#GasRentalID").val() || 0,
            GroupName: $("#GroupName").val(),
            Rental: parseFloat($("#Rental").val()).toFixed(3),
            Date: new Date().toISOString()
        };

        let url = gasRental.ID == 0 ? "/Admin/ATGLRates/AddGasRental" : "/Admin/ATGLRates/UpdateGasRental";

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(gasRental),
            success: function (response) {
                alert(response.message);
                $("#addGasRentalModal").modal("hide");
                $("#GasRentalGrid").dxDataGrid("instance").refresh();
            }
        });
    });

    $("#openaddGasRentalModal").on("click", function () {
        $("#gasRentalForm")[0].reset(); // clear all inputs
        $("#GasRentalID").val(""); // clear ID so it's treated as 'add'
        $("#GroupName").val(""); // optionally reset group dropdown
        $("#addGasRentalModal").modal("show");
    });


    function editGasRental(data) {
        $("#GasRentalID").val(data.ID);
        $("#GroupName").val(data.GroupName);
        $("#Rental").val(parseFloat(data.Rental).toFixed(3));
        $("#Date").val(data.Date.replace(" ", "T")); // Format for datetime-local input

        $("#addGasRentalModal").modal("show");
    }

    function deleteGasRental(id) {
        if (confirm("Are you sure you want to delete this record?")) {
            $.post("/Admin/ATGLRates/DeleteGasRental", { id: id }, function (response) {
                alert(response.message);
                loadGasRental();
                $("#GasRentalGrid").dxDataGrid("instance").refresh();
            });
        }
    }
});
