'use strict';
var m = angular.module('angular1.config', []);

m.constant('XHR_METHOD', {
    GET: 'GET',
    POST: 'POST',
    DELETE: 'DELETE',
    PUT: 'PUT'
});
m.constant('XSRF', {
    XHR_HEADER_NAME: 'X-Requested-With',
    XHR_HEADER_VALUE: 'XMLHttpRequest',
    TOKEN_NAME: '__RequestVerificationToken'
});

m.config(['$httpProvider', function($httpProvider) {
    $httpProvider.interceptors.push('XSRFInterceptor');
}]);

m.factory('XSRFInterceptor', ['XSRF', 'XHR_METHOD', 'xsrfRegex',
    function (XSRF, XHR_METHOD, xsrfRegex)
{
    return {
        request: function(config) {
            /* AngularJS is one of the few frameworks that **DOES NOT** include
               the following header anymore. reference:
               https://github.com/angular/angular.js/commit/3a75b1124d062f64093a90b26630938558909e8d
               **REQUIRED** for System.Web.Mvc.AjaxRequestExtensions.IsAjaxRequest()
            */
            config.headers[XSRF.XHR_HEADER_NAME] = XSRF.XHR_HEADER_VALUE;

            var csrfToken = angular.element
                (document.querySelectorAll('input[name=' + XSRF.TOKEN_NAME + ']')
            );
            if (config.method.search(xsrfRegex.get()) != -1 && csrfToken.length > 0)
            {
                config.headers[XSRF.TOKEN_NAME] = csrfToken[0].value;
            }

            return config;
        }
    };
}]);

m.factory('xsrfRegex', ['XHR_METHOD', function (XHR_METHOD) {
    return {
        get: function () {
            return new RegExp(
            '^(?:'
                + [XHR_METHOD.POST, XHR_METHOD.DELETE, XHR_METHOD.PUT].join("|")
                + ')$'
            , 'i');
        }
    };
}]);