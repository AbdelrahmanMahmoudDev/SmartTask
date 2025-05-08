/*
Author       : Dreamstechnologies
Template Name: Smarthr - Bootstrap Admin Template
*/
(function () {
    "use strict";
	
	// Add Salary Settings

	$(document).on('click','.add-salary-btn',function(){

		var expensescontent = '<div class="row salary-add-row">' +
			'<div class="col-md-4">' +
				'<div class="mb-3">' +
					'<label class="form-label">Salary From</label>' +
					'<input type="text" class="form-control">' +
				'</div>' +
			'</div>' +
			'<div class="col-md-4">' +
				'<div class="mb-3">' +
					'<label class="form-label">Salary To</label>' +
					'<input type="text" class="form-control">' +
				'</div>' +
			'</div>' +
			' <div class="col-md-4">' +
				'<div class="d-flex align-items-center">' +
					'<div class="mb-3 flex-fill">' +
						'<label class="form-label">Percentage</label>' + 
						'<input type="text" class="form-control">' +
					'</div>' +
					'<div class="d-flex align-items-center pt-3 ms-3">' +
						'<a href="#" class="avatar avatar-md rounded bg-gray delete-salary text-primary"><i class="ti ti-trash"></i></a>' +
					'</div>' +
				'</div>' +
			'</div>' +
		'</div>' +
	'</div>' ;	
		'</div>'

		
		$(".add-salary-info").append(expensescontent);
		return false;
	});

	// Remove Salary
	$(document).on('click', '.delete-salary', function () {
		$(this).closest('.salary-add-row').remove();
		return false;
	});
	
})();