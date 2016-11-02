var map;
$(document).ready(function () {

   

    var mapDiv = document.getElementById('map');
    map = new google.maps.Map(mapDiv, {
        center: { lat: 37.983932, lng: 23.727009 },
        zoom: 5
    });
    setAthens();
    setThesalloniki();   
});

function setAthens(){
    var athensLocation = new google.maps.LatLng(37.983932, 23.727009);
    var athens = new google.maps.Circle({
        center: athensLocation,
        radius: 20000,
        strokeColor: "#0000FF",
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillColor: "#0000FF",
        fillOpacity: 0.4
    });
    athens.setMap(map);
    google.maps.event.addListener(athens, 'click', function () {
        window.location = window.location.origin + "/Home/Index?city=Athens";
    });
}

function setThesalloniki() {
    var thesallonikiLocation = new google.maps.LatLng(40.639947, 22.944310);
    var thesalloniki = new google.maps.Circle({
        center: thesallonikiLocation,
        radius: 20000,
        strokeColor: "#0000FF",
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillColor: "#0000FF",
        fillOpacity: 0.4
    });
    thesalloniki.setMap(map);
    google.maps.event.addListener(thesalloniki, 'click', function () {
        window.location = window.location.origin + "/Home/Index?city=Thesalloniki";
    });
}

