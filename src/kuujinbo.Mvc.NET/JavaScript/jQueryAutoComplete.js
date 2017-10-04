function JQueryAutoComplete() {
    Object.defineProperty(this, 'jQueryRequiredError', {
        value: 'jQuery and jQuery UI libraries required.'
    });
    Object.defineProperty(this, 'searchInputError', {
        value: 'Search <input> selector missing or DOM element could not be found.'
    });
    Object.defineProperty(this, 'selectCallbackError', {
        value: 'selectCallback required and **MUST** be a JavaScript function with *EXACT** signature:\n\n'
                + 'function(event, ui) {}'
    });

    Object.defineProperty(this, 'resultElementError', {
        value: 'autocomplete results DOM element could not be found.'
    });
    Object.defineProperty(this, 'resultElementSelector', { value: 'ul.ui-autocomplete' });
    Object.defineProperty(this, 'resultElementLoadingClass', { value: 'ui-autocomplete-loading' });

    Object.defineProperty(this, 'searchUrl', { value: 'search-url' });
    Object.defineProperty(this, 'minSearchLength', { value: 'min-search-length' });

    this._jQueryUI = jQuery && jQuery.ui && typeof jQuery.ui.dialog === 'function';
}

JQueryAutoComplete.prototype = {
    constructor: JQueryAutoComplete

    , validateSearchSelector: function(searchSelector) {
        var searchInputElement = null;
        if (searchSelector
            && (searchInputElement = document.querySelector(searchSelector)))
        {
            return searchInputElement;
        } else {
            throw this.searchInputError;
        }
    }
    , validateSelectCallback: function(selectCallback) {
        if (!selectCallback || typeof selectCallback !== 'function') {
            throw this.selectCallbackError;
        }
    }
    // selectCallback **MUST** name parameters **MUST EXACTLY MATCH**:
    // function(event, ui) {}
    , autocomplete: function(searchSelector, selectCallback) {
        if (!this._jQueryUI) throw this.jQueryRequiredError;
        var searchInputElement = this.validateSearchSelector(searchSelector);
        this.validateSelectCallback(selectCallback);

        var self = this;
        $(searchInputElement).autocomplete({
            source: function(request, response) {
                var jqxhr = new JQueryXhr();
                jqxhr.alwaysCallback = function() {
                    searchInputElement.classList.remove(self.resultElementLoadingClass);
                };
                jqxhr.send(
                    searchInputElement.getAttribute(self.searchUrl),
                    function(data) { self.sourceCallback(response, data) },
                    { searchText: searchInputElement.value },
                    jqxhr.httpMethods.GET
                );
            }
            , minLength: searchInputElement.getAttribute(self.minSearchLength) || 1
            , focus: function(event, ui) {
                searchInputElement.value = '';
                return false;
            }
            , select: function(event, ui) {
                selectCallback(event, ui)
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