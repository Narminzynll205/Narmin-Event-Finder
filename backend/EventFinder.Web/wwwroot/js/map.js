// Nearby-users map: shares the browser's geolocation, then plots markers for
// the current user and everyone else within the chosen radius.
document.addEventListener('DOMContentLoaded', function () {
    var statusEl = document.getElementById('mapStatus');
    var map = L.map('nearbyMap').setView([40.3777, 49.8920], 12);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    var meMarker = null;
    var otherMarkers = [];
    var currentPosition = null;

    function clearOtherMarkers() {
        otherMarkers.forEach(function (m) { map.removeLayer(m); });
        otherMarkers = [];
    }

    function loadNearby() {
        if (!currentPosition) return;
        var radiusKm = document.getElementById('radiusInput').value || 10;
        var url = '/Map/Nearby?lat=' + currentPosition.lat + '&lng=' + currentPosition.lng + '&radiusKm=' + radiusKm;

        fetch(url)
            .then(function (res) { return res.json(); })
            .then(function (users) {
                clearOtherMarkers();
                statusEl.textContent = users.length + ' istifadəçi tapıldı (radius: ' + radiusKm + ' km).';
                renderUserMarkers(users);
            })
            .catch(function () {
                statusEl.textContent = 'İstifadəçilər yüklənərkən xəta baş verdi.';
            });
    }

    function renderUserMarkers(users) {
        users.forEach(function (u) {
            if (u.latitude === undefined || u.longitude === undefined) return;
            var marker = L.marker([u.latitude, u.longitude]).addTo(map);
            marker.bindPopup(
                '<strong>' + u.fullName + '</strong><br/>' +
                (u.distanceKm ? u.distanceKm.toFixed(1) + ' km' : '') +
                '<br/><a href="/Chat/User/' + u.userId + '" class="btn btn-sm btn-brand mt-1">Chat başlat</a>'
            );
            otherMarkers.push(marker);
        });
    }

    function updateLocationAndRefresh(position) {
        currentPosition = { lat: position.coords.latitude, lng: position.coords.longitude };

        if (meMarker) {
            meMarker.setLatLng(currentPosition);
        } else {
            meMarker = L.marker(currentPosition, { title: 'Siz' }).addTo(map);
            meMarker.bindPopup('<strong>Siz</strong>');
        }
        map.setView(currentPosition, 13);

        EventFinder.postJson('/Map/Location', { latitude: currentPosition.lat, longitude: currentPosition.lng })
            .finally(loadNearby);
    }

    if (navigator.geolocation) {
        statusEl.textContent = 'Mövqe müəyyən edilir...';
        navigator.geolocation.getCurrentPosition(updateLocationAndRefresh, function () {
            statusEl.textContent = 'Mövqe alına bilmədi. Brauzer icazəsini yoxlayın.';
        });
    } else {
        statusEl.textContent = 'Brauzeriniz geolocation dəstəkləmir.';
    }

    document.getElementById('refreshBtn').addEventListener('click', function () {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(updateLocationAndRefresh);
        }
    });
});
