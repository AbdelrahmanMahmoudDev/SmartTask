

function initializeNotifications()
{
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
    }).catch(function (err) {
        return console.error(err.toString());
    });




    // Listen for broadcasted notifications   
    connection.on("newnotifcation", function (notification) {
        console.log('new notification');
        toastr.info(notification.message, "Success");

    });

    connection.on("assignedtask", function (notification) {
        console.log('new assign task');
        if (notification.type == "NewTask")
        {
            toastr.success(notification.message, "Success");
        } 
        else if (notification.type == "UpdateTask")
        {
            toastr.warning(notification.message, "Warning");
        }
        else  {
            toastr.info(notification.message, "Information");

        }

    });

    // Listen for group messages
    connection.on("grouptask", function (notification) {
        toastr.info(notification.message, "Success");


    });
}


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