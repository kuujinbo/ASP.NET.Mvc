/// <reference path="./../../../src/kuujinbo.ASP.NET.Mvc/JavaScript/SessionTerminator.js" />

'use strict';

describe('SessionTerminator', function() {
    var terminator, goodTimeout, goodLogoutUrl, invalidTimeout;

    beforeEach(function() {
        terminator = new SessionTerminator();
        goodTimeout = 4;
        goodLogoutUrl = '/app';
        invalidTimeout = terminator.maxTimeout + 1;
    });

    describe('new instance', function() {
        it('initializes defaults', function() {
            expect(terminator._timeoutID).toBe(0);
            expect(terminator._idleTimeout).toBe(terminator.adminTimeout);
            expect(terminator._logoutUrl).toBe('');
        });
    });

    describe('init', function() {
        it('throws when logoutUrl not passed', function() {
            expect(function() { terminator.init(); }).toThrow(terminator.initError);
        });

        it('throws when logoutUrl is invalid', function() {
            expect(function() { terminator.init(goodTimeout, ''); })
                 .toThrow(terminator.initError);
            expect(function() { terminator.init(goodTimeout, '---'); })
                 .toThrow(terminator.initError);
            expect(function() { terminator.init(goodTimeout, null); })
                 .toThrow(terminator.initError);
        });

        it('sets the default timeout when timeoutInSeconds not passed', function() {
            terminator.init(null, '/a');

            expect(terminator._idleTimeout).toBe(terminator.maxTimeout * 1000);
        });

        it('sets the default timeout when timeoutInSeconds is greater than allowed', function() {
            terminator.init(invalidTimeout, goodLogoutUrl);

            expect(terminator._idleTimeout).toBe(terminator.maxTimeout * 1000);
        });

        it('adds event listeners when valid parameters are passed', function() {
            terminator.init(goodTimeout, goodLogoutUrl);

            expect(window.onload).toEqual(jasmine.any(Function));
            expect(window.onkeypress).toEqual(jasmine.any(Function));
            expect(window.onmousemove).toEqual(jasmine.any(Function));
            expect(window.onscroll).toEqual(jasmine.any(Function));
        });
    });

    describe('logout', function() {
        it('calls window.location.replace() ', function() {
            spyOn(window.location, 'replace');

            terminator.init(goodTimeout, goodLogoutUrl);
            terminator.logout();

            expect(window.location.replace).toHaveBeenCalledTimes(1);
            expect(window.location.replace).toHaveBeenCalledWith(goodLogoutUrl);
        });
    });

    describe('reset', function() {
        it('calls window.clearTimeout() if _timeoutID greater than 0', function() {
            spyOn(window, 'clearTimeout');
            spyOn(window, 'setTimeout');
            var fakeId = 1;

            terminator.init(goodTimeout, goodLogoutUrl);
            terminator._timeoutID = fakeId;
            terminator.reset();

            expect(window.clearTimeout).toHaveBeenCalledTimes(1);
            expect(window.clearTimeout).toHaveBeenCalledWith(fakeId);
            // setTimeout() always called
            expect(window.setTimeout).toHaveBeenCalledTimes(1);
            expect(window.setTimeout).toHaveBeenCalledWith(jasmine.any(Function), goodTimeout * 1000);
        });

        it('does not call window.clearTimeout() if _timeoutID less than 1', function() {
            spyOn(window, 'clearTimeout');
            spyOn(window, 'setTimeout').and.callThrough();

            terminator.init(goodTimeout, goodLogoutUrl);
            terminator.reset();

            expect(window.clearTimeout).not.toHaveBeenCalled();
            expect(terminator._timeoutID).toBeGreaterThan(0);
            expect(window.setTimeout).toHaveBeenCalledTimes(1);
            expect(window.setTimeout).toHaveBeenCalledWith(jasmine.any(Function), goodTimeout * 1000);
        });

        it('calls reset on  window.onload()', function() {
            spyOn(terminator, 'reset');
            terminator.init(goodTimeout, goodLogoutUrl);
            window.onload();

            expect(terminator.reset).toHaveBeenCalledTimes(1);
        });

        it('calls reset on  window.onkeypress()', function() {
            spyOn(terminator, 'reset');
            terminator.init(goodTimeout, goodLogoutUrl);
            window.onkeypress();

            expect(terminator.reset).toHaveBeenCalledTimes(1);
        });

        it('calls reset on  window.onmousemove()', function() {
            spyOn(terminator, 'reset');
            terminator.init(goodTimeout, goodLogoutUrl);
            window.onmousemove();

            expect(terminator.reset).toHaveBeenCalledTimes(1);
        });

        it('calls reset on  window.onscroll()', function() {
            spyOn(terminator, 'reset');
            terminator.init(goodTimeout, goodLogoutUrl);
            window.onscroll();

            expect(terminator.reset).toHaveBeenCalledTimes(1);
        });
    });

    describe('showLogoutMessage', function() {
        it('calls window.alert() if jQuery is not available', function() {
            spyOn(window, 'alert');

            // jQuery UI included in Chutzpah.json
            terminator._jQueryUI = false;
            terminator.init(goodTimeout, goodLogoutUrl);
            terminator.showLogoutMessage();

            expect(window.alert).toHaveBeenCalledTimes(1);
            expect(window.alert).toHaveBeenCalledWith(jasmine.any(String));
        });

        it('calls showDialog() if jQuery is available', function() {
            spyOn(terminator, 'showDialog').and.callThrough;

            terminator.init(goodTimeout, goodLogoutUrl);
            terminator.showLogoutMessage();

            expect(terminator.showDialog).toHaveBeenCalledTimes(1);
            expect(terminator.showDialog).toHaveBeenCalledWith(jasmine.any(String));
        });
    });
});