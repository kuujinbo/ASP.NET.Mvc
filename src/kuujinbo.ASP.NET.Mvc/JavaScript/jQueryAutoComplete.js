function jQueryAutoComplete(searchSelector, selectCallback) {
    // selectCallback **MUST** name parameters **EXACTLY** same as below
    // function (event, ui) {}

    Object.defineProperty(this, 'jQueryRequiredError', {
        value: 'jQuery and jQuery UI libraries required.'
    });
    Object.defineProperty(this, 'searchInputError', {
        value: 'Search <input> selector missing or DOM element could not be found.'
    });
    Object.defineProperty(this, 'selectCallbackError', {
        value: 'selectCallback required and **MUST** be a JavaScript function with *EXACT** signature:\n\n'
                + 'function (event, ui) {}'
    });
    Object.defineProperty(this, 'searchUrl', {
        value: 'search-url'
    });

    Object.defineProperty(this, 'minSearchLength', {
        value: 'min-search-length'
    });

    if (typeof jQuery === 'undefined'
        || typeof jQuery.ui === 'undefined'
        || typeof jQuery.ui.dialog !== 'function')
    {
        throw this.jQueryRequiredError;
    }

    this._searchSelector = searchSelector;
    this._searchInputElement = null;
    if (searchSelector 
        && (this._searchInputElement = document.querySelector(searchSelector)))
    {
        this._url = this._searchInputElement.getAttribute(this.searchUrl);
    } else {
        throw this.searchInputError;
    }

    if (selectCallback && typeof selectCallback === 'function') { this._selectCallback = selectCallback; }
    else { throw this.selectCallbackError; }

}

jQueryAutoComplete.prototype = {
    constructor: jQueryAutoComplete
    , autocomplete: function() {
        var that = this;
        $(that._searchInputElement).autocomplete({
            source: function(request, response) {
                that.sendXhr(request, response);
            }
            , minLength: that._searchInputElement.getAttribute(that.minSearchLength) || 1
            , focus: function(event, ui) {
                that._searchInputElement.value = '';
                return false;
            }
            , select: function(event, ui) {
                that._selectCallback(event, ui)
                // console.log('Selected: ' + ui.item.value + ' SOME_PROPERTY ' + ui.item.someProperty);
                return false;
            }
        });
    }
    , sendXhr: function(request, response) {
        var that = this;
        $.ajax({
            url: this._url,
            dataType: 'json',
            data: { searchText: that._searchInputElement.value }
        })
        .done(function (data, textStatus, jqXHR) {
            var resultElement = document.querySelector('ul.ui-autocomplete');
            if (resultElement) {
                if (data.length > 4) {
                    resultElement.style.overflowX = 'hidden';
                    resultElement.style.overflowY = 'auto';
                    resultElement.style.height = '100px';
                } else {
                    resultElement.style.height = 'auto';
                }
            }
            response(data);
        })
        .fail(function(jqXHR, textStatus, errorThrown) {
            alert('Error');
        })
        .always(function () {
            that._searchInputElement.classList.remove('ui-autocomplete-loading');
        });
    }
}