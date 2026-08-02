// Event details page: renders a read-only Leaflet marker and wires up the "Join" AJAX button.
document.addEventListener('DOMContentLoaded', function () {
    if (typeof L !== 'undefined' && document.getElementById('detailsMap')) {
        var map = L.map('detailsMap').setView([eventLat, eventLng], 14);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);
        L.marker([eventLat, eventLng]).addTo(map).bindPopup(eventTitle);
    }

    var joinBtn = document.getElementById('joinBtn');
    if (joinBtn) {
        joinBtn.addEventListener('click', function () {
            joinBtn.disabled = true;
            var eventId = joinBtn.getAttribute('data-event-id');
            EventFinder.postJson('/Events/' + eventId + '/Join')
                .then(function (res) {
                    return res.json().then(function (body) { return { ok: res.ok, body: body }; });
                })
                .then(function (result) {
                    if (result.ok) {
                        EventFinder.showAlert('#alertPlaceholder', result.body.message, 'success');
                        setTimeout(function () { window.location.reload(); }, 800);
                    } else {
                        EventFinder.showAlert('#alertPlaceholder', result.body.message || 'Xəta baş verdi.', 'danger');
                        joinBtn.disabled = false;
                    }
                })
                .catch(function () {
                    EventFinder.showAlert('#alertPlaceholder', 'Şəbəkə xətası baş verdi.', 'danger');
                    joinBtn.disabled = false;
                });
        });
    }
});
