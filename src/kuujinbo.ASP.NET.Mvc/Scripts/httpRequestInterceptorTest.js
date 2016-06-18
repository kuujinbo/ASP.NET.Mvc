'use strict';

angular.module('angular1.HttpRequestInterceptorTest', [])
    .controller('HttpRequestInterceptorTestController', ['$scope', '$http', 'XHR_METHOD',
    function ($scope, $http, XHR_METHOD) {
        $scope.method = XHR_METHOD.POST;
        $scope.url = '/Home/Xhr';
        $scope.isTernary = true;

        $scope.fetch = function () {
            $scope.code = null;
            $scope.response = null;

            var config = {
                method: $scope.method,
                url: $scope.url + $scope.method,
                data: { method: $scope.method }
            };

            switch ($scope.method) {
                case XHR_METHOD.POST:
                    break;
                case XHR_METHOD.DELETE:
                    config.params = { id: $scope.method };
                    break;
                case XHR_METHOD.PUT:
                    config.params = { id: $scope.method };
                    break;
                default:
                    config.params = { id: $scope.method };
                    break;
            }

            $http(config).
                then(function (response) {
                    $scope.status = response.status;
                    $scope.data = response.data;
                }, function (response) {
                    $scope.data = response.data || "Request failed";
                    $scope.status = response.status;
                }
            );
        };
    }]
);