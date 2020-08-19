"use strict"
function capNhatPageDirectory(title, list) {
    $('#pageTitle').text(title);
    $('#pageDirectory').html(
        '<a href="javascript:;" class="opacity-75 hover-opacity-100" >\
                    <i class="flaticon2-shelter text-white icon-1x"></i>\
             </a>'
    );
    list.forEach(function (item, index, array) {
        $('#pageDirectory').append(
            '<span class="label label-dot label-sm bg-white opacity-75 mx-3"></span>\
                 <a href="javascript:;" class="text-white text-hover-white opacity-75 hover-opacity-100" id="pageDirectory">'+ item + '</a>'
        );
    });
}