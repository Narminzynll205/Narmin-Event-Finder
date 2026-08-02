// Events list page: "search nearby" fills lat/lng via the browser geolocation API.
document.addEventListener('DOMContentLoaded', function () {
    var btn = document.getElementById('useLocationBtn');
    if (!btn) return;

    btn.addEventListener('click', function () {
        var status = document.getElementById('locationStatus');
        if (!navigator.geolocation) {
            status.textContent = 'Brauzeriniz geolocation dəstəkləmir.';
            return;
        }

        status.textContent = 'Mövqe müəyyən edilir...';
        navigator.geolocation.getCurrentPosition(function (position) {
            document.getElementById('latInput').value = position.coords.latitude;
            document.getElementById('lngInput').value = position.coords.longitude;
            var radiusInput = document.getElementById('radiusKm');
            if (!radiusInput.value) {
                radiusInput.value = 5;
            }
            status.textContent = 'Mövqe tapıldı, "Filtrlə" düyməsinə basın.';
        }, function () {
            status.textContent = 'Mövqe alına bilmədi. İcazə verildiyini yoxlayın.';
        });
    });
});
