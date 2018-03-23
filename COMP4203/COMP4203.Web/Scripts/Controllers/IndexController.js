app.controller("indexController", ["$scope", "dataService", function ($scope, context) {
    $scope.anotherTest = "Another Test";
    $scope.name = "Guest101";
    $scope.message = "";
    $scope.messages = [];
    $scope.testHub = null;
    $scope.testObjectsTwoList = [];

    $scope.newMessage = function () {
        $scope.testHub.server.sendMessage($scope.name, $scope.message);
        $scope.message = "";
    };

    $scope.broadcastTestBtn = function() {
        context.getTestsTwoList({
            aNumber: 1
        }).then(function () {
            console.log("Success!");
        });
    };

    $scope.getTestsData = function () {
        context.getTests({
            aNumber: 100
        }).then(function (response) {
            var data = response.data;

            $scope.testList = data;
        });

        context.getTestsTwo({
            aNumber: 1000
        }).then(function (response) {
            var data = response.data;
            console.log(data);

            $scope.testTwoList = data;
        });
    };

    angular.element(document).ready(function () {
        $scope.testHub = $.connection.testHub;
        $scope.testTwoHub = $.connection.testTwoHub;
        $.connection.hub.start();
        $scope.testHub.client.broadcastMessage = function (name, message) {
            var newMessage = name + " says: " + message;

            $scope.messages.push(newMessage);
            $scope.$apply();
        };
        $scope.testTwoHub.client.broadcastTests = function (jsonList) {
            console.log("RAW JSON: " + jsonList);

            var objects = angular.fromJson(jsonList);

            console.log("JSON PARSED: " + objects);

            angular.forEach(objects, function (value, key) {
                this.push(value);
            }, $scope.testObjectsTwoList);
            $scope.$apply();
        };
        $scope.getTestsData();
    });
}]);


// NEEDS FINISHING OFF