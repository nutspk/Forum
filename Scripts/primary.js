$(function () {
    $('.box-error').hide();
});

function Redirect(url) {
    if (IsEmpty(url)) {
        window.location.href = url;
    }
}

function IsEmpty(value) {
    if (value !== undefined && value != null && value != "") {
        return false;
    } else {
        return true;
    }
}

function submit() {
    $('#main').submit();
}

function imgError(image) {
    var fullPath = "../Files/Upload/Images/no-image.png";
    image.onerror = "";
    image.src = fullPath;
    return true;
}

function showError(msg) {
    var elem = $('.box-error');
    var elemMsg = $('.errorMsg');

    msg = IsEmpty(msg) ? "Unknows Error !" : msg;
    
    elemMsg.html(msg);

    elem.fadeIn(300).fadeOut(3000);
}



