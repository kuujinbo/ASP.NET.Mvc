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

    Object.defineProperty(this, 'resultElementError', {
        value: 'autocomplete results DOM element could not be found.'
    });
    Object.defineProperty(this, 'resultElementSelector', { value: 'ul.ui-autocomplete' });
    Object.defineProperty(this, 'resultElementLoadingClass', { value: 'ui-autocomplete-loading' });

    Object.defineProperty(this, 'searchUrl', { value: 'search-url' });
    Object.defineProperty(this, 'minSearchLength', { value: 'min-search-length' });

    if (typeof jQuery === 'undefined'
        || typeof jQuery.ui === 'undefined'
        || typeof jQuery.ui.dialog !== 'function')
    {
        throw this.jQueryRequiredError;
    }

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
        var self = this;
        $(self._searchInputElement).autocomplete({
            source: function (request, response) {
                var jqxhr = new JQueryXhr();
                jqxhr.alwaysCallback = function () {
                    self._searchInputElement.classList.remove(self.resultElementLoadingClass);
                };
                jqxhr.send(
                    self._url,
                    function (data) { self.sourceCallback(response, data) },
                    { searchText: self._searchInputElement.value },
                    jqxhr.httpMethods.GET
                );
            }
            , minLength: self._searchInputElement.getAttribute(self.minSearchLength) || 1
            , focus: function(event, ui) {
                self._searchInputElement.value = '';
                return false;
            }
            , select: function(event, ui) {
                self._selectCallback(event, ui)
                // console.log('Selected: ' + ui.item.value + ' SOME_PROPERTY ' + ui.item.someProperty);
                return false;
            }
        });
    }
    , sourceCallback: function(response, data) {
        var resultElement = document.querySelector(this.resultElementSelector);
        if (resultElement) {
            if (data.length > 4) {
                resultElement.style.overflowX = 'hidden';
                resultElement.style.overflowY = 'auto';
                resultElement.style.height = '100px';
            } else {
                resultElement.style.height = 'auto';
            }
            response(data);
        } else {
            throw this.resultElementError;
        }
    }
}