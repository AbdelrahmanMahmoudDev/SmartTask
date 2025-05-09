/*
Author       : Dreamstechnologies
Template Name: Smarthr - Bootstrap Admin Template
*/
(function () {
    "use strict";
	
	// Budgets
	$(document).on('click','.add-revenue',function(){

		var revenuescontent = '<div class="row align-items-end revenues-cont">' +
			'<div class="col-md-6">' +
				'<div class="mb-3">' +
					'<input type="text" class="form-control">' +
				'</div>' +
			'</div>' +
			'<div class="col-md-6">' +
				'<div class="d-flex align-items-center mb-3">' +
					'<div>' +
						'<div class="d-flex align-items-center">' + 
							'<input type="text" class="form-control">' +
							'<div class="ms-2">' +
								'<a href="javascript:void(0);" class="btn btn-icon trash-revenue btn-sm btn-primary rounded-circle"><i class="ti ti-trash"></i></a>' +
							'</div>' +
						'</div>' +
					'</div>' +
				'</div>' +
			'</div>' +		
		'</div>';

		
		$(".revenues-content").append(revenuescontent);
		return false;
	});

	// Remove Budget
	$(document).on('click', '.trash-revenue', function () {
		$(this).closest('.revenues-cont').remove();
		return false;
	});

	// Add Expense
	$(document).on('click','.add-expenses',function(){

		var expensescontent = '<div class="row align-items-end expenses-cont">' +
			'<div class="col-md-6">' +
				'<div class="mb-3">' +
					'<input type="text" class="form-control">' +
				'</div>' +
			'</div>' +
			'<div class="col-md-6">' +
				'<div class="d-flex align-items-center mb-3">' +
					'<div>' +
						'<div class="d-flex align-items-center">' + 
							'<input type="text" class="form-control">' +
							'<div class="ms-2">' +
								'<a href="javascript:void(0);" class="btn btn-icon trash-expenses btn-sm btn-primary rounded-circle"><i class="ti ti-trash"></i></a>' +
							'</div>' +
						'</div>' +
					'</div>' +
				'</div>' +
			'</div>' +		
		'</div>';

		$(".expenses-content").append(expensescontent);
		return false;
	});

	// Remove Expense
	$(document).on('click', '.trash-expenses', function () {
		$(this).closest('.expenses-cont').remove();
		return false;
	});
		
})();