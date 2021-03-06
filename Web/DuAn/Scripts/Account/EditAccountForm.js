﻿
//EditAccountForm
var _avatar = new KTImageInput('kt_user_add_avatar');

$('#submitAccount').on('click', function () {
   if (CheckTotalAccount()) {

      var avatar = document.getElementById('avatar');
      var formData = new FormData();

      formData.append('avatar', avatar.files[0]);

      var id = document.getElementById('id').value;
      formData.append('id', id);

      var username = document.getElementById('username').value;
      formData.append('username', username);

      var password = document.getElementById('password').value;
      formData.append('password', password);

      var fullname = document.getElementById('fullname').value;
      formData.append('fullname', fullname);

      var phone = document.getElementById('phone').value;
      formData.append('phone', phone);

      var email = document.getElementById('email').value;
      formData.append('email', email);

      var address = document.getElementById('address').value;
      formData.append('address', address);

      var icode = document.getElementById('icode').value;
      formData.append('icode', icode);

      var dob = document.getElementById('dob').value;
      formData.append('dob', dob);

      var roleID = document.getElementById('dropdownRoleButton').value;
      formData.append('roleID', roleID);

      var url = "";
      if (id != 0) {
         url = "/Account/UpdateAccount";
      } else {
         url = "/Account/InsertAccount";
      }

      $.ajax({
         cache: false,
         url: url,
         type: 'POST',
         processData: false,
         contentType: false,
         data: formData,
         success: function (data) {
            debugger
            if (data != "success") {
               showMessage(data, false);
            } else {
               reloadAccountDatatable();
               showMessage('Thành công !!!', true);
               reloadAvatarUser();
            }
         },
         error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status == 401) {
               showMessage('Bạn không có quyền này', false);
            } else {
               showMessage(jqXHR.responseText, false);
            }
         }
      });
   }
});
function hideError(name) {
   document.getElementById(name).innerText = "";
}
function CheckTotalAccount() {
   var username = document.getElementById('username').value;
   var password = document.getElementById('password').value;
   var fullname = document.getElementById('fullname').value;
   var phone = document.getElementById('phone').value;
   var email = document.getElementById('email').value;
   var address = document.getElementById('address').value;
   var dob = document.getElementById('dob').value;
   var identifycode = document.getElementById("icode").value;

   if (username == "" || username == null) {
      document.getElementById("errorUsername").innerText = "Thông tin không được bỏ trống";

   }
   if (password == "" || password == null) {
      document.getElementById("errorPassword").innerText = "Thông tin không được bỏ trống";

   }
   if (fullname == "" || fullname == null) {
      document.getElementById("errorFullname").innerText = "Thông tin không được bỏ trống";

   }
   if (phone == "" || phone == null) {
      document.getElementById("errorPhone").innerText = "Thông tin không được bỏ trống";

   }
   if (email == "" || email == null) {
      document.getElementById("errorEmail").innerText = "Thông tin không được bỏ trống";

   }
   if (address == "" || address == null) {
      document.getElementById("errorAddress").innerText = "Thông tin không được bỏ trống";

   }
   if (identifycode == "" || identifycode == null) {
      document.getElementById("errorIdentifyCode").innerText = "Thông tin không được bỏ trống";

   }
   if (dob == "" || dob == null) {
      document.getElementById("errorDOB").innerText = "Thông tin không được bỏ trống";

   }
   var error = document.getElementsByClassName("errorRegistrationAccount");
   for (var i = 0; i < error.length; i++) {
      if (error[i].innerText != "" && error[i].innerText != null) {
         return false;
      }
   }
   return true;
}

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
         showMessage('Lỗi lấy dữ liệu vai trò', false);
      }
   });
});
