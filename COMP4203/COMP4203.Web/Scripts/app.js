var app = angular.module('app', ['ngRoute']);

app.config(function ($routeProvider) {
    $routeProvider.
        when('/home', {
            templateUrl: 'Home/Index'
        }).
        otherwise({
            redirectTo: 'Home/Index'
        });
});

app.factory("dataService", function ($http) {
    return {
        getTests: function (params) {
            return $http.get("api/test/gettestlist", {
                params: params
            });
        }
    }
});