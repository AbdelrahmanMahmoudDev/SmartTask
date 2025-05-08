
if($('#calendar').length > 0) {
    document.addEventListener('DOMContentLoaded', function() {
        var Draggable = FullCalendar.Draggable;

        // Initialize external events (the draggable ones)
        var containerEl = document.getElementById('external-events');
        new Draggable(containerEl, {
            itemSelector: '.fc-event',

            eventData: function (eventEl) {
                var className = eventEl.getAttribute('data-event-classname'); // Get the class name
                return {
                    title: eventEl.innerText.trim(),
                    classNames: [className], // Pass dynamic class name
                };
            }
        });
        
        var calendarEl = document.getElementById('calendar');		
        var calendar = new FullCalendar.Calendar(calendarEl, {
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },
          initialView: 'dayGridMonth', 
          events: [
            {
              title: 'Meeting with Team Dev',
              className: 'badge badge-pink-transparent',
              backgroundColor: '#FFEDF6',
              textColor: "#FD3995",		
              start: new Date($.now() - 168000000).toJSON().slice(0, 10),
              end: new Date($.now() - 168000000).toJSON().slice(0, 10),
            },
            {
                title: 'UI/UX Team...',	
                className: 'badge badge-secondary-transparent',
                backgroundColor: '#EDF2F4' ,
                textColor: "#0C4B5E",				  
                start: new Date($.now() + 338000000).toJSON().slice(0, 10)
            },
            {
                title: 'Data Update...',
                className: 'badge badge-purple-transparent',
                backgroundColor: '#F7EEF9',		
                textColor: "#AB47BC",		  
                start: new Date($.now() - 338000000).toJSON().slice(0, 10) 
            },
            {
                title: 'Meeting with Team Dev',
                className: 'badge badge-dark-transparent',
                backgroundColor: '#E8E9EA',		
                textColor: "#212529",				  
                start: new Date($.now() + 68000000).toJSON().slice(0, 10) 
            },
            {
                title: 'Design System',
                className: 'badge badge-danger-transparent',
                backgroundColor: '#FAE7E7',	
                textColor: "#E70D0D",				  
                start: new Date($.now() + 88000000).toJSON().slice(0, 10) 
            },
          ],
          headerToolbar: {
            start: 'today prev,next',
            end: 'dayGridMonth,dayGridWeek,dayGridDay',
            center: 'title'
          }, 
          eventClick: function(info) {
            // Open modal
            $('#event_modal').modal('show');
            
            // Populate modal with event details
            document.getElementById('eventTitle').innerText = info.event.title;
          },
          editable: true,
            droppable: true, // Enable drag and drop
            drop: function (info) {
                // If the event is dropped, do something here (optional)
                console.log('Event dropped');
            },
            eventReceive: function(info) {
                // When event is dropped on calendar
                console.log('Event added', info.event.title);
            }
        });		
        calendar.render();
    });			
}



if($('#calendar1').length > 0) {

    document.addEventListener('DOMContentLoaded', function() {
        var todayDate = moment().startOf('day');
        var YM = todayDate.format('YYYY-MM');
        var YESTERDAY = todayDate.clone().subtract(1, 'day').format('YYYY-MM-DD');
        var TODAY = todayDate.format('YYYY-MM-DD');
        var TOMORROW = todayDate.clone().add(1, 'day').format('YYYY-MM-DD');

        var calendarEl = document.getElementById('calendar');
        var calendar = new FullCalendar.Calendar(calendarEl, {
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay,listMonth'
            },

            height: 500,
            contentHeight: 580,
            aspectRatio: 3,  // see: https://fullcalendar.io/docs/aspectRatio


            views: {
                dayGridMonth: { buttonText: 'month' },
                timeGridWeek: { buttonText: 'week' },
                timeGridDay: { buttonText: 'day' }
            },

            initialView: 'dayGridMonth',
            initialDate: TODAY,

            editable: true,
            dayMaxEvents: true, // allow "more" link when too many events
            navLinks: true,
            events: [
                {
                    title: 'All Day Event',
                    start: new Date($.now() - 168000000).toJSON().slice(0, 10),
                    backgroundColor: '#FDE9ED'
                },
                {
                    id: 1000,
                    title: 'Repeating Event',
                    start: new Date($.now() - 338000000).toJSON().slice(0, 10) 
                },
                {
                    title: 'Meeting',
                    start: new Date($.now() - 338000000).toJSON().slice(0, 10)
                },
                {
                    title: 'Click for Google',
                    start: new Date($.now() + 68000000).toJSON().slice(0, 10),
                    className: "bg-secondary text-white" 
                }
            ]
        });

        calendar.render();
    });
}
