/// <reference path="./../../../src/kuujinbo.ASP.NET.Mvc/JavaScript/jQueryAjaxHelper.js" />

'use strict';

describe('jQueryAjaxHelper', function() {
    var xhr, url;

    beforeEach(function () {
        url = '/test.html';
    });

    describe('constructor', function () {
        beforeEach(function () {
            xhr = new Xhr(url, function () {  });
        });

        it('throws when url is missing', function () {
            expect(function () { new Xhr(null, function () { }) })
                .toThrow(xhr.urlError);
        });

        it('throws when doneCallback is missing', function () {
            expect(function () { new Xhr(url, null) })
                .toThrow(xhr.doneCallbackError);
        });

        it('throws when doneCallback is not a function', function () {
            expect(function () { new Xhr(url, 1) })
                .toThrow(xhr.doneCallbackError);
        });

        it('sets the default message when failMessage is missing', function () {
            expect(xhr._failMessage).toEqual(xhr.defaultFailMessage);
        });

        it('sets the message when failMessage is set', function () {
            var message = 'message';
            xhr = new Xhr(url, function () { }, message);

            expect(xhr._failMessage).toEqual(message);
        });

        it('initializes Xhr when valid paramters passed', function () {
            expect(xhr).toBeDefined();
            expect(xhr._doneCallback).toEqual(jasmine.any(Function));
            // jQuery UI included in Chutzpah.json
            expect(xhr._jQueryUI).toEqual(true);
        });
    });

    describe('getXsrfToken()', function () {
        beforeEach(function () {
            xhr = new Xhr(url, function(){});
        });

        it('returns null when the hidden field is not in the DOM', function () {
            expect(xhr.getXsrfToken()).toBeNull()
        });

        it('returns the XSRF token when the hidden field is in DOM', function () {
            setFixtures(
                "<input name='__RequestVerificationToken' type='hidden' value='XXX' />"
            );

            var xsrf = xhr.getXsrfToken();

            expect(xsrf).not.toBeNull();
            expect(xsrf.__RequestVerificationToken).toEqual('XXX');
        });
    });

    describe('send()', function () {
        var deferred, alwaysCallback, alwaysLogMessage;

        beforeEach(function () {
            deferred = new jQuery.Deferred();
            spyOn(jQuery, 'ajax').and.returnValue(deferred);
            spyOn(xhr, 'getXsrfToken');
            alwaysLogMessage = 'In alwaysCallback()';
            alwaysCallback = function() { console.log(alwaysLogMessage) };
            spyOn(window.console, 'log');
        });

        it('calls getXsrfToken() and jQuery.ajax()', function () {
            var requestType = 'POST';
            var expectedArgs = {
                url: url, headers: undefined, data: {  }, type: requestType
            };
            xhr.send({}, requestType);

            expect(xhr.getXsrfToken).toHaveBeenCalledTimes(1);
            expect(jQuery.ajax).toHaveBeenCalledTimes(1);
            expect(jQuery.ajax).toHaveBeenCalledWith(expectedArgs);
        });

        it('calls alert() when jQuery UI is **NOT** available and promise is rejected', function () {
            spyOn(window, 'alert');
            var failMessage = 'fail';
            xhr = new Xhr(url, function () { }, failMessage);
            // jQuery UI included in Chutzpah.json
            xhr._jQueryUI = false;
            xhr.send({}, 'GET', alwaysCallback);

            deferred.reject();

            // ajax.fail()
            expect(window.alert).toHaveBeenCalledTimes(1);
            expect(window.alert).toHaveBeenCalledWith(failMessage);
            // ajax.always()
            expect(window.console.log).toHaveBeenCalledTimes(1);
            expect(window.console.log).toHaveBeenCalledWith(alwaysLogMessage);
        });

        it('calls dialog() when jQuery UI is available and promise is rejected', function () {
            spyOn(jQuery.fn, 'dialog').and.callThrough();

            var failMessage = 'fail';
            xhr = new Xhr(url, function () { }, failMessage);
            xhr.send({ test: 1 }, 'GET', alwaysCallback);

            deferred.reject();

            // ajax.fail()
            expect(jQuery.fn.dialog).toHaveBeenCalledTimes(1);
            expect(jQuery.fn.dialog).toHaveBeenCalledWith(xhr.failModalArgs());
            // ajax.always()
            expect(window.console.log).toHaveBeenCalledTimes(1);
            expect(window.console.log).toHaveBeenCalledWith(alwaysLogMessage);
        });

        it('calls _doneCallback() when the XHR is succcess', function () {
            var doneCallback = function (data) {
                console.log(data);
            };
            var responseData = 'Ajax success';

            spyOn(xhr, '_doneCallback').and.callThrough();

            xhr = new Xhr(url, doneCallback);
            xhr.send({ data: 'data' }, 'POST', alwaysCallback);

            deferred.resolve(responseData, alwaysCallback);

            // ajax.done()
            expect(window.console.log).toHaveBeenCalledWith(responseData);
            // ajax.always()
            expect(window.console.log).toHaveBeenCalledWith(alwaysLogMessage);
            // total calls from above
            expect(window.console.log).toHaveBeenCalledTimes(2);
        });
    });
});