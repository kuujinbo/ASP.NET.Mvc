function SessionTerminator() {
    Object.defineProperty(this, 'initError', {
        value: 'logoutUrl required'
    });
    Object.defineProperty(this, 'adminTimeout', {
        value: 10 * 60 // STIG
    });
    Object.defineProperty(this, 'maxTimeout', {
        value: 15 * 60 // STIG
    });

    this._timeoutID = 0;
    this._idleTimeout = this.adminTimeout;
    this._logoutUrl = '';
}

SessionTerminator.prototype = {
    constructor: SessionTerminator,
    init: function(timeoutInSeconds, logoutUrl) {
        if (!logoutUrl || !logoutUrl.match(/[\/\w]+/)) throw this.initError;

        this._idleTimeout = timeoutInSeconds && timeoutInSeconds < this.maxTimeout
            ? timeoutInSeconds * 1000
            : this.maxTimeout * 1000;
        this._logoutUrl = logoutUrl;
        window.onload = this.reset.bind(this);
        window.onkeypress = this.reset.bind(this);
        window.onmousemove = this.reset.bind(this);
        window.onscroll = this.reset.bind(this);
        //window.onmousedown = this.reset.bind(this);
        //window.ontouchstart = this.reset.bind(this);
        //window.onclick = this.reset.bind(this);
    },
    logout: function() {
        // don't allow back button
        window.location.replace(this._logoutUrl);
    },
    reset: function() {
        if (this._timeoutID > 0) clearTimeout(this._timeoutID);

        this._timeoutID = setTimeout(this.logout.bind(this), this._idleTimeout);
    },
    showDialog: function(message) {
        $('<div></div>').html(message).dialog({ width: 'auto', modal: true, title: 'Session Logout' });
    },
    showLogoutMessage: function() {
        var d = new Date().toLocaleString();
        if (typeof jQuery !== 'undefined'
            && typeof jQuery.ui !== 'undefined'
            && typeof jQuery.ui.dialog === 'function')
        {
            var logoutMessage = "<h1><span style='color:red' class='glyphicon glyphicon-flag'></span>Session Logout</h1>"
                + "<p><strong style='text-decoration:underline'>Per DOD Cybersecurity policy</strong> you were logged out of the application at <em>"
                + d
                + '</em> due to inactivity.</p>';

            this.showDialog(logoutMessage);
        } else {
            var logoutMessage = 'Session Logout\n\n'
                + 'Per DOD Cybersecurity policy you were logged out of the application at '
                + d
                + ' due to inactivity.';
            alert(logoutMessage);
        }
    }
}