function Xhr() {
    // MS @Html.AntiForgeryToken() **IGNORES** HTML4 naming standards:
    // https://www.w3.org/TR/html4/types.html#type-id ('name' token)
    this._xsrf = '__RequestVerificationToken';
}

Xhr.prototype = {
    constructor: Xhr,

    getXsrfToken: function (element) {
        var token = element.form.querySelector('input[name=' + this._xsrf + ']');
        if (token !== null) {
            var xsrf = {};
            xsrf[this._xsrf] = token.value;
            return xsrf;
        }
        return null;
    },
    sendXhr: function (element, url, requestData, requestType) {
        var self = this;
        $.ajax({
            url: url,
            headers: self.getXsrfToken(element),
            data: requestData,
            type: requestType || 'POST'
        })
        .done(function (data, textStatus, jqXHR) {
            alert(data);
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(
                jqXHR.responseJSON
                || (jqXHR.status !== 500 ? jqXHR.statusText : 'Error!')
            );
        });
    }
}