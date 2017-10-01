/// <reference path="./../../../src/kuujinbo.ASP.NET.Mvc/JavaScript/JQueryXhr.js" />

'use strict';

describe('JQueryXhr', function () {
    var jqxhr;
    beforeEach(function () {
        jqxhr = new JQueryXhr();
        setFixtures(
            '<html><head></head><body></body></html>'
        );
    });

    describe('constructor', function () {
        it('initializes the object and creates the read only properties', function () {
            expect(jqxhr).toBeDefined();
            // jQuery UI included in Chutzpah.json
            expect(jqxhr._jQueryUI).toEqual(true);

            // read-only
            expect(function () { jqxhr.doneCallbackError = ''; })
                .toThrowError(TypeError, 'Attempted to assign to readonly property.');
            expect(function () { jqxhr.failTitle = ''; })
                .toThrowError(TypeError, 'Attempted to assign to readonly property.');
            expect(function () { jqxhr.defaultFailMessage = ''; })
                .toThrowError(TypeError, 'Attempted to assign to readonly property.');
            expect(function () { jqxhr.xsrf = ''; })
                .toThrowError(TypeError, 'Attempted to assign to readonly property.');
            expect(function () { jqxhr.blockUiStyleId = ''; })
                .toThrowError(TypeError, 'Attempted to assign to readonly property.');
            expect(function () { jqxhr.blockUiStyleCss = ''; })
                .toThrowError(TypeError, 'Attempted to assign to readonly property.');
            expect(function () { jqxhr.blockUiDivId = ''; })
                .toThrowError(TypeError, 'Attempted to assign to readonly property.');
            expect(function () { jqxhr.blockUiDiv = ''; })
                .toThrowError(TypeError, 'Attempted to assign to readonly property.');
        });

        it('sets the default fail message', function () {
            expect(jqxhr.failMessage).toEqual(jqxhr.defaultFailMessage);
        });
    });

    describe('constructor DOM updates', function () {
        it('adds the block UI style element', function () {
            var element = document.querySelectorAll('#' + jqxhr.blockUiStyleId);

            expect(element.length).toEqual(1);
        });
        it('adds the block UI style element only once', function () {
            var jqhrSecondInstance = new JQueryXhr()
            var element = document.querySelectorAll('#' + jqxhr.blockUiStyleId);

            expect(element.length).toEqual(1);
        });
        it('adds the style element and correct CSS', function () {
            var element = jqxhr.getBlockUiStyleElement();

            expect(element.tagName).toEqual('STYLE');
            expect(element.textContent).toEqual(jqxhr.blockUiStyleCss);
        });

        it('adds the block UI div element', function () {
            var element = document.querySelectorAll('#' + jqxhr.blockUiDivId);

            expect(element.length).toEqual(1);
        });
        it('adds the block UI style element only once', function () {
            var jqhrSecondInstance = new JQueryXhr()
            var element = document.querySelectorAll('#' + jqxhr.blockUiDivId);

            expect(element.length).toEqual(1);
        });
        it('adds the style element and correct CSS', function () {
            var element = jqxhr.getBlockUiElement();
            var quotedInnerHTML = element.innerHTML.replace(/"/g, "'");

            expect(element.tagName).toEqual('DIV');
            expect(quotedInnerHTML).toEqual(jqxhr.blockUiDiv);
        });
    });

    describe('alwaysCallback', function () {
        it('returns null before setting the property', function () {
            expect(jqxhr.alwaysCallback).toBeNull();
        });

        it('throws when alwaysCallback is not a Function', function () {
            expect(function () { jqxhr.alwaysCallback = ''; })
                .toThrow(jqxhr.alwaysCallbackError);
        });

        it('sets the property when the setter is called', function () {
            spyOn(window.console, 'log');

            var message = 'error';
            var callback = function () { console.log(message) }
            jqxhr.alwaysCallback = callback;
            jqxhr.alwaysCallback();

            expect(jqxhr.alwaysCallback).toEqual(jasmine.any(Function));
            expect(window.console.log).toHaveBeenCalledTimes(1);
            expect(window.console.log).toHaveBeenCalledWith(message);
        });
    });

    describe('isFunction()', function () {
        it('returns false when not a function', function () {
            expect(jqxhr.isFunction('a')).toBe(false);
        });

        it('returns true when a function', function () {
            expect(jqxhr.isFunction(function() { })).toBe(true);
        });
    });

    describe('getXsrfToken()', function () {
        it('returns null when the hidden field is not in the DOM', function () {
            expect(jqxhr.getXsrfToken()).toBeNull();
        });

        it('returns the XSRF token when the hidden field is in DOM', function () {
            setFixtures(
                "<input name='__RequestVerificationToken' type='hidden' value='XXX' />"
            );

            var xsrf = jqxhr.getXsrfToken();

            expect(xsrf).not.toBeNull();
            expect(xsrf.__RequestVerificationToken).toEqual('XXX');
        });
    });

    describe('failModalConfig()', function () {
        it('returns a config object for the modal', function () {
            var config = jqxhr.failModalConfig();

            expect(config).toEqual(jasmine.any(Object));
        });
    });

    describe('send()', function () {
        var url, doneCallback, data, method, deferred;

        beforeEach(function () {
            url = '/test.html';
            deferred = new jQuery.Deferred();
            spyOn(jQuery, 'ajax').and.returnValue(deferred);
            spyOn(jqxhr, 'getXsrfToken').and.callThrough();
            setFixtures(
                "<input name='__RequestVerificationToken' type='hidden' value='XXX' />"
            );
        });

        it('throws if doneCallback is missing', function () {
            expect(function () { jqxhr.send(url); deferred.resolve(); })
                .toThrow(jqxhr.doneCallbackError);
        });

        it('throws if doneCallback not a function()', function () {
            expect(function () { jqxhr.send(url, {}); deferred.resolve(); })
                .toThrow(jqxhr.doneCallbackError);
        });

        it('does not call getXsrfToken() for HTTP GET', function () {
            jqxhr.send(url, function () { }, null, jqxhr.httpMethods.GET);

            expect(jqxhr.getXsrfToken).not.toHaveBeenCalled();
        });

        it('calls getXsrfToken() and jQuery.ajax() for any method not GET', function () {
            var data = {};
            var method = jqxhr.httpMethods.PUT;
            var headerName = jqxhr.xsrf;
            var headers = {};
            headers[headerName] = 'XXX';
            var expectedArgs = {
                url: url, data: data, method: method, headers: headers
            };
            jqxhr.send(url, function () { }, data, method);

            expect(jqxhr.getXsrfToken).toHaveBeenCalledTimes(1);
            expect(jQuery.ajax).toHaveBeenCalledTimes(1);
            expect(jQuery.ajax).toHaveBeenCalledWith(expectedArgs);
        });

        it('calls alert() when jQuery UI is **NOT** available and promise is rejected', function () {
            spyOn(window, 'alert');
            var failMessage = 'fail';
            jqxhr = new JQueryXhr();
            jqxhr.failMessage = failMessage;
            // jQuery UI included in Chutzpah.json
            jqxhr._jQueryUI = false;
            jqxhr.send(url, function () { });

            deferred.reject();

            // ajax.fail()
            expect(window.alert).toHaveBeenCalledTimes(1);
            expect(window.alert).toHaveBeenCalledWith(failMessage);
        });

        it('calls dialog() when jQuery UI is available and promise is rejected', function () {
            spyOn(jQuery.fn, 'dialog').and.callThrough();

            jqxhr = new JQueryXhr();
            jqxhr.send(url, function () { });

            deferred.reject();

            // ajax.fail()
            expect(jQuery.fn.dialog).toHaveBeenCalledTimes(1);
            expect(jQuery.fn.dialog).toHaveBeenCalledWith(jqxhr.failModalConfig());
        });

        // send: function(url, doneCallback, data, method) {
        it('calls doneCallback() when XHR is succcessful', function () {
            spyOn(window.console, 'log');
            var doneCallback = jasmine.createSpy().and.callFake(function (data) {
                console.log(data);
            });
            var responseData = 'Ajax success';
            var alwaysData = 'extra always callback';
            var alwaysCallback = jasmine.createSpy().and.callFake(function (data) {
                console.log(alwaysData);
            });

            jqxhr = new JQueryXhr();
            jqxhr.alwaysCallback = alwaysCallback;
            jqxhr.send(url, doneCallback);

            // send() shows the block UI element 
            expect(jqxhr.getBlockUiElement().style.display).toEqual('block');

            deferred.resolve(responseData);

            // ajax.done()
            expect(window.console.log).toHaveBeenCalledWith(responseData);

            // ajax.always() hides the block UI element
            expect(jqxhr.getBlockUiElement().style.display).toEqual('none');
            // extra always callback
            expect(window.console.log).toHaveBeenCalledWith(alwaysData);

            expect(window.console.log).toHaveBeenCalledTimes(2);
        });
    });
});