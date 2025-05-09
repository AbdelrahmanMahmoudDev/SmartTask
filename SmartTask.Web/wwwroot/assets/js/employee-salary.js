/*
Author       : Dreamstechnologies
Template Name: Smarthr - Bootstrap Admin Template
*/
(function () {
    "use strict";
	
	//Earnings Append
	$(document).on('click', '.add-earnings', function() {
		var expandearning= 
		'<div class="row earning-add-row">'+
			'<div class="col-md-3">'+
				'<div class="mb-3">'+
					'<label class="form-label">Basic</label>'+
					'<input type="text" class="form-control">'+
				'</div>'+
			'</div>'+
			'<div class="col-md-3">'+
				'<div class="mb-3">'+
					'<label class="form-label">DA(40%)</label>'+
					'<input type="text" class="form-control">'+
				'</div>'+
			'</div>'+
			'<div class="col-md-3">'+
				'<div class="mb-3">'+
					'<label class="form-label">HRA(15%)</label>'+
					'<input type="text" class="form-control">' +                     
				'</div>'+
			'</div>'+
			'<div class="col-md-3 d-flex align-items-center">'+
				'<div class="mb-3">'+
					'<label class="form-label">Conveyance</label>'+
					'<input type="text" class="form-control">'+
				'</div>'+
			'</div>'+
			'<div class="col-md-3">'+
                                    '<div class="mb-3">'+
                                        '<label class="form-label">Allowance </label>'+
                                        '<input type="text" class="form-control">'+                                       
                                    '</div>'+									
                                '</div>'+
                                '<div class="col-md-3">'+
                                    '<div class="mb-3">'+
                                        '<label class="form-label">Medical Allowance</label>'+
                                        '<input type="text" class="form-control">'+                                       
                                    '</div>'+									
                                '</div>'+
                                '<div class="col-md-3">'+
                                    '<div class="mb-3">'+
                                        '<label class="form-label">Others</label>'+
										'<div class="d-flex align-items-center">'+
                                        '<input type="text" class="form-control"> '+    
										'<a href="#" class="link-danger ms-2 delete-earning"><i class="far fa-trash-alt"></i></a>'+        
										'<div>'+                          
                                    '</div>'+						
                                '</div>'+
		'</div>';
		$(".earning-row").append(expandearning);
		$('.select').select2({
		minimumResultsForSearch: -1,
		width: '100%'
	});
		return false;
	
	});

	// Remove earning
	$(document).on('click', '.delete-earning', function () {
		$(this).closest('.earning-add-row').remove();
		return false;
	});

	//Deduction Append
	$(document).on('click', '.add-deduction', function() {
		var expanddeduction= 
		'<div class="row deduction-add-row">'+
			'<div class="col-md-3">'+
				'<div class="mb-3">'+
					'<label class="form-label">TDS</label>'+
					'<input type="text" class="form-control">'+
				'</div>'+
			'</div>'+
			'<div class="col-md-3">'+
				'<div class="mb-3">'+
					'<label class="form-label">ESI</label>'+
					'<input type="text" class="form-control">'+
				'</div>'+
			'</div>'+
			'<div class="col-md-3">'+
				'<div class="mb-3">'+
					'<label class="form-label">PF</label>'+
					'<input type="text" class="form-control">' +                     
				'</div>'+
			'</div>'+
			'<div class="col-md-3 d-flex align-items-center">'+
				'<div class="mb-3">'+
					'<label class="form-label">Leave</label>'+
					'<input type="text" class="form-control">'+
				'</div>'+
			'</div>'+
			'<div class="col-md-3">'+
				'<div class="mb-3">'+
					'<label class="form-label">Prof.Tax</label>'+
					'<input type="text" class="form-control">'+                               
				'</div>	'+								
			'</div>'+
			'<div class="col-md-3">'+
				'<div class="mb-3">'+
					'<label class="form-label">Labour Welfare</label>'+
					'<input type="text" class="form-control">'+                                       
				'</div>'+									
			'</div>'+
			'<div class="col-md-3">'+
				'<div class="mb-3">'+
					'<label class="form-label">Others</label>'+
					'<div class="d-flex align-items-center">'+
					'<input type="text" class="form-control">'+
					'<a href="#" class="link-danger ms-2 delete-deduction"><i class="far fa-trash-alt"></i></a>'+
					'</div>'+
				'</div>'+									
			'</div>'+
		'</div>';
		$(".deduction-row").append(expanddeduction);
		$('.select').select2({
		minimumResultsForSearch: -1,
		width: '100%'
	});
		return false;
	
	});

	// Remove earning
	$(document).on('click', '.delete-deduction', function () {
		$(this).closest('.deduction-add-row').remove();
	return false;
	});
	
})();