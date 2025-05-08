(function($){

 "use strict"; // Start of use strict

$(function() {
    "use strict";
    // Just the defaults.
    $("span.pie").peity("pie",{
        width: '80',
        height: '80'
    })

    $('span.donut').peity('donut',{
        width: '50',
        height: '50'
    })


    $(".peity-line").peity("line",{
        width: '100%',
        height: '65',
		fill:['#506EE4']
    })

    $(".bar").peity("bar",{
        width: '100%',
        height: '50'
    })

    $(".bar-colours-1").peity("bar", {
        fill: ["#664dc9", "#e3dff5"],
        width: '100',
        height: '100'
    })

    $(".bar-colours-2").peity("bar", {
        fill: function(value) {
            return value > 0 ? "#38cb89" : "#aeeacf"
        },
        width: '100',
        height: '100'
    })

    $(".bar-colours-3").peity("bar", {
        fill: ["#ffab00", "#ffdd99"],
        width: '100',
        height: '100'
    })

    $(".bar-colours-4").peity("bar", {
        fill: function(value) {
            return value > 0 ? "#ef4b4b" : "#f7a1a1"
        },
        width: '100',
        height: '100'
    })

    $(".pie-colours-1").peity("pie", {
        fill: ["#705ec8", "#fa057a", "#2dce89", "#ff5b51"],
        width: '100',
        height: '100'
    })

    $(".pie-colours-2").peity("pie", {
		fill: ["#705ec8", "#fa057a", "#2dce89", "#ff5b51", "#fcbf09"],
        width: '100',
        height: '100'
    })

    // Using data attributes
    $(".data-attributes span").peity("donut")

    // Evented example.
    $("select").change(function() {
        var text = $(this).val() + "/" + 5
		
        $(this)
            .siblings("span.graph")
            .text(text)
            .change()

        $("#notice").text("Chart updated: " + text)
    }).change()

    $("span.graph").peity("pie")

    // Updating charts.
    var updatingChart = $(".updating-chart").peity("line", { width: "100%",height:65 })

    setInterval(function() {
        var random = Math.round(Math.random() * 20)
        var values = updatingChart.text().split(",")
        values.shift()
        values.push(random)

        updatingChart
            .text(values.join(","))
            .change()
    }, 2500)
})

$(".ticket-chart-1").peity("bar", {
    fill: ["#F26522"],
    width: '100%',
    height: '70'
})

$(".ticket-chart-2").peity("bar", {
    fill: ["#AB47BC"],
    width: '100%',
    height: '70'
})

$(".ticket-chart-3").peity("bar", {
    fill: ["#03C95A"],
    width: '100%',
    height: '70'
})

$(".ticket-chart-4").peity("bar", {
    fill: ["#0DCAF0"],
    width: '100%',
    height: '70'
})
$(".subscription-line-1").peity("line",{
    width: '100%',
    height: '35',
    fill:['#F7A37A'],
    stroke:['#F7A37A']
})
$(".subscription-line-2").peity("line",{
    width: '100%',
    height: '25',
    fill:['#70B1FF'],
    stroke:['#70B1FF']
})
$(".subscription-line-3").peity("line",{
    width: '100%',
    height: '25',
    fill:['#60DD97'],
    stroke:['#60DD97']
})
$(".subscription-line-4").peity("line",{
    width: '100%',
    height: '25',
    fill:['#DE5555'],
    stroke:['#DE5555']
})
$(".country-chart-1").peity("line",{
    width: '60%',
    height: '20',
    fill:['#D2F5E1'],
    stroke:['#1CCE6B']
})
$(".country-chart-2").peity("line",{
    width: '60%',
    height: '20',
    fill:['#F6CECE'],
    stroke:['#D00C0C']
})

$(".company-bar1").peity("bar", {
    fill: ["#FF6F28"],
    width: '52',
    height: '40'
})

$(".company-bar2").peity("bar", {
    fill: ["#4B3088"],
    width: '52',
    height: '40'
})

$(".company-bar3").peity("bar", {
    fill: ["#177DBC"],
    width: '52',
    height: '40'
})

$(".company-bar4").peity("bar", {
    fill: ["#2DCB73"],
    width: '52',
    height: '40'
})

})(jQuery);
