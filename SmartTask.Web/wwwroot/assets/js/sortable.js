(function () {
    "use strict";

    /* simple sortable */
    var simple = document.getElementById('simple-list');
    new Sortable(simple, {
        animation: 150,
    });
    /* simple sortable */

    /* shared sortable */
    var sharedLeft = document.getElementById('shared-left');
    new Sortable(sharedLeft, {
        group: 'shared',
        animation: 150
    });
    var sharedRight = document.getElementById('shared-right');
    new Sortable(sharedRight, {
        group: 'shared',
        animation: 150
    });
    /* shared sortable */

    /* cloning */
    var cloningLeft = document.getElementById('cloning-left');
    new Sortable(cloningLeft, {
        group: {
            name: 'shared',
            pull: 'clone' // To clone: set pull to 'clone'
        },
        animation: 150
    });
    var cloningRight = document.getElementById('cloning-right');
    new Sortable(cloningRight, {
        group: {
            name: 'shared',
            pull: 'clone'
        },
        animation: 150
    });
    /* cloning */

    /* disabling sorting */
    var disablingSortingLeft = document.getElementById('disabling-sorting-left');
    new Sortable(disablingSortingLeft, {
        group: {
            name: 'shared',
            pull: 'clone',
            put: false // Do not allow items to be put into this list
        },
        animation: 150,
        sort: false // To disable sorting: set sort to false
    });
    var disablingSortingRight = document.getElementById('disabling-sorting-right');
    new Sortable(disablingSortingRight, {
        group: 'shared',
        animation: 150
    });
    /* disabling sorting */

    /* sorting with handle */
    var sortingWithHandle = document.getElementById('sorting-with-handle');
    new Sortable(sortingWithHandle, {
        handle: '.handle', // handle's class
        animation: 150
    });
    /* sorting with handle */

    /* sorting with filter */
    var sortingWithFilter = document.getElementById('sorting-with-filter');
    new Sortable(sortingWithFilter, {
        filter: '.filtered', // 'filtered' class is not draggable
        animation: 150
    });
    /* sorting with filter */

    /* sortable grid */
    var grid = document.getElementById('sortable-grid');
    new Sortable(grid, {
        animation: 150,
    });
    /* sortable grid */

    /* nested sortable */
    var nestedSortables = [].slice.call(document.querySelectorAll('.nested-sortable'));
    for (var i = 0; i < nestedSortables.length; i++) {
        new Sortable(nestedSortables[i], {
            group: 'nested',
            animation: 150,
            fallbackOnBody: true,
            swapThreshold: 0.65
        });
    }
    /* nested sortable */

    /* multi drag */
    var multiDragDemo = document.getElementById('multiple-drag');
    new Sortable(multiDragDemo, {
        multiDrag: true, // Enable multi-drag
        selectedClass: 'selected', // The class applied to the selected items
        fallbackTolerance: 3, // So that we can select items on mobile
        animation: 150
    });
    /* multi drag */

    /* swap */
    var swapDemo = document.getElementById('sortable-swap');
    new Sortable(swapDemo, {
        swap: true, // Enable swap plugin
        swapClass: 'highlight', // The class applied to the hovered swap item
        animation: 150
    });
    /* swap */

})();