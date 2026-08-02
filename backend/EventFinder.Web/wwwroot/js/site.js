// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Shared helpers used by events.js, map.js and chat.js for AJAX calls against
// the server's own MVC endpoints (cookie-authenticated, anti-forgery protected).
window.EventFinder = window.EventFinder || {};

EventFinder.getAntiForgeryToken = function () {
    var input = document.querySelector('#af-token-form input[name="__RequestVerificationToken"]');
    return input ? input.value : null;
};

EventFinder.postJson = function (url, data) {
    var headers = { 'Content-Type': 'application/json' };
    var token = EventFinder.getAntiForgeryToken();
    if (token) {
        headers['RequestVerificationToken'] = token;
    }
    return fetch(url, {
        method: 'POST',
        headers: headers,
        body: data !== undefined ? JSON.stringify(data) : null
    });
};

EventFinder.showAlert = function (container, message, type) {
    type = type || 'danger';
    var el = typeof container === 'string' ? document.querySelector(container) : container;
    if (!el) return;
    el.innerHTML = '<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">' +
        message + '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button></div>';
};
