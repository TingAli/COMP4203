// app as the main module of the web application
var app=angular.module("app",["ngRoute","ui.bootstrap","highcharts-ng"]);

// app configuration such as routing
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

// Data service to allow HTTP Ajax calls
app.factory("dataService",function($http) {
	return {
        run: function (nodeNumber, messageNumber, simSpeedNumber, nodeRange,pureSelfishNodeNumber, partialSelfishNodeNumber, executionNumber, tabIndex) {
            return $http.get("/api/main/run/" + nodeNumber + "/" + messageNumber + "/" + simSpeedNumber + "/" + nodeRange + "/" 
	            + pureSelfishNodeNumber + "/" + partialSelfishNodeNumber + "/" + executionNumber + "/" + tabIndex, {
			});
		},
		reset: function () {
			return $http.get("/api/main/reset");
		}
	}
});

// Custom filter directive to show list items in reverse
app.filter("reverse", function() {
	return function(items) {
		return items.slice().reverse();
	};
});