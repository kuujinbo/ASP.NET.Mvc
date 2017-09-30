function JQueryXhr() {
    Object.defineProperty(this, 'doneCallbackError', {
        value: 'doneCallback required, and must be a function().'
    });

    Object.defineProperty(this, 'failTitle', {
        value: 'Unable to Process Request'
    });
    Object.defineProperty(this, 'defaultFailMessage', {
        value: 'There was a problem processing your request. Please try again. If the problem continues please contact the application administrators.'
    });

    // M$ @Html.AntiForgeryToken() **IGNORES** HTML4 naming standards:
    // https://www.w3.org/TR/html4/types.html#method-id ('name' token)
    Object.defineProperty(this, 'xsrf', {
        value: '__RequestVerificationToken'
    });

    Object.defineProperty(this, 'httpMethod', {
        value: {
            GET: 'GET',
            POST: 'POST',
            DELETE: 'DELETE',
            PUT: 'PUT'
        }
    });

    Object.defineProperty(this, 'isFunction', {
        value: function(obj) { return obj && typeof obj === 'function'; }
    });

    var alwaysCallback = null;
    Object.defineProperty(this, 'alwaysCallback', {
        get: function() { return alwaysCallback },
        set: function(callback) {
            if (this.isFunction(callback)) { alwaysCallback = callback; }
            else { throw 'alwaysCallback must be a function'; }
        }
    });

    Object.defineProperty(this, 'blockUiStyleId', {
        value: 'xhr-block-ui-style-element'
    });
    Object.defineProperty(this, 'blockUiStyle', {
        value: ".spin-infinite { height: 60px; width: 60px; margin:20% auto;padding:20px; border-left: 8px solid rgba(0, 0, 0, 0.2); border-right: 8px solid rgba(0, 0, 0, 0.2); border-bottom: 8px solid rgba(0, 0, 0, 0.2); border-top: 8px solid rgba(0, 0, 0, 0.8); border-radius: 100%; -webkit-animation: spin-infinite .8s infinite linear; -moz-animation: spin-infinite 1s infinite linear; -o-animation: spin-infinite 1s infinite linear; animation: spin-infinite 1s infinite linear; } @-moz-keyframes spin-infinite { from {-moz-transform: rotate(0deg);}  to {-moz-transform: rotate(360deg);} } @-webkit-keyframes spin-infinite { from {-webkit-transform: rotate(0deg);} to { -webkit-transform: rotate(360deg);} } @keyframes spin-infinite { from {transform: rotate(0deg);} to {transform: rotate(360deg);} }"
    });
    Object.defineProperty(this, 'blockUiDivId', {
        value: 'xhr-block-ui-element'
    });
    Object.defineProperty(this, 'blockUiDiv', {
        value: "<div style='z-index:88888888;position:fixed;width:100%;height:100%;left:0;top:0;background-color:rgb(0,0,0);background-color:rgba(0,0,0,0.4);'><div class='spin-infinite'></div></div> "
    });

    var failMessage = null;
    Object.defineProperty(this, 'failMessage', {
        get: function() { return failMessage || this.defaultFailMessage; },
        set: function(value) { failMessage = value; }
    });

    this._jQueryUI = typeof jQuery.ui !== 'undefined'
                     && typeof jQuery.ui.dialog === 'function';
}

JQueryXhr.prototype = {
    constructor: JQueryXhr,

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
        return { width: 476, modal: true, title: this.failTitle };
    }
    , addBlockUiStyleElement: function () {
        var blockUiStyleElement;
        if (!(blockUiStyleElement = this.getBlockUiStyleElement())) {
            var style = document.createElement('style');
            style.id = this.blockUiStyleId;
            style.setAttribute('type', 'text/css');
            style.appendChild(document.createTextNode(this.blockUiStyle));
            document.head.appendChild(style);
        }
    }
    , getBlockUiStyleElement: function () {
        return document.querySelector('#' + this.blockUiStyleId);
    }
    , addBlockUiElement: function() {
        var blockUiElement;
        if (!(blockUiElement = this.getBlockUiElement())) {
            var blockUiElement = document.createElement('div');
            blockUiElement.id = this.blockUiDivId;
            blockUiElement.style.display = 'none';
            blockUiElement.innerHTML = this.blockUiDiv;
            document.body.appendChild(blockUiElement);
        }
    }
    , getBlockUiElement: function() {
        return document.querySelector('#' + this.blockUiDivId);
    }
    , send: function(url, doneCallback, data, method) {
        var self = this;
        self.addBlockUiStyleElement();
        self.addBlockUiElement();

        var options = {
            url: url,
            data: data,
            method: method || self.httpMethod.POST
        };
        if (method !== self.httpMethod.GET) options.headers = self.getXsrfToken();

        var blockUiElement = self.getBlockUiElement();
        if (blockUiElement) {
            blockUiElement.style.display = 'block';
        }

        var jqxhr = $.ajax(options)
            .done(function(data, textStatus, jqXHR) {
                if (self.isFunction(doneCallback)) { doneCallback(data); }
                else { throw self.doneCallbackError; }
            })
            .fail(function(jqXHR, textStatus, errorThrown) {
                if (self._jQueryUI) {
                    var message = "<h1><span style='color:red' class='glyphicon glyphicon-flag'></span>"
                        + self.failTitle + '</h1>' + self.failMessage;
                    $('<div></div>').html(message).dialog(self.failModalArgs());
                    return;
                } else {
                    alert(self.failMessage);
                }
            })
            .always(function() {
                if (self.alwaysCallback) { self.alwaysCallback(); }
            });

        if (blockUiElement) {
            jqxhr.always(function() {
                blockUiElement.style.display = 'none';
            });
        }
    }
}