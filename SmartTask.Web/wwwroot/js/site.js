////// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
////// for details on configuring this project to bundle and minify static web assets.

////// Write your JavaScript code.


// Initialize Kanban Sortable Columns
function initializeSortable() {
    $(".kanban-tasks").sortable({
        connectWith: ".kanban-tasks",
        placeholder: "ui-state-highlight",
        items: ".kanban-task:not(.disabled)",
        helper: "clone",
        cursor: "move",
        opacity: 0.7,
        tolerance: "pointer",
        revert: 0,
        delay: 0,
        forcePlaceholderSize: true,

        start: function (e, ui) {
            const currentColumn = $(this).closest(".kanban-column");
            const currentStatus = currentColumn.data("status");
            if (currentStatus === "Done") {
                $(this).sortable("cancel");
                return false;
            }

            ui.item.addClass("dragging");
            ui.item.data("prev-parent", currentColumn);
        },

        stop: function (e, ui) {
            ui.item.removeClass("dragging");

            const taskId = ui.item.data("id");
            const newColumn = ui.item.closest(".kanban-column");
            const newStatus = newColumn.data("status");
            const prevParent = ui.item.data("prev-parent");

            if (!prevParent.is(newColumn)) {
                $.ajax({
                    url: "/Task/UpdateStatus",
                    type: "POST",
                    data: { id: taskId, status: newStatus },
                    success: function () {
                        if (newStatus === "Done") {
                            ui.item.addClass("disabled");
                        } else {
                            ui.item.removeClass("disabled");
                        }
                        ui.item.find(".status-badge").text(newStatus);

                        filterTasks();
                    },
                    error: function (xhr) {
                        console.error("Error:", xhr.responseText);
                        prevParent.append(ui.item.detach());
                        ui.item.find(".status-badge").text(prevParent.data("status"));
                    }
                });
            }
        },

        receive: function (e, ui) {
            const allowedTransitions = {
                "Todo": ["InProgress"],
                "InProgress": ["Done"],
                "Done": []
            };

            const fromStatus = ui.sender.closest(".kanban-column").data("status");
            const toStatus = $(this).closest(".kanban-column").data("status");

            if (!allowedTransitions[fromStatus]?.includes(toStatus)) {
                $(ui.sender).sortable("cancel");
                ui.item.detach().appendTo(ui.sender);
            }
        }
    }).disableSelection();
}

// Open Edit Task Modal
function openEditTaskModal(taskId) {
    $.ajax({
        url: `/Task/GetTask/${taskId}`,
        type: "GET",
        success: function (task) {
            $("#taskId").val(task.id);
            $("#taskTitle").val(task.title);
            $("#taskPriority").val(task.priority);
            $("#taskStatus").val(task.status);
            $("#editTaskModal").modal("show");
        },
        error: function (xhr) {
            console.error("Error fetching task data:", xhr.responseText);
        }
    });
}

// Submit Edit Task Form
$("#editTaskForm").on("submit", function (e) {
    e.preventDefault();

    const taskId = $("#taskId").val();
    const newStatus = $("#taskStatus").val();
    const taskCard = $(`.kanban-task[data-id="${taskId}"]`);
    const currentStatus = taskCard.find(".status-badge").text();

    if (currentStatus === "Todo" && newStatus === "Done") {
        alert("You can't move directly to Done. Move to In Progress first.");
        return;
    }

    $.ajax({
        url: "/Task/UpdateTask",
        type: "POST",
        data: { id: taskId, status: newStatus },
        success: function () {
            $("#editTaskModal").modal("hide");

            taskCard.find(".status-badge").text(newStatus);
            taskCard.toggleClass("disabled", newStatus === "Done");

            const newColumn = $(`#column-${newStatus}`);
            taskCard.detach().appendTo(newColumn);

            filterTasks();
        },
        error: function (xhr) {
            console.error("Error updating task:", xhr.responseText);
        }
    });
});

function filterTasks() {
    const selectedPriority = $("#priorityFilter").val();
    const selectedStatus = $("#statusFilter").val();

    $(".kanban-task").each(function () {
        const taskPriority = $(this).data("priority");
        const taskStatus = $(this).closest(".kanban-column").data("status");

        const priorityMatch = selectedPriority === "All" || taskPriority === selectedPriority;
        const statusMatch = selectedStatus === "All" || taskStatus === selectedStatus;

        $(this).toggle(priorityMatch && statusMatch);
    });
}

 //Initialize column sorting
function initializeColumnSortable() {
    $("#kanban-board").sortable({
        items: ".col-md-4",
        handle: ".kanban-header",
        placeholder: "ui-state-highlight-column",
        cursor: "move",
        opacity: 0.7,
        tolerance: "pointer",

        update: function (event, ui) {
            const newOrder = [];
            $(".kanban-column").each(function (index) {
                newOrder.push({
                    ColumnId: $(this).data("column-id"),
                    Order: index
                });
            });

            console.log("New Order Data:", newOrder);
            $.ajax({
                url: "/Task/UpdateColumnOrder",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(newOrder),
                success: () => { console.log("Order updated"); },
                error: (xhr) => console.error("Error updating order:", xhr.responseText)
            });
        }
    });
}


// Handle column name editing
$(document).on("click", ".edit-column-name", function () {
    const status = $(this).data("status");
    const currentName = $(this).siblings(".display-name").text();
    const newName = prompt("Enter new column name:", currentName);

    if (newName && newName !== currentName) {
        $.ajax({
            url: "/Task/UpdateColumnDisplayName",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                status: status,
                displayName: newName
            }),
            success: () => {
                $(`.kanban-column[data-status="${status}"] .display-name`)
                    .text(newName);
            },
            error: (xhr) => {
                const error = xhr.responseJSON || { title: 'Unknown Error', detail: xhr.statusText };
                console.error("Full Error:", xhr);
                alert(`Error ${xhr.status}: ${error.title}\n${error.detail || error.message}`);

                // Optional: Reset positions visually without reload
                $('#kanban-board').sortable('cancel');
            }
        });
    }
});
// Update document ready
$(document).ready(function () {
    // Initialize column sorting first
    initializeColumnSortable();

    // Then initialize task sorting
    initializeSortable();

    // Initialize filters
    $("#priorityFilter, #statusFilter").on("change", filterTasks);
});