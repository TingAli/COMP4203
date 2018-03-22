app.controller("indexController", ["$scope", "dataService", function ($scope, context) {
    $scope.anotherTest = "Another Test";

    $scope.getTestsData = function () {
        context.getTests({
            aNumber: "100"
        }).then(function (response) {
            var data = response.data;
            console.log(data);

            $scope.testList = data;
        });
    };

    angular.element(document).ready(function () {
        $scope.getTestsData();
    });
}]);