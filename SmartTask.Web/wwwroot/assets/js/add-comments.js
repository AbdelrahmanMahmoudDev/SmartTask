/*
Author       : Dreamstechnologies
Template Name: Smarthr - Bootstrap Admin Template
*/
(function () {
    "use strict";
	
	// Add Comment

	if($('.add-comment').length > 0) {
		$(".add-comment").on("click", function(a) {
		$(this).closest(".notes-editor").children(".note-edit-wrap").slideToggle();
		});
		$(".add-cancel").on("click", function(a) {
		$(this).closest(".note-edit-wrap").slideUp();
		});
	}

	$(document).on('click','.add-sign',function(){

		var signcontent = '<div class="row sign-cont">' +
			'<div class="col-md-6">' +
				'<div class="mb-3">' +
					'<input class="form-control" type="text">' +
				'</div>' +
			'</div>' +
			'<div class="col-md-6">' +
				'<div class="d-flex align-items-center mb-3">' +    
					'<div class="flex-fill me-2">' +    
					'<input class="form-control" type="text">' +    
					'</div>' +
					'<div class="input-btn">' +
						'<a href="javascript:void(0);" class="btn btn-icon btn-sm text-primary trash-sign"><i class="ti ti-trash"></i></a>' +
					'</div>' +
				'</div>' +
			'</div>' +
		'</div>';
		$(".sign-content").append(signcontent);
		return false;
	});
	
	// Remove Sign
	$(document).on('click', '.trash-sign', function () {
		$(this).closest('.sign-cont').remove();
		return false;
	});

})();