// Create/Edit event form: click-to-pick coordinates on a small Leaflet map.
document.addEventListener('DOMContentLoaded', function () {
    var mapEl = document.getElementById('pickerMap');
    if (!mapEl || typeof L === 'undefined') return;

    var map = L.map('pickerMap').setView([initialLat, initialLng], 12);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    var latField = document.getElementById('latField');
    var lngField = document.getElementById('lngField');
    var marker = hasInitialMarker ? L.marker([initialLat, initialLng]).addTo(map) : null;

    map.on('click', function (e) {
        var lat = e.latlng.lat;
        var lng = e.latlng.lng;
        latField.value = lat;
        lngField.value = lng;

        if (marker) {
            marker.setLatLng(e.latlng);
        } else {
            marker = L.marker(e.latlng).addTo(map);
        }
    });
});
