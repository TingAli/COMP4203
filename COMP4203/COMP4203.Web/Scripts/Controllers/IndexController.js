app.controller("indexController", ["$scope", "dataService", "$window", function ($scope, context, $window) {
    $scope.outputMessages = [];

    angular.element(document).ready(function () {
        $scope.mainHub = $.connection.mainHub;
        $.connection.hub.start();
        $scope.mainHub.client.broadcastOutputMessage = function (outputMessageJson) {
            var outputMessage = angular.fromJson(outputMessageJson);

            $scope.outputMessages.push(outputMessage);
            $scope.$apply();
        };
    });
}]);

//// TEST START
//$scope.anotherTest = "Another Test";
//$scope.name = "Guest101";
//$scope.message = "";
//$scope.messages = [];
//$scope.testHub = null;
//$scope.testObjectsTwoList = [];
//$scope.tabs = [
//    { title: "Dynamic Title 1", content: "Dynamic content 1" },
//    { title: "Dynamic Title 2", content: "Dynamic content 2", disabled: true }
//];

//$scope.addPoints = function () {
//    var seriesArray = $scope.chartConfig.series;
//    var rndIdx = Math.floor(Math.random() * seriesArray.length);
//    seriesArray[rndIdx].data = seriesArray[rndIdx].data.concat([1, 10, 20]);
//};

//var series = 0;
//$scope.addSeries = function () {
//    var rnd = [];
//    for (var i = 0; i < 10; i++) {
//        rnd.push(Math.floor(Math.random() * 20) + 1);
//    }
//    $scope.chartConfig.series.push({
//        data: rnd,
//        id: 'series_' + series++
//    });
//}

//$scope.removeRandomSeries = function () {
//    var seriesArray = $scope.chartConfig.series
//    var rndIdx = Math.floor(Math.random() * seriesArray.length);
//    seriesArray.splice(rndIdx, 1);
//}

//$scope.swapChartType = function () {
//    if (this.chartConfig.chart.type === 'line') {
//        this.chartConfig.chart.type = 'bar';
//    } else {
//        this.chartConfig.chart.type = 'line';
//        this.chartConfig.chart.zoomType = 'x';
//    }
//}

//$scope.chartConfig = {
//    chart: {
//        type: 'bar'
//    },
//    series: [{
//        data: [10, 15, 12, 8, 7],
//        id: 'series1'
//    }],
//    title: {
//        text: 'Hello'
//    }
//}

//$scope.alertMe = function () {
//    setTimeout(function () {
//        $window.alert("You've selected the alert tab!");
//    });
//}

//$scope.newMessage = function () {
//    $scope.testHub.server.sendMessage($scope.name, $scope.message);
//    $scope.message = "";
//}

//$scope.broadcastTestBtn = function () {
//    context.getTestsTwoList({
//        aNumber: 1
//    }).then(function () {
//        console.log("Success!");
//    });
//}

//$scope.getTestsData = function () {
//    context.getTests({
//        aNumber: 100
//    }).then(function (response) {
//        var data = response.data;

//        $scope.testList = data;
//    });

//    context.getTestsTwo({
//        aNumber: 1000
//    }).then(function (response) {
//        var data = response.data;
//        console.log(data);

//        $scope.testTwoList = data;
//    });
//}

//angular.element(document).ready(function () {
//    $scope.testHub = $.connection.testHub;
//    $scope.testTwoHub = $.connection.testTwoHub;
//    $.connection.hub.start();
//    $scope.testHub.client.broadcastMessage = function (name, message) {
//        var newMessage = name + " says: " + message;

//        $scope.messages.push(newMessage);
//        $scope.$apply();
//    };
//    $scope.testTwoHub.client.broadcastTests = function (jsonList) {
//        console.log("RAW JSON: " + jsonList);

//        var objects = angular.fromJson(jsonList);

//        console.log("JSON PARSED: " + objects);

//        angular.forEach(objects,
//            function (value, key) {
//                this.push(value);
//            },
//            $scope.testObjectsTwoList);
//        $scope.$apply();
//    };
//    $scope.getTestsData();
//});
//// TEST END