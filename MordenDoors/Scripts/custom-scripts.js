$(function () {
    $(".form-control").on('blur', function () {
        if ($(this).val() == '') {
            $(this).nextAll('label').eq(0).removeClass('active');
        } else {
            $(this).nextAll('label').eq(0).addClass('active');
        }
    });

    $(".form-control").on('click', function () {
        $(this).nextAll('label').eq(0).addClass('active');
    });
    populate_material();
    $(".form-control").on("change", function () {
        populate_material();
    });
});

function populate_material() {
    $.each($(".form-control"), function (i, v) {
        if ($(v).val() == '') {
            $(v).nextAll('label').eq(0).removeClass('active');
        } else {
            $(v).nextAll('label').eq(0).addClass('active');
        }
    });
}

$(document).ready(function () {

    $('.toggle-sidebar').on('click', function () {
        $(this).toggleClass('Mhide');
        $('.side-bar').toggleClass('hide-sidebar');
        $('.body-content').toggleClass('adjust');
    })

    $('.navbar-toggle').on('click', function () {
        $(this).toggleClass('menu-collapse');
    })
    
})