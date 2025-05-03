/*
Author       : Dreamstechnologies
Template Name: Smarthr - Bootstrap Admin Template
*/

(function () {
    "use strict";
	
	//Channel Logo Slider
	if ($('.channels-slider').length > 0) {
		$('.channels-slider').owlCarousel({
			loop: true,
			margin: 24,
			dots: false,
			nav: true,
			smartSpeed: 2000,
			navContainer: '.custom-nav',
			navText: [
				'<i class="ti ti-arrow-narrow-left"></i>',
				'<i class="ti ti-arrow-narrow-right"></i>'
			],
			responsive: {
				0: {
					items: 3
				},
				768: {
					items: 8
				},
				1300: {
					items: 8
				}
			}
		})
	}

	//Social Gallery Slider
	if ($('.social-gallery-slider').length > 0) {
		$('.social-gallery-slider').owlCarousel({
			loop: true,
			margin: 8,
			dots: false,
			nav: false,
			smartSpeed: 2000,
			responsive: {
				0: {
					items: 2
				},
				768: {
					items: 3
				},
				1300: {
					items: 4
				}
			}
		})
	}

})();