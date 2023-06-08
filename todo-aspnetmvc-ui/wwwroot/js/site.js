// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



//$(".sidebarItem").on('click', function () {
//    $(".sidebarItem.active").removeClass('active');
//    $(".submenuItem.active").removeClass('active');
//    $(this).addClass('active');
//});

//$(document).on('click', '.submenuItem', function () {
//    $(".submenuItem.active").removeClass('active');
//    $(".sidebarItem.active").removeClass('active');
//    $(this).addClass('active');
//});


$(".side-link").on('click', function () {
    $(this).find('.fa-chevron-down').toggleClass('rotate180');
})


$('.open-btn').on('click', function () {
    $('.sidebar').addClass('active');
});

$('.close-btn').on('click', function () {
    $('.sidebar').removeClass('active');
});

$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".sidebar .side-link").forEach(function (element) {
        element.addEventListener('click', function (e) {
            let nextEl = element.nextElementSibling;
            let parentEl = element.parentElement;

            if (nextEl) {
                e.preventDefault();
                let mycollapse = new bootstrap.Collapse(nextEl);

                if (nextEl.classList.contains('show')) {
                    mycollapse.hide();
                } else {
                    mycollapse.show();
                    let opened_submenu = parentEl.parentElement.querySelector('.submenu.show');
                    if (opened_submenu) {
                        new bootstrap.Collapse(opened_submenu);
                    }
                }
            }
        });

    });
});

$(document).ready(function () {
    $('#strikethroughCheckbox').change(function () {
        if ($(this).is(':checked')) {
            $('#textToStrike').addClass('strikethrough');
            console.log($('#markCompleteForm'))
            $('#hiddenSubmitButton').click();
        } else {
            $('#textToStrike').removeClass('strikethrough');
        }
    });

    var currentUrl = window.location.pathname;
    var $activeLink = $(".sidebarItem a[href='" + currentUrl + "']");
    $activeLink.closest(".sidebarItem").addClass("active");

    // Handle sidebar link clicks
    $(".sidebarItem").on("click", function () {
        $(".sidebarItem.active").removeClass("active");
        $(this).addClass("active");
    });

    $(document).on("click", ".submenuItem", function () {
        $(".submenuItem.active").removeClass("active");
        $(".sidebarItem.active").removeClass("active");
        $(this).addClass("active");
    });
});
