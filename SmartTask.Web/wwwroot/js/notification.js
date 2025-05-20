function initializeNotifications() {
    toastr.options = {
        closeButton: true,
        progressBar: true,
        positionClass: "toast-top-right",
        timeOut: 5000,
        extendedTimeOut: 2000,
        showEasing: "swing",
        hideEasing: "linear",
        showMethod: "fadeIn",
        hideMethod: "fadeOut"
    };
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/NotificationHub")
        .build();

    // Start connection
    connection.start().then(function () {
        console.log("SignalR Connected");
        // Load initial notifications when connected
        loadNotifications();
    }).catch(function (err) {
        return console.error(err.toString());
    });

    // Listen for broadcasted notifications   
    connection.on("newnotifcation", function (notification) {
        console.log('new notification');
        toastr.info(notification.message, "Success");
        addNotificationToPopup(notification);
        updateNotificationCount();
    });

    connection.on("assignedtask", function (notification) {
        console.log('new assign task');
        if (notification.type == "NewTask") {
            toastr.success(notification.message, "Success");
        }
        else if (notification.type == "UpdateTask") {
            toastr.warning(notification.message, "Warning");
        }
        else if (notification.type == "Delete") {
            toastr.error(notification.message, "Delete");
        }
        else {
            toastr.info(notification.message, "Information");
        }
        addNotificationToPopup(notification);
        updateNotificationCount();
    });

    // Listen for group messages
    connection.on("grouptask", function (notification) {
        toastr.info(notification.message, "Success");
        addNotificationToPopup(notification);
        updateNotificationCount();
    });
}

function loadNotifications() {
    $.ajax({
        url: '/Notification/GetAllNotifications',  // MVC controller action
        type: 'GET',
        success: function (response) {
            //console.log(response);
            if (response.success) {
                const notificationContent = document.querySelector('.noti-content .d-flex');
                notificationContent.innerHTML = ''; // Clear existing notifications
                console.log('before');
                response.notifications.forEach(notification => {
                    addNotificationToPopup(notification);
                });

                updateNotificationCount(response.notifications.length);

                // Update unread status
                if (response.hasUnread) {
                    $('.notification-status-dot').show();
                } else {
                    $('.notification-status-dot').hide();
                }
            }
        },
        error: function (xhr, status, error) {
            console.log('errrrrorrrrr');
            console.error('Error loading notifications:', error);
            toastr.error('Failed to load notifications');
        }
    });
}

function addNotificationToPopup(notification) {
    const notificationContent = document.querySelector('.noti-content .d-flex');
    console.log('Notification type:', notification.type);

    // Determine notification icon and color based on type
    let iconClass = 'ti-bell';
    let iconColor = 'text-primary';
    let bgColor = 'bg-light-primary';

    switch (notification.type?.toLowerCase()) {
        case 'newtask':
            iconClass = 'ti-clipboard';
            iconColor = 'text-success';
            bgColor = 'bg-light-success';
            break;
        case 'updatetask':
            iconClass = 'ti-pencil';
            iconColor = 'text-warning';
            bgColor = 'bg-light-warning';
            break;
        case 'delete':
            iconClass = 'ti-trash';
            iconColor = 'text-danger';
            bgColor = 'bg-light-danger';
            break;
        case 'reminder':
            iconClass = 'ti-alarm';
            iconColor = 'text-info';
            bgColor = 'bg-light-info';
            break;
        case 'mention':
            iconClass = 'ti-at';
            iconColor = 'text-purple';
            bgColor = 'bg-light-purple';
            break;
        default:
            iconClass = 'ti-bell';
            iconColor = 'text-primary';
            bgColor = 'bg-light-primary';
    }

    const notificationHtml = `
        <div class="border-bottom mb-3 pb-3 notification-item ${notification.isRead ? 'read' : 'unread'}" 
             data-id="${notification.id}">
            <a href="${notification.link || '#'}" class="notification-link">
                <div class="d-flex align-items-center">
                    <span class="avatar avatar-lg me-2 flex-shrink-0 ${bgColor}">
                        <i class="${iconClass} ${iconColor} notification-icon"></i>
                    </span>
                    <div class="flex-grow-1">
                        <div class="notification-header d-flex justify-content-between align-items-center">
                            <p class="mb-1 notification-message ${notification.isRead ? '' : 'fw-bold'}">${notification.message}</p>
                            ${!notification.isRead ? '<span class="unread-indicator"></span>' : ''}
                        </div>
                        <span class="text-muted small">${formatTimestamp(notification.timestamp || new Date())}</span>
                    </div>
                </div>
            </a>
        </div>
    `;

    // Add at the beginning of the list
    notificationContent.insertAdjacentHTML('afterbegin', notificationHtml);
}

function updateNotificationCount(count) {
    // Update the notification count in the header
    const notificationBadge = document.querySelector('.header-badge');
    if (notificationBadge) {
        if (count === undefined) {
            // If count is not provided, increment the current count
            const currentCount = parseInt(notificationBadge.textContent) || 0;
            notificationBadge.textContent = currentCount + 1;
        } else {
            // If count is provided, set it directly
            notificationBadge.textContent = count;
        }
    }
}

function formatTimestamp(timestamp) {
    const date = new Date(timestamp);
    const now = new Date();
    const diffInSeconds = Math.floor((now - date) / 1000);

    if (diffInSeconds < 60) {
        return 'Just now';
    } else if (diffInSeconds < 3600) {
        const minutes = Math.floor(diffInSeconds / 60);
        return `${minutes} minute${minutes > 1 ? 's' : ''} ago`;
    } else if (diffInSeconds < 86400) {
        const hours = Math.floor(diffInSeconds / 3600);
        return `${hours} hour${hours > 1 ? 's' : ''} ago`;
    } else {
        const days = Math.floor(diffInSeconds / 86400);
        return `${days} day${days > 1 ? 's' : ''} ago`;
    }
}

// Handle mark all as read
$(document).on('click', '#mark-all-read', function (e) {
    e.preventDefault();
    e.stopPropagation();

    const token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/Notification/MarkAllAsRead',
        type: 'POST',
        headers: {
            'RequestVerificationToken': token
        },
        beforeSend: function () {
            console.log('Sending mark all as read request...');
        },
        success: function (response) {
            console.log('Mark all as read response:', response);
            if (response.success) {
                // Update UI to show all notifications as read
                $('.notification-status-dot').hide();
                $('.unread-indicator').removeClass('unread-indicator');
                $('.unread').removeClass('unread').addClass('read');
                toastr.success('All notifications marked as read');

                // Update the notification count
                updateNotificationCount(0);
            } else {
                console.log('Error in mark all:', response);
                toastr.error(response.message || 'Failed to mark notifications as read');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error marking notifications as read:', error);
            console.error('Status:', status);
            console.error('Response:', xhr.responseText);
            toastr.error('Failed to mark notifications as read');
        }
    });
});

// Handle individual notification click
$(document).on('click', '.notification-link', function (e) {
    e.preventDefault();
    const notificationItem = $(this).closest('.notification-item');
    const notificationId = notificationItem.data('id');
    const notificationLink = $(this).attr('href');

    // Don't process if it's a delete notification or invalid link
    if (notificationLink === '#') {
        return;
    }

    // Mark individual notification as read
    $.ajax({
        url: '/Notification/MarkAsRead',
        type: 'POST',
        data: { notificationId: notificationId },
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response.success) {
                // Update UI for this notification
                notificationItem.removeClass('unread').addClass('read');
                notificationItem.find('.unread-indicator').remove();
                notificationItem.find('.notification-message').removeClass('fw-bold');

                // Update count
                const currentCount = parseInt($('.header-badge').text()) || 0;
                if (currentCount > 0) {
                    updateNotificationCount(currentCount - 1);
                }

                // If there are no more unread notifications, hide the dot
                if (currentCount - 1 === 0) {
                    $('.notification-status-dot').hide();
                }

                // Navigate to the notification target page
                window.location.href = notificationLink;
            }
        },
        error: function (xhr, status, error) {
            console.error('Error marking notification as read:', error);
            // Still navigate even if marking as read fails
            window.location.href = notificationLink;
        }
    });
});

// SignalR connection
//let connection;

// Initialize notifications
//function initializeNotifications(userId) {
//    // Configure toastr
//    toastr.options = {
//        closeButton: true,
//        progressBar: true,
//        positionClass: "toast-top-right",
//        timeOut: 5000,
//        extendedTimeOut: 2000,
//        showEasing: "swing",
//        hideEasing: "linear",
//        showMethod: "fadeIn",
//        hideMethod: "fadeOut"
//    };

//    // Start connection
//    connection = new signalR.HubConnectionBuilder()
//        .withUrl("/notificationHub")
//        .withAutomaticReconnect()
//        .build();

//    // Handle notifications
//    connection.on("newAssignedTask", function (notification) {
//        // Display notification in toastr
//        switch (notification.type.toLowerCase()) {
//            case "success":
//                toastr.success(notification.message, "Success");
//                console.log("succes toaster!!!!!!!");
//                break;
//            case "warning":
//                toastr.warning(notification.message, "Warning");

//                break;
//            case "error":
//                toastr.error(notification.message, "Error");
//                break;
//            default:
//                toastr.info(notification.message, "Information");
//                break;
//        }

//        // Add to history
//        addToNotificationHistory(notification);

//        console.log("Notification received:", notification);
//    });

//    // Start connection
//    connection.start()
//        .then(function () {
//            console.log("SignalR Connected!");

//            // Register the user ID with the hub
//            if (userId) {
//                connection.invoke("RegisterUserId", userId).catch(function (err) {
//                    console.error("Error registering user ID:", err.toString());
//                });
//                console.log("Connected with user ID:", userId);
//            }
//        })
//        .catch(function (err) {
//            console.error("SignalR Connection Error: " + err.toString());
//            setTimeout(function () {
//                initializeNotifications(userId);
//            }, 5000); // Retry after 5 seconds
//        });

//    // Handle connection closed
//    connection.onclose(function () {
//        console.log("SignalR Disconnected!");

//        // Try to reconnect after 5 seconds
//        setTimeout(function () {
//            initializeNotifications(userId);
//        }, 5000);
//    });
//}

//// Add notification to history
//function addToNotificationHistory(notification) {
//    const history = document.getElementById("notification-history");

//    // Remove "no notifications" message if it exists
//    const noNotifications = history.querySelector(".text-muted");
//    if (noNotifications) {
//        history.innerHTML = "";
//    }

//    // Create notification element
//    const notificationElement = document.createElement("div");
//    notificationElement.className = `notification ${notification.type.toLowerCase()}`;

//    // Format timestamp
//    const timestamp = new Date(notification.timestamp);
//    const formattedTime = timestamp.toLocaleTimeString();

//    // Build content
//    let content = `
//        <p class="mb-1"><strong>${formattedTime}</strong></p>
//        <p class="mb-1">${notification.message}</p>
//    `;

//    // Add recipient information if it's a targeted notification
//    if (notification.userId) {
//        content += `<p class="mb-0 small">To: ${notification.userId}</p>`;
//    } else {
//        content += `<p class="mb-0 small">Broadcast</p>`;
//    }

//    notificationElement.innerHTML = content;

//    // Add to history
//    history.prepend(notificationElement);
//}

//// Send notification programmatically (can be used from other scripts)
//function sendNotification(message, type = "info", userId = null) {
//    if (connection && connection.state === signalR.HubConnectionState.Connected) {
//        connection.invoke("SendNotification", {
//            message: message,
//            type: type,
//            userId: userId,
//            timestamp: new Date()
//        }).catch(function (err) {
//            console.error(err.toString());
//            toastr.error("Failed to send notification");
//        });
//    } else {
//        console.error("SignalR not connected");
//        toastr.error("SignalR not connected");
//    }
//}

// Add this CSS to your notification.css file
const style = document.createElement('style');
style.textContent = `
    .notification-item {
        transition: background-color 0.3s ease;
    }
    
    .notification-item.unread {
        background-color: #f8f9fa;
    }
    
    .notification-item:hover {
        background-color: #f1f3f4;
    }
    
    .notification-icon {
        font-size: 1.2rem;
    }
    
    .unread-indicator {
        width: 8px;
        height: 8px;
        background-color: #0d6efd;
        border-radius: 50%;
        display: inline-block;
        margin-left: 8px;
    }
    
    .notification-message {
        color: #495057;
        margin-bottom: 0.25rem;
    }
    
    .notification-link {
        text-decoration: none;
        color: inherit;
    }
    
    .bg-light-primary {
        background-color: rgba(13, 110, 253, 0.1);
    }
    
    .bg-light-success {
        background-color: rgba(25, 135, 84, 0.1);
    }
    
    .bg-light-warning {
        background-color: rgba(255, 193, 7, 0.1);
    }
    
    .bg-light-danger {
        background-color: rgba(220, 53, 69, 0.1);
    }
    
    .bg-light-info {
        background-color: rgba(13, 202, 240, 0.1);
    }
    
    .bg-light-purple {
        background-color: rgba(111, 66, 193, 0.1);
    }
    
    .avatar {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 40px;
        height: 40px;
        border-radius: 50%;
    }
`;
document.head.appendChild(style);