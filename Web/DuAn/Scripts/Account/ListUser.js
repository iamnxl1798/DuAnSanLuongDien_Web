
//ListUser
jQuery('#newaccount').on('click', function () {
   $.ajax({
      url: "/Account/CreateAccountForm",
      type: 'POST',
      success: function (data) {
         $('#formEditAccount').html(data);
         $('#formEditAccount').modal('show');
         $('#formEditAccount').modal({
            backdrop: false
         });
      },
      error: function (jqXHR, textStatus, errorThrown) {
         if (jqXHR.status == 401) {
            showMessage('Bạn không có quyền này', false);
         } else {
            showMessage('Đã xảy ra lỗi trong quá trình chỉnh sửa/ thêm mới tài khoản ' + jqXHR.responseText, false);
         }
      }
   });
});


jQuery(document).ready(function () {
   $.ajax({
      url: "/Account/TableDataUser",
      type: 'POST',
      success: function (data) {
         $('#datatable_account_ajax').html(data);
         /*@* $('#test_ajax').modal('show');*@*/
      },
      error: function (data) {
         showMessage('Đã xả ra lỗi trong quá trình lấy dữ liệu bảng', false);
      }
   });
   $('#reloadAccountTable').on('click', function (e) {
      e.preventDefault();
      $('#my_datatable_account').DataTable().destroy();
      loadDataTableAccount();
   });
});

