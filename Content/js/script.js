$(function() {
    // nav
    $(".nav-btn").on('click', function() {
        $("#wrap").toggleClass("aside-toggle");
        $(".nav-item button").removeClass("is-active");
        $(".nav-sub").slideUp();
    });

    $(".header-nav").on('click', function() {
        $("#wrap").toggleClass("aside-toggle");
        $(".nav-item button").removeClass("is-active");
        $(".nav-sub").slideUp();
    });

    $(".nav-item button").on("click", function(e){
        $(this).toggleClass("is-active");
        $(this).next(".nav-sub").slideToggle();
    });

    // user
    $(".user-text").on('click', function() {
        $(".user-wrap").toggleClass("is-active");
        $(".user-list").slideToggle();
        $(".filter-box").removeClass("is-active");
        $(".tool-search").removeClass("is-active");
        $("body, html").toggleClass("no-scroll");
        $(".overlay").remove();
        if($(".user-wrap").hasClass("is-active")) {
            $(".user-wrap").append("<div class='overlay'></div>");
        }
        else {
            $(".overlay").remove();
        }
    });

    // search
    $("#toolSearchBtn").on('click', function() {
        $(".tool-search").toggleClass("is-active");
        $(".filter-box").removeClass("is-active");
        $(".user-wrap").removeClass("is-active");
        $(".user-list").css("display", "none");
    });

    // filter
    $(".filter-btn").on('click', function() {
        $(".filter-box").toggleClass("is-active");
        $(".user-wrap").removeClass("is-active");
        $(".user-list").css("display", "none");
        $(".tool-search").removeClass("is-active");
        $("body, html").toggleClass("no-scroll");
        $(".overlay").remove();
        if($(".filter-box").hasClass("is-active")) {
            $(".tool-filter").append("<div class='overlay' style='z-index: 100;'></div>");
        }
        else {
            $(".overlay").remove();
        }
    });
    $(".btn-filter-submit").on('click', function() {
        $(".filter-box").removeClass("is-active");
    });

    // check
    $("#checkAll").on('click', function() {
        var checkVal = $('#checkAll').is(":checked");
        if(checkVal == true){
            $(".list-checkbox").attr("checked", true);
            $(".list-checkbox").prop("checked", true);
        }
        else {
            $(".list-checkbox").attr("checked", false);
            $(".list-checkbox").prop("checked", false);
        }
    });


    // gotop
    $(window).scroll(function() {
        if ($(this).scrollTop() > 240) {
            $(".btn-top").fadeIn();
        } else {
            $(".btn-top").fadeOut();
        }
    });
    $(".btn-top").click(function() {
        $("body, html").animate({
            scrollTop: 0
        }, 500);
    });
});

$(document).on('click', '.overlay', function(){
    $(this).remove();
    $(".user-wrap").removeClass("is-active");
    $(".user-list").css("display", "none");
    $(".filter-box").removeClass("is-active");
    $("body, html").removeClass("no-scroll");
});

if ($(window).width() < 1024) {
    $("#wrap").removeClass("aside-toggle");
    $(".nav-item button").removeClass("is-active");
    $(".nav-sub").slideUp();
}
$(window).on('resize', function(){
    var win = $(this);
    if (win.width() < 1024) {
        $("#wrap").removeClass("aside-toggle");
        $(".nav-item button").removeClass("is-active");
        $(".nav-sub").slideUp();
    }
});