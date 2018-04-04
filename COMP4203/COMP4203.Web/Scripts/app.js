var app=angular.module("app",["ngRoute","ui.bootstrap","highcharts-ng"]);

app.config(function($routeProvider) {
	$routeProvider.
		when("/home",{
			templateUrl: "Home/Index",
			controller: "indexController"
		}).
		otherwise({
			redirectTo: "Home/Index",
			controller: "indexController"
		});
});

app.factory("dataService",function($http) {
	return {
        run: function (nodeNumber, messageNumber, simSpeedNumber, nodeRange, tabIndex) {
            return $http.get("/api/main/run/" + nodeNumber + "/" + messageNumber + "/" + simSpeedNumber + "/" + nodeRange + "/" + tabIndex, {
			});
		},
		demo: function(tabIndex) {
			return $http.get("/api/main/demo/" + tabIndex,{
			});
		}
	}
});