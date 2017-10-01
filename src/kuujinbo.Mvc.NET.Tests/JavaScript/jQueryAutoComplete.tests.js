/// <reference path="./../../../src/kuujinbo.Mvc.NET/JavaScript/jQueryAutoComplete.js" />

'use strict';

describe('jQueryAutoComplete', function () {
    var autocomplete, searchSelector;

    beforeEach(function () {
        searchSelector = '#selector';
        setFixtures(
            "<input id='selector' min-search-length='1' search-url='/Home/SearchUsers'>"
        );
    });

    describe('constructor', function () {
        beforeEach(function () {
            autocomplete = new jQueryAutoComplete(searchSelector, function () { });
        });

        it('throws when jQuery is missing', function () {
            // jQuery = undefined;

            expect(function () { new jQueryAutoComplete(searchSelector, function () { }) })
                .toThrow(autocomplete.jQueryRequiredError);
        });

 
    });

});