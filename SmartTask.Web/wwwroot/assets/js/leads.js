/*
Author       : Dreamstechnologies
Template Name: Smarthr - Bootstrap Admin Template
*/
(function () {
    "use strict";
	
	//Leads Append

	// Attach click event to the "add-lead-phno" button
	$(document).on('click', '.add-modal-row', function() {
	 
  
		// Create the new HTML structure for the additional input and select
		var newRow = '<div class="row phone-add-row">'+
		'<div class="col-lg-8">'+
			'<div class="input-block mb-3">'+
			'<input class="form-control" type="text">'+
			'</div>'+
		'</div>'+
		'<div class="col-lg-4 d-flex align-items-end">'+
			'<div class="input-block w-100 mb-3 d-flex align-items-center">'+
			'<div class="w-100">'+
				'<select class="select">'+
				'<option>Work</option>'+
				'<option>Home</option>'+
				'</select>'+
			'</div>'+
			'<a href="#" class="avatar avatar-md rounded delete-phone text-primary"><i class="ti ti-trash"></i></a>'+
			'</div>'+
		'</div>'+
	  '</div>'+
	  '</div>';
	
		  
	
		  $(".lead-phno-col").append(newRow);
		  $('.select').select2({
			minimumResultsForSearch: -1,
			width: '100%'
		});
		  return false;
		 
	});
	
	  
	  // Remove phone
	$(document).on('click', '.delete-phone', function () {
		$(this).closest('.phone-add-row').remove();
		return false;
	});

	//email Append

	$(document).on('click', '.add-email-row', function() {
		var expandemail = '<div class="row email-add-row">'+
		  '<div class="col-lg-8">'+
			'<div class="input-block mb-3">'+
			  '<input class="form-control" type="text">'+
			'</div>'+
		  '</div>'+
		  '<div class="col-lg-4 d-flex align-items-end">'+
			'<div class="input-block w-100 mb-3 d-flex align-items-center">'+
			  '<div class="w-100">'+
				'<select class="select">'+
				  '<option>Work</option>'+
				  '<option>Home</option>'+
				'</select>'+
			  '</div>'+
			  '<a href="#" class="avatar avatar-md rounded delete-email text-primary"><i class="ti ti-trash"></i></a>'+
			'</div>'+
		  '</div>'+
	'</div>'+
	'</div>';
	$(".lead-email-col").append(expandemail);
	$('.select').select2({
		minimumResultsForSearch: -1,
		width: '100%'
	});
	return false;
  	});

	// Remove email
	$(document).on('click', '.delete-email', function () {
		$(this).closest('.email-add-row').remove();
		return false;
	});
	
})();