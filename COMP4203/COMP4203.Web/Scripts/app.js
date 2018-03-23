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
            return $http.get("/api/test/gettestlist", {
                params: params
            });
        },
        getTestsTwo: function (params) {
            return $http.get("/api/testtwo/gettestlist", {
                params: params
            });
        },
        getTestsTwoList: function (params) {
            return $http.get("/api/testtwo/gettesttwolist", {
                params: params
            });
        }
    }
});