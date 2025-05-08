(function () {
    "use strict"

    /* basic select2 */
    $('.select2').select2();

    /* multiple select */
    $('.js-example-basic-multiple').select2();
	
// Single Select Placeholder
$("#select2-with-placeholder").select2({
	placeholder: "Select a state",
	allowClear: true,
	dir: "ltr"
});

    /* single select with placeholder */
    $("#select2-placeholder-single").select2({
        placeholder: "Select a state",
        allowClear: true,
        dir: "ltr"
    });

    /* multiple select with placeholder */
    $(".js-example-placeholder-multiple").select2({
        placeholder: "Select"
    });

    /* templating */
    function formatState(state) {
        if (!state.id) {
            return state.text;
        }
        var baseUrl = "../assets/images/faces/select2";
        var $state = $(
            '<span><img src="' + baseUrl + '/' + state.element.value.toLowerCase() + '.jpg" class="img-flag" /> ' + state.text + '</span>'
        );
        return $state;
    };
    $(".js-example-templating").select2({
        templateResult: formatState,
        placeholder: "Choose Customer"
    });

    /* with images */
    function selectClient(client) {
        if (!client.id) { return client.text; }
        var $client = $(
            '<span><img src="../assets/images/faces/select2/' + client.element.value.toLowerCase() + '.jpg" /> '
            + client.text + '</span>'
        );
        return $client;
    };
    $(".select2-client-search").select2({
        templateResult: selectClient,
        templateSelection: selectClient,
        placeholder: "Choose Client",
        escapeMarkup: function (m) { return m; }
    });

    /* max selections limiting */
    $(".js-example-basic-multiple-limit-max").select2({
        maximumSelectionLength: 3,
        placeholder: "Choose Person",
    });

    /* Disablind select 2 controls */
    $(".js-example-disabled").select2();
    $(".js-example-disabled-multi").select2();

    $(".js-programmatic-enable").on("click", function () {
        $(".js-example-disabled").prop("disabled", false);
        $(".js-example-disabled-multi").prop("disabled", false);
    });
    $(".js-programmatic-disable").on("click", function () {
        $(".js-example-disabled").prop("disabled", true);
        $(".js-example-disabled-multi").prop("disabled", true);
    });

})();