$(function () {
    LoadMyProfiles();
});

function LoadMyProfiles() {
    $.getJSON("api/v1/Members/Me", function (data) {
        console.log(data);
        //$.each(data, function (idx, val) {
        //    $("#Title").html(val.Title);
        //    $("#DisplayName").html(val.DisplayName);

        //    $("[id$=Avatar]").attr('src', "<%=@ResolveClientUrl("~/Files/Upload / Avatar / ")%>" + val.Avatar);
        //});
    });
}



