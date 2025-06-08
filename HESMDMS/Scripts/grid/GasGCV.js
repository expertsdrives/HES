$(document).ready(function () {
    loadGasGCV();
    loadGroups();

    function loadGasGCV() {
        $("#GasGCVGrid").dxDataGrid({
            dataSource: "/Admin/ATGLRates/GetGasGCV",
            keyExpr: "ID",
            paging: { pageSize: 10 },
            columns: [
                { dataField: "ID", caption: "ID", width: 50, visible: false },
                { dataField: "GroupName", caption: "Group Name" },
                { dataField: "GCV", caption: "GCV", format: "#,##0.000" },
                { dataField: "Date", caption: "Date", dataType: "date", format: "yyyy-MM-dd HH:mm:ss" },
                {
                    caption: "Actions",
                    type: "buttons",
                    buttons: [
                        {
                            hint: "Edit",
                            icon: "edit",
                            onClick: function (e) { editGasGCV(e.row.data); }
                        },
                        {
                            hint: "Delete",
                            icon: "trash",
                            onClick: function (e) { deleteGasGCV(e.row.data.ID); }
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

    $("#gasGCVForm").submit(function (e) {
        e.preventDefault();

        let gasGCV = {
            ID: $("#GasGCVID").val() || 0,
            GroupName: $("#GroupName").val(),
            GCV: parseFloat($("#GCV").val()).toFixed(3),
            Date: new Date().toISOString()
        };

        let url = gasGCV.ID == 0 ? "/Admin/ATGLRates/AddGasGCV" : "/Admin/ATGLRates/UpdateGasGCV";

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(gasGCV),
            success: function (response) {
                alert(response.message);
                $("#addGasGCVModal").modal("hide");
                $("#GasGCVGrid").dxDataGrid("instance").refresh();
            }
        });
    });

    $("#openaddGasGCVModal").on("click", function () {
        $("#gasGCVForm")[0].reset(); // clear all inputs
        $("#GasGCVID").val(""); // clear ID so it's treated as 'add'
        $("#GroupName").val(""); // optionally reset group dropdown
        $("#addGasGCVModal").modal("show");
    });


    function editGasGCV(data) {
        $("#GasGCVID").val(data.ID);
        $("#GroupName").val(data.GroupName);
        $("#GCV").val(parseFloat(data.GCV).toFixed(3));
        $("#Date").val(data.Date.replace(" ", "T")); // Format for datetime-local input

        $("#addGasGCVModal").modal("show");
    }

    function deleteGasGCV(id) {
        if (confirm("Are you sure you want to delete this record?")) {
            $.post("/Admin/ATGLRates/DeleteGasGCV", { id: id }, function (response) {
                alert(response.message);
                loadGasGCV();
                $("#GasGCVGrid").dxDataGrid("instance").refresh();
            });
        }
    }
});
