
//EditAccountForm
$('#submit').on('click', function () {
    if (CheckTotal()) {
        var id = document.getElementById('id').value;
        var username = document.getElementById('username').value;
        var fullname = document.getElementById('fullname').value;
        var phone = document.getElementById('phone').value;
        var email = document.getElementById('email').value;
        var address = document.getElementById('address').value;
        var icode = document.getElementById('icode').value;
        var dob = document.getElementById('dob').value;
        var roleID = $('#role_change').children("option:selected").val();

        if (id != 0) {
            url = "/Account/UpdateAccount";
        } else {
            url = "/Account/InsertAccount";
        }

        $.ajax({
            url: url,
            type: 'POST',
            data: {
                id: id,
                username: username,
                fullname: fullname,
                phone: phone,
                email: email,
                address: address,
                icode: icode,
                dob: dob,
                roleID: roleID
            },
            success: function (data) {
                if (data != "success") {
                    document.getElementById('result').innerText = data;
                } else {
                    document.getElementById('result').innerText = 'Successfully';
                    reloadAccountDatatable();
                }
            },
            error: function (data) {
                document.getElementById('result').innerText = 'Error load insert/udapte accout';
                /*@* alert("Error load insert/udapte accout");*@*/
                }
        });
    }
});
function hideError(name) {
    document.getElementById(name).innerText = "";
    document.getElementById('result').innerText = "";
}
function CheckTotal() {
    var username = document.getElementById('username').value;
    var fullname = document.getElementById('fullname').value;
    var phone = document.getElementById('phone').value;
    var email = document.getElementById('email').value;
    var address = document.getElementById('address').value;
    var dob = document.getElementById('dob').value;
    var identifycode = document.getElementById("icode").value;

    if (username == "" || username == null) {
        document.getElementById("errorUsername").innerText = "You need to fill it";

    }
    if (fullname == "" || fullname == null) {
        document.getElementById("errorFullname").innerText = "You need to fill it";

    }
    if (phone == "" || phone == null) {
        document.getElementById("errorPhone").innerText = "You need to fill it";

    }
    if (email == "" || email == null) {
        document.getElementById("errorEmail").innerText = "You need to fill it";

    }
    if (address == "" || address == null) {
        document.getElementById("errorAddress").innerText = "You need to fill it";

    }
    if (identifycode == "" || identifycode == null) {
        document.getElementById("errorIdentifyCode").innerText = "You need to fill it";

    }
    if (dob == "" || dob == null) {
        document.getElementById("errorDOB").innerText = "You need to fill it";

    }
    var error = document.getElementsByClassName("errorRegistration");
    for (var i = 0; i < error.length; i++) {
        if (error[i].innerText != "" && error[i].innerText != null) {
            return false;
        }
    }
    return true;
}

// if edit

$(document).ready(function () {
    var url = "/RoleAccount/AllRole";
    var role = $('#all-role').attr('data-role');
    $.ajax({
        url: url,
        type: 'POST',
        data: {
            role: role
        },
        success: function (data) {
            $('#all-role').append(data);
        },
        error: function (data) {
            alert("Error load ajax get role");
        }
    });
});