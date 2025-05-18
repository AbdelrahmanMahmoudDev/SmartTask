// dashboard.js - Handles all dashboard-related interactions

$(document).ready(function () {
    // Initialize dashboard components
    initializeDashboard();

    // Setup widget controls
    setupWidgetControls();

    // Setup view switching
    setupViewSwitching();
});

/**
 * Initialize dashboard components
 */
function initializeDashboard() {
    // Show loading state during initial load
    $('.dashboard-widget').each(function () {
        const widget = $(this);
        widget.addClass('loading');

        // Remove loading state after components are loaded
        setTimeout(function () {
            widget.removeClass('loading');
        }, 500);
    });

    // Handle any TempData messages
    if ($('.alert-success').length) {
        setTimeout(function () {
            $('.alert-success').fadeOut();
        }, 5000);
    }
}

/**
 * Setup widget controls (toggle visibility, refresh)
 */
function setupWidgetControls() {
    // Toggle widget visibility
    $('.toggle-widget').click(function () {
        const widgetType = $(this).data('widget');
        const widget = $('#' + widgetType + '-widget');

        // Toggle visibility
        widget.toggle();

        // Update button text
        const isVisible = widget.is(':visible');
        $(this).text(isVisible ? 'Hide' : 'Show');

        // Send update to server
        updateWidgetVisibility(widgetType, isVisible);
    });

    // Refresh widget content
    $('.refresh-widget').click(function () {
        const widgetType = $(this).data('widget');
        const widget = $('#' + widgetType + '-widget');

        // Add loading indicator
        widget.addClass('loading').append('<div class="widget-loader"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>');

        // For a real implementation, you'd use AJAX to refresh just this component
        // For simplicity, we're reloading the page
        setTimeout(function () {
            location.reload();
        }, 800);
    });

    // Handle recent projects settings
    $('#saveRecentProjectsSettings').click(function () {
        const count = $('#projectCount').val();

        // Validate input
        if (count < 1 || count > 20) {
            alert('Please enter a number between 1 and 20');
            return;
        }

        // Send update to server (you'd need to implement this endpoint)
        $.ajax({
            url: '/Dashboard/UpdateRecentProjectsCount',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ count: parseInt(count) }),
            success: function (response) {
                if (response.success) {
                    $('#recentProjectsSettingsModal').modal('hide');
                    // Refresh the widget
                    $('#recent-projects-widget').addClass('loading');
                    location.reload();
                }
            }
        });
    });
}

/**
 * Setup view switching (grid/list)
 */
function setupViewSwitching() {
    $('#gridViewBtn, #listViewBtn').click(function () {
        const viewType = $(this).data('view');

        // Toggle active state on buttons
        $('#gridViewBtn, #listViewBtn').removeClass('active');
        $(this).addClass('active');

        // Toggle view class on container
        $('.dashboard-container')
            .removeClass('grid-view list-view')
            .addClass(viewType + '-view');

        // Send update to server
        updateViewPreference(viewType);
    });
}

/**
 * Update widget visibility on the server
 */
function updateWidgetVisibility(widgetType, isVisible) {
    $.ajax({
        url: '/Dashboard/UpdateLayout',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            widgetType: widgetType,
            isVisible: isVisible
        }),
        success: function (response) {
            if (response.success) {
                console.log('Widget visibility updated');
            } else {
                console.error('Failed to update widget visibility');
            }
        },
        error: function () {
            console.error('Error updating widget visibility');
        }
    });
}

/**
 * Update view preference on the server
 */
function updateViewPreference(viewType) {
    $.ajax({
        url: '/Dashboard/ChangeView',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ viewType: viewType }),
        success: function (response) {
            if (response.success) {
                console.log('View preference updated');
            } else {
                console.error('Failed to update view preference');
            }
        },
        error: function () {
            console.error('Error updating view preference');
        }
    });
}

// Add CSS for widget loading state
$('<style>')
    .prop('type', 'text/css')
    .html(`
        .dashboard-widget.loading {
            position: relative;
            min-height: 200px;
        }
        .widget-loader {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(255,255,255,0.7);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 10;
        }
    `)
    .appendTo('head');