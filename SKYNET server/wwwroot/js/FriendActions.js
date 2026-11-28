// Shared friend-action UI for /users (cards) and /users/{id} (profile).
// Every container <div data-friend-actions data-steam-id data-rel data-variant data-field data-url> is drawn by
// render() from the viewer's relationship (0 none, 2 incoming, 3 friend, 4 outgoing). The same function paints
// the first render, the answer of every action (the server returns the NEW relationship) and the poll result,
// so the buttons never depend on guessing from a DOM attribute.
(function () {
    var STATE_URL = '/users?handler=State';
    var POLL_MS = 5000;

    var CLS = {
        card: {
            primary: 'w-10 h-10 rounded-lg bg-skynet-accent/10 text-skynet-accent border border-skynet-accent/20 flex items-center justify-center hover:bg-skynet-accent hover:text-white transition',
            danger: 'w-10 h-10 rounded-lg bg-red-500/10 text-red-500 border border-red-500/20 flex items-center justify-center hover:bg-red-500 hover:text-white transition',
            pending: 'w-10 h-10 rounded-lg bg-yellow-500/10 text-yellow-500 border border-yellow-500/20 flex items-center justify-center hover:bg-red-500 hover:text-white transition'
        },
        profile: {
            primary: 'rounded-lg bg-skynet-accent px-5 py-3 text-xs font-black uppercase tracking-widest text-white hover:bg-blue-600 transition-all',
            danger: 'rounded-lg border border-red-500/30 px-5 py-3 text-xs font-black uppercase tracking-widest text-red-300 hover:bg-red-500/10 transition-all',
            pending: 'rounded-lg border border-yellow-500/30 px-5 py-3 text-xs font-black uppercase tracking-widest text-yellow-500 hover:bg-red-500/10 transition-all'
        }
    };

    // Buttons per relationship. Cancel / Decline / Remove all post to the Remove handler (the server tells them apart).
    var PLAN = {
        0: [{ handler: 'Request', kind: 'primary', icon: 'fa-user-plus', title: 'Add friend', label: 'Add Friend' }],
        2: [{ handler: 'Accept', kind: 'primary', icon: 'fa-user-check', title: 'Accept request', label: 'Accept' },
            { handler: 'Remove', kind: 'danger', icon: 'fa-user-times', title: 'Decline request', label: 'Decline' }],
        3: [{ handler: 'Remove', kind: 'danger', icon: 'fa-user-minus', title: 'Remove friend', label: 'Remove' }],
        4: [{ handler: 'Remove', kind: 'pending', icon: 'fa-clock', title: 'Cancel request', label: 'Cancel Request' }]
    };

    var STATUS = { 0: 'Not connected', 2: 'Request received', 3: 'Friend', 4: 'Request sent' };

    var version = 0; // bumped on every submit/answer so a poll that started earlier cannot overwrite a newer state

    function pageToken() {
        var input = document.querySelector('input[name="__RequestVerificationToken"]');
        return input ? input.value : '';
    }

    function hidden(name, value) {
        var input = document.createElement('input');
        input.type = 'hidden';
        input.name = name;
        input.value = value;
        return input;
    }

    function buildForm(box, step) {
        var variant = box.getAttribute('data-variant') === 'profile' ? 'profile' : 'card';
        var form = document.createElement('form');
        form.method = 'post';
        form.setAttribute('action', box.getAttribute('data-url') + '?handler=' + step.handler);
        // CallHelper.js turns forms with OnComplete into fetch() calls; it also sends the antiforgery header.
        form.setAttribute('OnComplete', 'onFriendAction');
        form.appendChild(hidden(box.getAttribute('data-field'), box.getAttribute('data-steam-id')));
        var token = pageToken();
        if (token) form.appendChild(hidden('__RequestVerificationToken', token));
        var button = document.createElement('button');
        button.type = 'submit';
        button.className = CLS[variant][step.kind];
        button.title = step.title;
        var icon = document.createElement('i');
        icon.className = 'fas ' + step.icon + (variant === 'card' ? ' text-xs' : ' mr-2');
        button.appendChild(icon);
        if (variant === 'profile') button.appendChild(document.createTextNode(step.label));
        form.appendChild(button);
        return form;
    }

    function render(box, relationship) {
        var rel = Number(relationship);
        var plan = PLAN[rel];
        if (!plan) return;
        box.setAttribute('data-rel', String(rel));
        while (box.firstChild) box.removeChild(box.firstChild);
        plan.forEach(function (step) { box.appendChild(buildForm(box, step)); });
        var id = box.getAttribute('data-steam-id');
        document.querySelectorAll('[data-friend-status]').forEach(function (el) {
            if (el.getAttribute('data-friend-status') === id) el.textContent = STATUS[rel];
        });
    }

    function boxes() {
        return document.querySelectorAll('[data-friend-actions]');
    }

    window.onFriendAction = function (data, form) {
        version++;
        var ok = !!(data && data.success);
        showToast(data && data.message ? data.message : 'Action failed.', ok ? 'success' : 'error');
        var box = form.closest('[data-friend-actions]');
        // Also on failure: the server says what the relationship really is, so a stale card heals itself.
        if (box && data && typeof data.relationship === 'number') render(box, data.relationship);
    };

    var inflight = false;
    var timer = null;

    function poll() {
        if (document.hidden || inflight) return;
        var started = version;
        inflight = true;
        fetch(STATE_URL, { credentials: 'same-origin', cache: 'no-store', headers: { 'X-Requested-With': 'XMLHttpRequest' } })
            .then(function (response) {
                if (response.status === 401 && timer) { clearInterval(timer); timer = null; } // signed out: stop asking
                return response.ok ? response.json() : null;
            })
            .then(function (data) {
                if (!data || !data.relationships || started !== version) return;
                boxes().forEach(function (box) {
                    var rel = data.relationships[box.getAttribute('data-steam-id')];
                    if (typeof rel === 'number' && rel !== Number(box.getAttribute('data-rel'))) render(box, rel);
                });
            })
            .catch(function () { })
            .then(function () { inflight = false; });
    }

    function init() {
        boxes().forEach(function (box) { render(box, box.getAttribute('data-rel')); });
        document.addEventListener('submit', function () { version++; }, true);
        if (boxes().length === 0) return;
        timer = setInterval(poll, POLL_MS);
        document.addEventListener('visibilitychange', function () { if (!document.hidden) poll(); });
    }

    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init);
    else init();
})();
