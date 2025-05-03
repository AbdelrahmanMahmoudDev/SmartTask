/*
Author       : Dreamstechnologies
Template Name: Smarthr - Bootstrap Admin Template
*/
(function () {
    "use strict";
	
	// Otp Verfication  
	$('.digit-group').find('input').each(function () { 
		$(this).attr('maxlength', 1); 
		$(this).on('keyup', function (e) { 
		var parent = $($(this).parent()); 
		if (e.keyCode === 8 || e.keyCode === 37) { 
		var prev = parent.find('input#' + $(this).data('previous')); 
		if (prev.length) { $(prev).select(); 
		} 
		} 
		else if ((e.keyCode >= 48 && e.keyCode <= 57) || (e.keyCode >= 65 && e.keyCode <= 90) || (e.keyCode >= 96 && e.keyCode <= 105) || e.keyCode === 39) {  
		var next = parent.find('input#' + $(this).data('next')); 
		if (next.length) { 
		$(next).select(); 
		} else { 
		if (parent.data('autosubmit')) { 
		parent.submit(); 
		}
		} 
		} 
		}); 
		}); 
		$('.digit-group input').on('keyup', function () { 
		var self = $(this); 
		if (self.val() != '') { 
		self.addClass('active'); 
		} else { 
		self.removeClass('active'); 
		} 
	});
		
})();