(function () {
    'use strict';

    var app = angular.module('realtor', ['mgcrea.ngStrap.modal', 'mgcrea.ngStrap.alert', 'bw.paging', 'angularSpinner', 'ngFileUpload']);

    app.config(['$httpProvider', '$provide', '$modalProvider', function ($httpProvider, $provide, $modalProvider) {
        //for asp.net mvc is ajax request
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
        $httpProvider.interceptors.push('AuthHttpResponseInterceptor');
    }]);

    app.directive("ngCalender", ['$timeout', function ($timeout) {
        return {
            restrict: 'E',
            replace: true,
            template: '<div class="input-group date">' +
                      '<input type="text" class="form-control" value="{{ngModel}}"/>' +
                      '<span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>' +
                      '</div>',
            scope: {
                ngModel: '=',
                ngDisabled: '=',
                format: '=',
                empty: '=',
                onChange: '&?'
            },
            link: function (scope, element, attributes, ngRange) {

                var container = element;
                var control = element.find('input');
                var index = 0;

                if (ngRange) {
                    index = ngRange.elements.length;
                    ngRange.elements.push(container);
                }

                container.datetimepicker({
                    format: scope.format
                });

                container.on("dp.change", function (e) {

                    if (!e.date && !scope.empty) {
                        container.data("DateTimePicker").date(e.oldDate);
                        e.date = e.oldDate;
                    }

                    $timeout(function () {
                        scope.$apply(function () {

                            scope.ngModel = control.val();

                            if (scope.onChange != null) {
                                scope.onChange(e);
                            }
                        });
                    }, 50);
                });

                scope.$watch('ngModel', function () {
                    $timeout(function () {
                        if (scope.ngModel) {
                            container.data("DateTimePicker").date(scope.ngModel);
                        }
                    }, 50);
                });

            }
        };
    }]);

    app.factory('AuthHttpResponseInterceptor', ['$q', '$window', function ($q, $window) {
        return {
            responseError: function (rejection) {

                if (rejection.status === 401) {
                    //unauthorize..
                    console.log("Response Error 401", rejection);
                    console.log("return url=" + $window.location.pathname)
                    //goto login
                    //$window.location.href = '/Home?returnUrl=' + $window.location.pathname;
                } else if (rejection.status === 500) {
                    //not found..
                    console.log("500 Error", rejection);

                    $window.alert('500 Error : ' + rejection.data.message);
                }

                return $q.reject(rejection);
            }
        }
    }]);

    app.factory("dataService", ["$http", "$q", function ($http, $q) {

        return {
            get: function (url, p) {
                return $http.get(url, {
                    params: p
                });
            },
            post: function (url, item) {
                return $http({
                    url: url,
                    method: 'POST',
                    data: item
                });
            },
            put: function (url, item) {
                return $http({
                    url: url,
                    method: 'PUT',
                    data: item,
                });
            }
        };

    }]);

    app.filter('call', function () {

        return function (txt) {
            var tel = txt.match(/\d/g);
            tel = tel.join("");
            return tel;
        };

    });

    //Add this to have access to a global variable
    app.run(function ($rootScope) {
        
    });

}());