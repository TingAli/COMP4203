app.controller("indexController",["$scope","dataService","$window",function($scope,context,$window) {
	$scope.outputMessages=[];
	$scope.canvasList=[];

	$scope.initiateRun=function(tabIndex) {
		context.initiateRun({
			nodeNumber: $scope.nodeNumber,
			messageNumber: $scope.messageNumber,
			simSpeedNumber: $scope.simSpeedNumber,
			tabIndex: tabIndex
		}).then(function() {
			var outputMessage={
				Id: $scope.newGuid(),
				Tag: "User",
				Message: "Run Initiated for Session " + tabIndex + "."
			}
			$scope.outputMessages.push(outputMessage);
		});
	}

	$scope.drawNode=function(node) {
		var canvasCtx=$scope.canvasList[node.CanvasIndex];

		canvasCtx.beginPath();
		canvasCtx.arc(node.CenterX,node.CenterY,node.Radius,0,2*Math.PI,false);
		canvasCtx.fillStyle=node.FillColour;
		canvasCtx.fill();
		canvasCtx.lineWidth=node.BorderWidth;
		canvasCtx.strokeStyle=node.StrokeColour;
		canvasCtx.stroke();

		$scope.drawBatteryLevel(node);

		canvasCtx.Nodes.push(node);
	}

	$scope.drawMessageLine=function(nodeStart,nodeEnd) {
		var canvasCtx=$scope.canvasList[nodeStart.CanvasIndex];
		var headlen=10;
		var angle=Math.atan2(nodeEnd.CenterY-nodeStart.CenterY,nodeEnd.CenterX-nodeStart.CenterX);
		var lineHistoryObject={
			NodeStart: nodeStart,
			NodeEnd: nodeEnd,
			IsVisible: true
		};

		canvasCtx.moveTo(nodeStart.CenterX,nodeStart.CenterY);
		canvasCtx.lineTo(nodeEnd.CenterX,nodeEnd.CenterY);
		canvasCtx.lineTo(nodeEnd.CenterX-headlen*Math.cos(angle-Math.PI/6),nodeEnd.CenterY-headlen*Math.sin(angle-Math.PI/6));
		canvasCtx.moveTo(nodeEnd.CenterX,nodeEnd.CenterY);
		canvasCtx.lineTo(nodeEnd.CenterX-headlen*Math.cos(angle+Math.PI/6),nodeEnd.CenterY-headlen*Math.sin(angle+Math.PI/6));
		canvasCtx.lineWidth=2;
		canvasCtx.strokeStyle="#FF00FF";
		canvasCtx.stroke();

		canvasCtx.LineHistory.push(lineHistoryObject);
	}

	$scope.drawBatteryLevel=function(node) {
		var canvasCtx=$scope.canvasList[node.CanvasIndex];
		var batteryLevelX=node.CenterX+(node.Radius*1.15);
		var batteryLevelY=node.CenterY-(node.Radius*1.15);
		var batteryLevelTextHistoryObject={
			X: batteryLevelX,
			Y: batteryLevelY,
			IsVisible: true
		};

		canvasCtx.font="12px Arial";
		canvasCtx.fillStyle="red";
		canvasCtx.fillText(node.BatteryLevel,batteryLevelX,batteryLevelY);

		canvasCtx.BatteryLevelTextHistory.push(batteryLevelTextHistoryObject);
	}

	$scope.populateCanvas=function(nodeList) {
		for(var index=0;index<nodeList.length;index++) {
			$scope.drawNode($scope.canvasList[nodeList[0].CanvasIndex],nodeList[index]);
		}
	}

	$scope.initAndAddCanvas=function(index) {
		var canvas=document.getElementById("canvas_"+index);
		var ctx=canvas.getContext("2d");
		ctx.Nodes=[];
		ctx.LineHistory=[];
		ctx.BatteryLevelTextHistory=[];
		ctx.NumberOfNodesRequested=0;
		ctx.NumberOfMessagesRequested=0;
		ctx.SpeedOfSimulation=2000;
		ctx.SimulationType="";

		$scope.canvasList.push(ctx);
	}

	$scope.newGuid=function() {
		return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g,function(c) {
			var r=Math.random()*16|0,v=c=='x'? r:(r&0x3|0x8);
			return v.toString(16);
		});
	}

	angular.element(document).ready(function() {
		$scope.mainHub=$.connection.mainHub;
		$.connection.hub.start();
		$scope.mainHub.client.broadcastOutputMessage=function(outputMessageJson) {
			var outputMessage=angular.fromJson(outputMessageJson);

			$scope.outputMessages.push(outputMessage);
			$scope.$apply();
		};

		$scope.mainHub.client.sendMessageBetweenTwoNodes=function(nodeStartJson,nodeEndJson) {
			var nodeStart=angular.fromJson(nodeStartJson);
			var nodeEnd=angular.fromJson(nodeEndJson);

			$scope.drawMessageLine(nodeStart,nodeEnd);
			$scope.$apply();
		};

		// TEST START
		$scope.initAndAddCanvas(0);
		var testNodeOne={
			Id: 1,
			FillColour: "#FFFFFF",
			BorderWidth: 2,
			StrokeColour: "#FF0000",
			Radius: 20,
			CenterX: 200,
			CenterY: 200,
			CanvasIndex: 0,
			BatteryLevel: 95
		};

		var testNodeTwo={
			Id: 2,
			FillColour: "#000000",
			BorderWidth: 4,
			StrokeColour: "#FF0000",
			Radius: 20,
			CenterX: 300,
			CenterY: 300,
			CanvasIndex: 0,
			BatteryLevel: 99
		};

		$scope.drawNode(testNodeOne);
		$scope.drawNode(testNodeTwo);

		$scope.drawMessageLine(testNodeOne,testNodeTwo);
		//TEST END
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