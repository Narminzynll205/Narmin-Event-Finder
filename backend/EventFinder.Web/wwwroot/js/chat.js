// Chat page: connects to the SignalR ChatHub (cookie-authenticated, same-origin) and
// wires up history loading + sending for the active event/direct conversation.
document.addEventListener('DOMContentLoaded', function () {
    if (typeof chatTargetId === 'undefined') {
        return; // no active conversation selected
    }

    var messagesEl = document.getElementById('chatMessages');
    var input = document.getElementById('chatInput');
    var sendBtn = document.getElementById('chatSendBtn');

    function renderMessage(msg) {
        var isOwn = msg.senderId === chatCurrentUserId;
        var bubble = document.createElement('div');
        bubble.className = 'chat-bubble ' + (isOwn ? 'own' : 'other');
        var time = new Date(msg.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
        bubble.innerHTML = (isOwn ? '' : '<div class="small fw-bold">' + msg.senderName + '</div>') +
            '<div>' + escapeHtml(msg.content) + '</div>' +
            '<div class="small text-end" style="opacity:.7;">' + time + '</div>';
        messagesEl.appendChild(bubble);
    }

    function escapeHtml(text) {
        var div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function scrollToBottom() {
        messagesEl.scrollTop = messagesEl.scrollHeight;
    }

    function loadHistory() {
        fetch('/Chat/History?type=' + chatType + '&id=' + encodeURIComponent(chatTargetId))
            .then(function (res) { return res.json(); })
            .then(function (messages) {
                messagesEl.innerHTML = '';
                messages.forEach(renderMessage);
                scrollToBottom();
            });
    }

    loadHistory();

    var connection = new signalR.HubConnectionBuilder()
        .withUrl('/hubs/chat')
        .withAutomaticReconnect()
        .build();

    connection.on('ReceiveMessage', function (msg) {
        var belongsToThisThread = chatType === 'event'
            ? (msg.eventId && msg.eventId.toString() === chatTargetId)
            : (!msg.eventId && (msg.senderId === chatTargetId || msg.receiverId === chatTargetId));

        if (belongsToThisThread) {
            renderMessage(msg);
            scrollToBottom();
        }
    });

    connection.start()
        .then(function () {
            if (chatType === 'event') {
                connection.invoke('JoinEventGroup', parseInt(chatTargetId, 10));
            }
        })
        .catch(function (err) {
            console.error('SignalR connection failed', err);
        });

    function send() {
        var content = input.value.trim();
        if (!content) return;

        var dto = { content: content };
        if (chatType === 'event') {
            dto.eventId = parseInt(chatTargetId, 10);
        } else {
            dto.receiverId = chatTargetId;
        }

        connection.invoke('SendMessage', dto).then(function () {
            input.value = '';
        }).catch(function (err) {
            console.error('Send failed', err);
        });
    }

    sendBtn.addEventListener('click', send);
    input.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            send();
        }
    });
});
