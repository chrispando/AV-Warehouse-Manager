 // Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getProductFromSKU(sku) {
    $.ajax({
        url: 'GetProductFromSKU',
        type: 'GET',
        dataType: "json",
        data: { sku: sku },
        success: function (data) {
            document.getElementById("newMake").value = data.make;//make
            document.getElementById("newModel").value = data.model;//model
            document.getElementById("newQuantity").value = data.quantity;//quantity
            document.getElementById("newDescription").value = data.description;//description
            document.getElementById("newLocation").value = data.location;//location

        }
    });
}

function getProductFromSerial(serial) {
    $.ajax({
        url: 'GetProductFromSerial',
        type: 'GET',
        dataType: "json",
        data: { serial: serial },
        success: function (data) {
            document.getElementById("newSkuNumber").value = data.sku;//sku
            document.getElementById("newMake").value = data.make;//model
            document.getElementById("newModel").value = data.model;//model
            document.getElementById("newQuantity").value = data.quantity;//quantity
            document.getElementById("newDescription").value = data.description;//description
            document.getElementById("newLocation").value = data.location;//location

        }
    });
}
//----------------------------------------------------------------------------------------------//
var calendarEl = document.getElementById('calendar');
var calendar = new FullCalendar.Calendar(calendarEl, {
    initialView: 'dayGridMonth',
    timeZone: 'CST',
    eventClick: function (info) {
        alert('Event: ' + info.event.title);
        
        //alert('Coordinates: ' + info.jsEvent.pageX + ',' + info.jsEvent.pageY);
        //alert('View: ' + info.view.type);

        // change the border color just for fun
        info.el.style.borderColor = 'red';
    }
});


function populateCalendar2(data) {
    var json_data = JSON.parse(data);
    var arr = [];
    for (var i = 0; i < json_data.length; i++) {
        arr.push(json_data[i]);
    }
    for (var i = 0; i < json_data.length; i++) {
        calendar.addEvent(arr[i]);

    }
    
    calendar.render();
}

document.getElementById("body").onload=function populateCalendar() {
    //start of ajax


    $.ajax({
        url: 'Calendar/GetData',
        type: "GET",
        contentType: "application/json",
        success: function (data) {

            populateCalendar2(data);

        }

    });
    //end of ajax
    

}


//document.addEventListener('DOMContentLoaded', function () {


//    populateCalendar();


//    calendar.render();
//});


//----------------------------------------------------------------------------------------//