
// load jstree
$(document).ready(function () {
    var RoleId = $('#idRoleAccount').attr('data-id');
    $.ajax({
        url: "/RoleAccount/PermissionTree",
        type: 'POST',
        data: {
            roleID: RoleId
        },
        success: function (data) {
            $('#jstree').jstree({
                plugins: ['checkbox'],
                'core': {
                    'data': data
                },
            }).on('ready.jstree', function () {
                $('#jstree').jstree("open_all");
            });
        },
        error: function (data) {
            alert("Error load permission");
        }
    });
});
// update or insert Role
$('#submit').on('click', function () {
    if (CheckTotal()) {
        var RoleId = $('#idRoleAccount').attr('data-id');
        if (RoleId != 0) {
            var url = "/RoleAccount/UpdateRole";
        } else {
            var url = "/RoleAccount/InsertRole";
        }

        //get all roleid chosen
        var checked_ids = [];
        var selectedNodes = $('#jstree').jstree("get_selected", true);
        $.each(selectedNodes, function () {
            checked_ids.push(this.id);
        });
        var rolename = document.getElementById("role-name").value;

        $.ajax({
            url: url,
            type: 'POST',
            data: {
                RoleID: RoleId,
                RoleName: rolename,
                listPermissionID: checked_ids
            },
            success: function (data) {
                if (data != "success") {
                    document.getElementById('result').innerText = data;
                } else {
                    document.getElementById('result').innerText = 'Successfully';
                    reloadRoleDatatable();
                }
            },
            error: function (data) {
                document.getElementById('result').innerText = 'Error load insert / udapte role';
            }
        });
    }
});
function CheckTotal() {
    var errorRolename = document.getElementById('errorRolename').innerText;
    var rolename = document.getElementById('role-name').value;

    if (errorRolename != "" && errorRolename != null) {
        document.getElementById("result").innerText = "You need to change rolename";
    }
    if (rolename == "" || rolename == null) {
        document.getElementById("errorRolename").innerText = "You need to fill it";
        document.getElementById("result").innerText = "You need to fill rolename";
    }

    var error = document.getElementsByClassName("errorRegistration");
    for (var i = 0; i < error.length; i++) {
        if (error[i].innerText != "" && error[i].innerText != null) {
            return false;
        }
    }
    return true;
}