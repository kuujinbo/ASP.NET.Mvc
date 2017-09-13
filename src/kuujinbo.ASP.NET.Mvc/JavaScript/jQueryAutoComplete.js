function jQueryAutoComplete(searchSelector, selectCallback) {
    Object.defineProperty(this, 'jQueryRequiredError', {
        value: 'jQuery and jQuery UI libraries required.'
    });
    Object.defineProperty(this, 'searchInputError', {
        value: 'Search <input> selector missing or DOM element could not be found.'
    });
    Object.defineProperty(this, 'selectCallbackError', {
        value: 'selectCallback URL required.'
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

    if (searchUrl) { this._searchUrl = searchUrl; }
    else { throw this.searchUrlError; }

    if (selectCallback && typeof selectCallback === 'function') { this._selectCallback = selectCallback; }
    else { throw this.selectCallbackError; }

}

jQueryAutoComplete.prototype = {
    constructor: jQueryAutoComplete
    , autocomplete: function() {
        $(this._searchInputElement).autocomplete({
            source: function(request, response) {
                this.sendXhr(request, response);
            }
            , minLength: this._searchInputElement.getAttribute(this.minSearchLength) || 1
            , focus: function(event, ui) {
                // this._searchInputElement.value = ui.item.label;
                this._searchInputElement.value = '';
                return false;
            }
            , select: function(event, ui) {
                this._selectCallback(event, ui)
                // console.log('Selected: ' + ui.item.value + ' SOME_PROPERTY ' + ui.item.someProperty);
                return false;
            }
        });
    }
    , sendXhr: function(request, response) {
        $.ajax({
            url: this._url,
            dataType: 'json',
            data: { searchText: this._searchInputElement.value }
        })
        .done(function(data, textStatus, jqXHR) {
            response(data);
        })
        .fail(function(jqXHR, textStatus, errorThrown) {
            alert('Error');
        })
        .always(function() {
            this._searchInputElement.classList.remove('ui-autocomplete-loading');
        });
    }
}