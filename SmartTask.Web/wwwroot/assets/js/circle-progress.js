/*
Author       : Dreamstechnologies
Template Name: Smarthr - Bootstrap Admin Template
*/
(function () {
    "use strict";
	
	$(".circle-progress").each(function() {
  
		var value = $(this).attr('data-value');
		var left = $(this).find('.progress-left .progress-bar');
		var right = $(this).find('.progress-right .progress-bar');
	
		if (value > 0) {
		  if (value <= 50) {
			right.css('transform', 'rotate(' + percentageToDegrees(value) + 'deg)')
		  } else {
			right.css('transform', 'rotate(180deg)')
			left.css('transform', 'rotate(' + percentageToDegrees(value - 50) + 'deg)')
		  }
		}
	
	  })
	
	  function percentageToDegrees(percentage) {  
		return percentage / 100 * 360  
	  }
	  const bottomcenterToast2 = document.querySelectorAll('.delete-toast-btn');
	  const bottomcentertoastExample2 = document.querySelectorAll('.delete-toast');
  
	  bottomcenterToast2.forEach((a, index) => {
	  a.addEventListener('click', () => {
	  const toast = new bootstrap.Toast(bottomcentertoastExample2[index]);
	  toast.show();
	  });
	});

	
	$(".attendance-circle-progress").each(function() {
	  
		var value = $(this).attr('data-value');
		var left = $(this).find('.progress-left .progress-bar');
		var right = $(this).find('.progress-right .progress-bar');
	
		if (value > 0) {
		  if (value <= 50) {
			right.css('transform', 'rotate(' + percentageToDegrees(value) + 'deg)')
		  } else {
			right.css('transform', 'rotate(180deg)')
			left.css('transform', 'rotate(' + percentageToDegrees(value - 50) + 'deg)')
		  }
		}
	
	  })
	
	  function percentageToDegrees(percentage) {
	
		return percentage / 100 * 360
	
	}
	
})();