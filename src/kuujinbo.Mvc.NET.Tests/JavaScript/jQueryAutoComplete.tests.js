/// <reference path="./../../../src/kuujinbo.Mvc.NET/JavaScript/JQueryAutoComplete.js" />

'use strict';

describe('JQueryAutoComplete', function () {
    var autocomplete, searchSelector;

    beforeEach(function () {
        searchSelector = '#selector';
        setFixtures(
            "<input id='selector' min-search-length='1' search-url='/Home/SearchUsers'>"
        );
    });

    describe('constructor', function () {
        beforeEach(function () {
            autocomplete = new JQueryAutoComplete(searchSelector, function () { });
            autocomplete._jQueryUI = false;
        });

        it('throws when jQuery is missing', function () {

            expect(function () { new JQueryAutoComplete(searchSelector, function () { }) })
                .toThrow(autocomplete.jQueryRequiredError);
        });

 
    });

});