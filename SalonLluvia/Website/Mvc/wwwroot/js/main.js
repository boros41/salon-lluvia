(function ($) {
    "use strict";

    // Spinner
    var spinner = function () {
        setTimeout(function () {
            if ($('#spinner').length > 0) {
                $('#spinner').removeClass('show');
            }
        }, 1);
    };
    spinner();
    
    
    // Initiate the wowjs
    // new WOW().init();


    // Sticky Navbar
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('.sticky-top').addClass('shadow-sm').css('top', '0px');
        } else {
            $('.sticky-top').removeClass('shadow-sm').css('top', '-150px');
        }
    });
    
    
    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.back-to-top').fadeIn('slow');
        } else {
            $('.back-to-top').fadeOut('slow');
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({scrollTop: 0}, 1500, 'easeInOutExpo');
        return false;
    });

    // Facts counter https://github.com/inorganik/CountUp.js?tab=readme-ov-file#umd-module
    if ($("#counter-yoe").length > 0) {
        window.onload = function () {
            const counterOptions = {
                autoAnimate: true
            };
            const numAnim = new countUp.CountUp('counter-yoe', 15, counterOptions);
        };
    }

    // Solution for Bootstrap modal issue: https://github.com/twbs/bootstrap/issues/41005#issuecomment-3118196916
    window.addEventListener('hide.bs.modal', event => {
        event.target.inert = true
    });

    window.addEventListener('show.bs.modal', event => {
        event.target.inert = false
    });
})(jQuery);

