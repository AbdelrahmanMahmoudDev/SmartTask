/*
Author       : Dreamstechnologies
Template Name: Smarthr - Bootstrap Admin Template
*/
(function () {
    "use strict";
	
	// Image Slider
	if($('.media-images-slider').length > 0 ){
		$('.media-images-slider').owlCarousel({
			items: 1,
			margin: 15,
			dots : false,
			nav: true,
			loop: false,
			responsiveClass:true,
			responsive: {
				0: {
					items: 2
				},
				800 : {
					items: 5
				},
				1170: {
					items: 7
				}
			}
		});
	}
	
})();