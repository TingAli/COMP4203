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
		initiateRun: function (params) {
			return $http.get("/api/main/run", {
				params: params
			});
		},
        testOutputMessage: function (params) {
            return $http.get("/api/main/testdebug", {
                params: params
            });
        }
    }
});