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