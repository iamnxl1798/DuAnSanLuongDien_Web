"use strict"
$(document).ready(function () {
    var roid = document.getElementById("permissionRole").value;
    if (roid == "" || roid == null) {
        document.getElementById("adminPage").style.display = "none";
    }
});
