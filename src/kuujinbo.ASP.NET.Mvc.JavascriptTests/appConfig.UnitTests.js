/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc/Scripts/appConfig.js" />
'use strict';

describe('angular1.config xsrfRegex', function () {
    beforeEach(module('angular1.config'));

    var xsrfRegex, XHR_METHOD;
    beforeEach(function () {
        inject(function (_xsrfRegex_, _XHR_METHOD_) {
            xsrfRegex = _xsrfRegex_;
            XHR_METHOD = _XHR_METHOD_;
        });
    });

    it('should match the XSRF methods and ignore case', function () {
        expect(XHR_METHOD.POST.search(xsrfRegex.get())).toEqual(0);
        expect(XHR_METHOD.DELETE.search(xsrfRegex.get())).toEqual(0);
        expect(XHR_METHOD.PUT.search(xsrfRegex.get())).toEqual(0);
        expect('pOSt'.search(xsrfRegex.get())).toEqual(0);
        expect('deLEte'.search(xsrfRegex.get())).toEqual(0);
        expect('PuT'.search(xsrfRegex.get())).toEqual(0);
    });

    it('should NOT match non-XSRF methods', function () {
        expect('0post'.search(xsrfRegex.get())).toEqual(-1);
        expect('delete0'.search(xsrfRegex.get())).toEqual(-1);
        expect('0put0'.search(xsrfRegex.get())).toEqual(-1);
        expect('non-existent HTTP method'.search(xsrfRegex.get())).toEqual(-1);
    });

});


describe('angular1.config XSRFInterceptor', function () {
    // ####################################################################
    // setup
    // ####################################################################
    var $httpProvider;
    beforeEach(module('angular1.config', function(_$httpProvider_) {
        $httpProvider = _$httpProvider_;
    }));

    var XHR_METHOD, XSRF, $httpBackend;
    beforeEach(function() {
        inject(function (_XHR_METHOD_, _XSRF_,  _$httpBackend_) {
            XHR_METHOD = _XHR_METHOD_;
            XSRF = _XSRF_;
            $httpBackend = _$httpBackend_;
        });
    });

    // ####################################################################
    // verify default headers: change value(s) below will **FAIL** test
    // ####################################################################
    function DefaultGetAndDeleteHeaders(headers) {
        return headers['Accept'] === 'application/json, text/plain, */*';
    }
    function DefaultPostAndPutHeaders(headers) {
        return DefaultGetAndDeleteHeaders(headers)
        && headers['Content-Type'] === 'application/json;charset=utf-8'
    }

    // ####################################################################
    // start tests
    // ####################################################################
    describe('initializing config', function() {
        it('should have the defined constants', function() {
            expect(XSRF).toBeDefined();
            expect(XHR_METHOD).toBeDefined();
        });

        it('should have the XSRFInterceptor provider', function() {
            expect($httpProvider.interceptors).toContain('XSRFInterceptor');
        });
    });

    describe('XSRF without DOM XSRF token', function() {
        beforeEach(function() {
            // only testing $http headers set correctly: **REQUIRES** expectation
            expect(true).toBeTruthy();
        });

        afterEach(function() {
            // all expect[GET|POST] complete
            $httpBackend.verifyNoOutstandingExpectation();
            // no extra/unexpected requests
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should only have default headers when POST', inject(function($http) {
            $httpBackend.expectPOST('/url/list', { data: 1 }, function(headers) {
                return DefaultPostAndPutHeaders(headers)
                    // custom request interceptor
                    && headers[XSRF.XHR_HEADER_NAME] === XSRF.XHR_HEADER_VALUE;
            }).respond(200, '');
            $http.post('/url/list', { data: 1 });
            $httpBackend.flush();
        }));

        it('should only have default headers when PUT', inject(function($http) {
            $httpBackend.expectPUT('/url/list', { data: 1 }, function(headers) {
                return DefaultPostAndPutHeaders(headers)
                    // custom request interceptor
                    && headers[XSRF.XHR_HEADER_NAME] === XSRF.XHR_HEADER_VALUE;
            }).respond(200, '');
            $http.put('/url/list', { data: 1 });
            $httpBackend.flush();
        }));

        it('should only have default headers when DELETE', inject(function($http) {
            $httpBackend.expectDELETE('/url/list?id=1', function(headers) {
                return DefaultGetAndDeleteHeaders(headers)
                    // custom request interceptor
                    && headers[XSRF.XHR_HEADER_NAME] === XSRF.XHR_HEADER_VALUE;
            }).respond(200, '');
            $http.delete('/url/list?id=1');
            $httpBackend.flush();
        }));


        describe('XSRF with DOM XSRF token', function() {
            // ####################################################################
            // create/remove DOM XSRF token for POST $http requests: 
            // note token value => '1'
            // ####################################################################
            function injectXsrf() {
                $('body').append(
                    '<input id="xsrf" name="__RequestVerificationToken" type="hidden" value="1">'
                );
            }
            function removeXsrf() { $('#xsrf').remove(); }

            beforeEach(function() { injectXsrf(); });
            afterEach(function() { removeXsrf(); });

            it('should only have default headers when GET', inject(function($http) {
                $httpBackend.expectGET('/url/list', function(headers) {
                    return DefaultGetAndDeleteHeaders(headers)
                        // custom request interceptor
                        && headers[XSRF.XHR_HEADER_NAME] === XSRF.XHR_HEADER_VALUE;
                }).respond(200, '');
                $http.get('/url/list');
                $httpBackend.flush();
            }));

            it('should have default headers and XSRF when DELETE', inject(function($http) {
                $httpBackend.expectDELETE('/url/list?id=1', function(headers) {
                    return DefaultGetAndDeleteHeaders(headers)
                        // custom request interceptor
                        && headers[XSRF.XHR_HEADER_NAME] === XSRF.XHR_HEADER_VALUE
                        && headers[XSRF.TOKEN_NAME] === '1'
                    ;
                }).respond(200, '');
                $http.delete('/url/list?id=1');
                $httpBackend.flush();
            }));


            it('should have default headers and XSRF when POST', inject(function($http) {
                $httpBackend.expectPOST('/url/list', { data: 1 }, function(headers) {
                    return DefaultPostAndPutHeaders
                        // custom request interceptor
                        && headers[XSRF.XHR_HEADER_NAME] === XSRF.XHR_HEADER_VALUE
                        && headers[XSRF.TOKEN_NAME] === '1'
                    ;
                }).respond(200, '');
                $http.post('/url/list', { data: 1 });
                $httpBackend.flush();
            }));

            it('should have default headers and XSRF when PUT', inject(function($http) {
                $httpBackend.expectPUT('/url/list', { data: 1 }, function(headers) {
                    return DefaultPostAndPutHeaders
                        // custom request interceptor
                        && headers[XSRF.XHR_HEADER_NAME] === XSRF.XHR_HEADER_VALUE
                        && headers[XSRF.TOKEN_NAME] === '1'
                    ;
                }).respond(200, '');
                $http.put('/url/list', { data: 1 });
                $httpBackend.flush();
            }));

        });
    });
});