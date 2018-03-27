var app = angular.module("app", ["ngRoute", "ui.bootstrap", "highcharts-ng"]);

app.config(function ($routeProvider) {
    $routeProvider.
        when("/home", {
            templateUrl: "Home/Index",
            controller: "indexController"
        }).
        otherwise({
            redirectTo: "Home/Index",
            controller: "indexController"
        });
});

app.factory("dataService", function ($http) {
    return {
        testOutputMessage: function (params) {
            return $http.get("/api/main/testdebug", {
                params: params
            });
        },
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