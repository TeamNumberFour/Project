$(document).ready(function () {
    var calc = ($('.left-part').width() - 235) + 'px';
    var calc2 = $('.left-part').width() + 'px';

    var timer = 0;


    //---Ввод универа---

    $('.form__search').on('click', function (event) {
        if ($('.choice__univer').val() == '') {
            event.preventDefault();
            $('.choice__univer').css('border-color', '#f00');
        } else {
            
            timer = new Date().getTime();
            $('#timer').animate({
                opacity: 1
            }, 2000);
            
        }
    })

    $('.title').click(function (e) {
        e.preventDefault();
        var dropDown = $(this).next();
        $('.dropdown').not(dropDown).hide('.4s');
        dropDown.slideToggle('.4s');
    });



    // ---Все соцсети---

    $('.viewAll').on('click', function () {
        var ad = $('.add').css('display');
        if (ad == 'none') {
            $('.add').css('display', 'block');
            $(this).text('Скрыть');
        } else {
            $('.add').css('display', 'none');
            $(this).text('Посмотреть все');
        }
    });



    //---Смена эмоциональной окраски---

    $('.coloring__main').on("click", function () {
        var ths = $(this).next().css('display');
        var oth = $('.other').css('display');

        $('.other').not($(this).next()).fadeOut('.1s');

        if (ths == 'none') {
            $(this).next().fadeIn('.1s');
        } else {
            $(this).next().fadeOut('.1s');
        }
    });


    $('.other__item').on('click', function () {
        var selected = $(this).children().attr('class');

        var current = $(this).parent().prev().children();
        var currentClass = $(this).parent().prev().children().attr('class');
        current.removeClass().addClass(selected);
        $(this).children().removeClass().addClass(currentClass);
        $('.other').fadeOut('.1s');
    });




    //---Появление/Исчезновение фильтров---

    $('.arrow > img').click(function () {
        $('.arrow > img').css('display', 'none');

        if ($('.right-part').css('right') != '0px') {
            $('.left-part').animate({
                width: calc
            }, 2000);

            $('.footer-buttons').animate({
                width: calc
            }, 2000);

            $('.right-part').animate({
                right: '0px'
            }, 2000);
        } else {

            $('.left-part').animate({
                width: calc2
            }, 2000);

            $('.footer-buttons').animate({
                width: calc2
            }, 2000);

            $('.right-part').animate({
                right: '-310px'
            }, 2000);
            $('.filters').trigger('reset');

        }

        setTimeout(function () {
            $('.arrow > img').css('display', 'block');
        }, 2000);
        $('.arrow > img').toggleClass('rotate');
    });




    //---Работа с комментами---

    jQuery(".list__text").each(function () {
        var review_full = jQuery(this).html();
        var review = review_full;

        if (review.length > 100) {
            review = review.substring(0, 100);
            jQuery(this).html('<div class="half_text">' + review + '<span class="read_more">...</span></div>');
        }

        jQuery(this).append('<div class="full_text" style="display: none;">' + review_full + '<span class="hide">Скрыть</span></div>');

    });

    jQuery(".read_more").click(function () {
        jQuery(this).parent().css('display', 'none');
        jQuery(this).parent().next().css('display', 'block');
		/*var aa = jQuery(this).parent().find(".full_text").html();
	    jQuery(this).parent().html( aa + ' <span class="read_more" style="display: none;">...</span>' );
*/
        jQuery(".hide").click(function () {
            //jQuery(this).parent().append('<div class="full_text" style="display: none;">' + aa + '</div>');
            //bb = aa;
            //bb = bb.substring(0, 100);
            //jQuery(this).parent().html( bb + '<span class="read_more">...</span><div class="full_text" style="display: none;">' + aa + '</div>');
            jQuery(this).parent().css('display', 'none');
            jQuery(this).parent().prev().css('display', 'block');
        });
    })



    //---Появление/Исчезновение кнопки "Экспорт"---

    $('.reset').on('click', function () {
        $('.export').css('display', 'none');
    })


    $(".submit").on('click', function () {
        $('input').each(function (index) {
            if ($('input:checkbox')[index].checked || ($(this).val() != '')) {
                $('.export').css('display', 'block');
                return false;
            }
        })
    })



    //---Смена фона---

    $('.bg').children().click(function () {
        var mainBg = $('.main-wrapper, .wrapper').parent().attr('class');
        var butBg = $(this).attr('class');

        $('.main-wrapper, .wrapper').parent().removeClass().addClass(butBg).removeClass('bg__common');
		/*if ($(this).hasClass('bg-red')) {
			$('.menu').css('background', 'linear-gradient(to left, #FF9C6B, #FA661E)');
		} else if ($(this).hasClass('bg-green')) {
			$('.menu').css('background', 'linear-gradient(to left, #C5FFDB, #59F359)');
		} else if ($(this).hasClass('bg-grey')) {
			$('.menu').css('background', 'linear-gradient(to left, #D2D2D2, #B4B3B3)');
		} else if ($(this).hasClass('bg-blue')) {
			$('.menu').css('background', 'linear-gradient(to left, #BFEAEA, #A7CECC)');
		}*/
        $(this).removeClass().addClass(mainBg).addClass('bg__common');
    });



    //---Добавление/Удаление пользователя---

	/*$(".create, .user__update").on('click', function() {
		$('.overlay').fadeIn(400, function() {
			$('.modal_form').css('display', 'block').animate({
				opacity: 1,
				top: '50%'
			}, 400);
		});
		if($(this).hasClass('create')) {
			$('.modal_form').append('<div class="accept">Принять</div>');

			$('.accept').on('click', function() {
				//var log = $('input[type="text"]:first').val();
				//var pas = $('input[type="text"]:last').val();
				//$('.content').append('<div class="user-item"><div class="user__number">+</div><div class="other-info"><div class="inf"><div class="user__login">' + log + '</div><div class="user__password">' + pas + '</div></div><div class="remake"><div class="user__update">Изменить</div><div class="user__delete">Удалить</div></div></div></div>');

				$('.modal_form').animate({
					opacity: 0,
					top: '45%'
				}, 	1000, function() {
					$(this).css('display', 'none');
					$('.overlay').fadeOut(100);
				});
				setTimeout(function() {
					$('.inputClear').val('');
					$('.accept').remove();
					$('.upd').remove();
				}, 1000);
			});
		} else {
			var login = $(this).parent().parent().find('.inf').find('.user__login');
			var password = $(this).parent().parent().find('.inf').find('.user__password');
			$('input[type="text"]:first').val(login.text());
			$('input[type="text"]:last').val(password.text());
			$('.modal_form').append('<div class="upd">Изменить</div>');
			
			$('.upd').on('click', function() {
				//login.text($('input[type="text"]:first').val());
				//password.text($('input[type="text"]:last').val());

				$('.modal_form').animate({
					opacity: 0,
					top: '45%'
				}, 	1000, function() {
					$(this).css('display', 'none');
					$('.overlay').fadeOut(100);
				});
				setTimeout(function() {
					$('.inputClear').val('');
					$('.accept').remove();
					$('.upd').remove();
				}, 1000);
			});
		}
	});


	$('.modal_close, .overlay').click(function() {
		$('.modal_form').animate({
			opacity: 0,
			top: '45%'
		}, 	1000, function() {
			$(this).css('display', 'none');
			$('.overlay').fadeOut(100);
		});
		setTimeout(function() {
			$('.inputClear').val('');
			$('.accept').remove();
			$('.upd').remove();
		}, 1000);
	});*/



    /*----------Секундомер-------------*/


    /*$('.form__search').click(function() {
    timer = new Date().getTime();
    });
    $('#stop').click(function() {
    timer = 0;
    });*/

    var interval = setInterval(function () {
        if (timer == 0) return;
        var sec = Math.floor((new Date().getTime() - timer) / 1000);
        var hours = Math.floor(sec / 3600);
        var minutes = Math.floor((sec - hours * 3600) / 60);
        var seconds = sec - hours * 3600 - minutes * 60;
        $('#timer').html('Время поиска:  ' + hours + 'h:' + minutes + 'm:' + seconds + 's');
    }, 50);
});
