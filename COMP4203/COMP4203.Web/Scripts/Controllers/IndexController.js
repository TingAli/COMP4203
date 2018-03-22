app.controller("indexController", ["$scope", "dataService", function ($scope, context) {
    $scope.anotherTest = "Another Test";
    $scope.name = "Guest101";
    $scope.message = "";
    $scope.messages = [];
    $scope.testHub = null;

    $scope.newMessage = function () {
        $scope.testHub.server.sendMessage($scope.name, $scope.message);
        $scope.message = "";
    };

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
        $scope.testHub = $.connection.testHub;
        $.connection.hub.start();
        $scope.testHub.client.broadcastMessage = function (name, message) {
            var newMessage = name + " says: " + message;

            $scope.messages.push(newMessage);
            $scope.$apply();
        };
        $scope.getTestsData();
    });
}]);