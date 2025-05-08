/*
Author       : Dreamstechnologies
Template Name: Smarthr - Bootstrap Admin Template
*/

(function () {
    "use strict";
	
	// Kanban Drag

	if($('.kanban-drag-wrap').length > 0) {
		$(".kanban-drag-wrap").sortable({
			connectWith: ".kanban-drag-wrap",
			handle: ".kanban-card",
			placeholder: "drag-placeholder"
		});
	}

})();