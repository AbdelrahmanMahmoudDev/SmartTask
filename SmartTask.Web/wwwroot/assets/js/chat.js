/*
Author       : Dreamstechnologies
Template Name: Smarthr - Bootstrap Admin Template
*/

(function () {
    "use strict";
	
	//Top Online Contacts
	if($('.chat-close').length > 0 ){
	// layout content remove
	$('.chat-close').on('click',function(){
		$(".chat").removeClass('show');
	});
	}

	$(".close_profile").on('click', function () {
		$('.right-side-contact').addClass('hide-right-sidebar');
		$('.right-side-contact').removeClass('show-right-sidebar');
		if ( $(window).width() > 991 && $(window).width() < 1201) {
			$(".chat").css('margin-left', 0);
		}
		if ($(window).width() < 992) {
			$('.chat').removeClass('hide-chatbar');
		}
	});

	if($('.emoj-action').length > 0) {
		$(".emoj-action").on('click', function () {
			$('.emoj-group-list').toggle();
		});
	}
	
	if($('.emoj-action-foot').length > 0) {
		$(".emoj-action-foot").on('click', function () {
			$('.emoj-group-list-foot').toggle();
		});
	}

	//Chat Resize

	$(".close_profile").on("click", function () {
		$('.right-user-side').removeClass('open-message');
		$('.chat-center-blk .card-comman').addClass('chat-center-space');
	});
	$(".profile-open").on("click", function () {
		$('.right-user-side').removeClass('add-setting');
		$('.chat-center-blk .card-comman').removeClass('chat-center-space');
	});

	//Call Resize
	$(".close_profile").on("click", function () {
		$('.right-user-side').removeClass('open-message');
		$('.video-screen-inner').removeClass('video-space');
		$('.right-side-party').removeClass('open-message');
		$('.meeting-list').removeClass('add-meeting');
		$('#chat-room').removeClass('open-chats');
		$('.main-img').removeClass('main-img-hide');
		$('.join-video').removeClass('main-img-hide');
		$('.call-user-side').addClass('add-setting');
	});

	$("#show-message").on("click", function () {
		$('#chat-room').addClass('open-chats');
		$('.right-side-party').removeClass('open-message');
		$('.main-img').addClass('main-img-hide');
		$('.join-video').addClass('main-img-hide');
	});
	
	//Chat Search Visible
	$('.chat-search-btn').on('click', function () {
		$('.chat-search').addClass('visible-chat');
	});
	$('.close-btn-chat').on('click', function () {
		$('.chat-search').removeClass('visible-chat');
	});
	$(".chat-search .form-control").on("keyup", function() {
		var value = $(this).val().toLowerCase();
		$(".chat .chat-body .messages .chats").filter(function() {
		$(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
		});
	});
	$(".guest-off").on('click', function () {
		$(this).toggleClass('activate');
		$('.chat-active-users').toggleClass('show-active-users');
	})
})();