function Xhr(url, doneCallback, failMessage) {
    Object.defineProperty(this, 'urlError', {
        value: 'url required.'
    });
    Object.defineProperty(this, 'doneCallbackError', {
        value: 'doneCallback required, and must be a function().'
    });
    Object.defineProperty(this, 'failTitle', {
        value: 'Unable to Process Request'
    });
    Object.defineProperty(this, 'defaultFailMessage', {
        value: 'There was a problem processing your request. Please try again. If the problem continues please contact the application administrators.'
    });

    // MS @Html.AntiForgeryToken() **IGNORES** HTML4 naming standards:
    // https://www.w3.org/TR/html4/types.html#type-id ('name' token)
    Object.defineProperty(this, 'xsrf', {
        value: '__RequestVerificationToken'
    });

    this._jQueryUI = typeof jQuery.ui !== 'undefined'
                     && typeof jQuery.ui.dialog === 'function';

    if (url) { this._url = url; } else { throw this.urlError; }

    if (doneCallback && typeof doneCallback === 'function') {
        this._doneCallback = doneCallback;
    } else {
        throw this.doneCallbackError;
    }

    this._failMessage = failMessage || this.defaultFailMessage;
}

Xhr.prototype = {
    constructor: Xhr,

    getXsrfToken: function() {
        var token = document.querySelector('input[name=' + this.xsrf + ']');
        if (token !== null) {
            var xsrf = {};
            xsrf[this.xsrf] = token.value;
            return xsrf;
        }
        return null;
    }
    , failModalArgs: function() {
        return { height: 276, width: 476, modal: true, title: this.failTitle };
    }
    , send: function(requestData, requestType, alwaysCallback) {
        var self = this;
        $.ajax({
            url: self._url,
            headers: self.getXsrfToken(),
            data: requestData,
            type: requestType || 'POST'
        })
        .done(function(data, textStatus, jqXHR) {
            self._doneCallback(data);
        })
        .fail(function(jqXHR, textStatus, errorThrown) {
            if (self._jQueryUI) {
                var message = "<h1><span style='color:red' class='glyphicon glyphicon-flag'></span>"
                    + self.failTitle + '</h1>' + self._failMessage;
                $('<div></div>').html(message).dialog(self.failModalArgs());
                return;
            } else {
                alert(self._failMessage);
            }
        })
        .always(function() {
            if (alwaysCallback && typeof alwaysCallback === 'function') { alwaysCallback(); }
        });
    }
}