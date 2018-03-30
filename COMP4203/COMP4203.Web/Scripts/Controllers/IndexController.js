app.controller("indexController",["$scope","dataService","$window","$timeout","$filter",function($scope,context,$window,$timeout,$filter) {
	$scope.outputMessages=[];
	$scope.canvasList=[];

	$scope.initiateRun=function(tabIndex) {
		context.initiateRun({
			nodeNumber: $scope.nodeNumber,
			messageNumber: $scope.messageNumber,
			simSpeedNumber: $scope.simSpeedNumber,
			tabIndex: tabIndex
		}).then(function() {
			$scope.pushOutputMessage("User","Run Initiated for Session "+tabIndex+".");
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
	}

	$scope.drawMessageLine=function(nodeStart,nodeEnd,strokeColour,isRedraw) {
		var canvasCtx=$scope.canvasList[nodeStart.CanvasIndex];
		var headlen=10;
		var angle=Math.atan2(nodeEnd.CenterY-nodeStart.CenterY,nodeEnd.CenterX-nodeStart.CenterX);
		var lineHistoryObject={
			NodeStart: nodeStart,
			NodeEnd: nodeEnd,
			StrokeColour: strokeColour,
			IsVisible: true
		};

		$scope.setMessageLinesToFalse(nodeStart.CanvasIndex);

		canvasCtx.moveTo(nodeStart.CenterX,nodeStart.CenterY);
		canvasCtx.lineTo(nodeEnd.CenterX,nodeEnd.CenterY);
		canvasCtx.lineTo(nodeEnd.CenterX-headlen*Math.cos(angle-Math.PI/6),nodeEnd.CenterY-headlen*Math.sin(angle-Math.PI/6));
		canvasCtx.moveTo(nodeEnd.CenterX,nodeEnd.CenterY);
		canvasCtx.lineTo(nodeEnd.CenterX-headlen*Math.cos(angle+Math.PI/6),nodeEnd.CenterY-headlen*Math.sin(angle+Math.PI/6));
		canvasCtx.lineWidth=2;
		canvasCtx.strokeStyle=strokeColour;
		canvasCtx.stroke();

		canvasCtx.LineHistory.push(lineHistoryObject);

		if(!isRedraw) {
			$scope.reDrawCurrentState(nodeStart.CanvasIndex);
		}
	}

	$scope.setMessageLinesToFalse=function(canvasIndex) {
		var canvasCtx=$scope.canvasList[canvasIndex];

		for(var lineIndex=0;lineIndex<canvasCtx.LineHistory.length;lineIndex++) {
			canvasCtx.LineHistory[lineIndex].IsVisible=false;
		}
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

	$scope.updateBatteryLevel=function(node) {
		var found=$filter("filter")($scope.canvasList[node.CanvasIndex].BatteryLevelTextHistory,{ Id: node.Id },true);
		if(found.length) {
			found[0].IsVisible=false;
		}

		$scope.reDrawCurrentState(node.CanvasIndex);
	}

	$scope.reDrawCurrentState=function(canvasIndex) {
		var canvasCtx=$scope.canvasList[canvasIndex];

		canvasCtx.clearRect(0,0,500,500);

		for(var nodeIndex=0;nodeIndex<canvasCtx.Nodes.length;nodeIndex++) {
			$scope.drawNode(canvasCtx.Nodes[nodeIndex]);
		}

		$scope.drawMessageLine(
			canvasCtx.LineHistory[canvasCtx.LineHistory.length-1].NodeStart,
			canvasCtx.LineHistory[canvasCtx.LineHistory.length-1].NodeEnd,
			canvasCtx.LineHistory[canvasCtx.LineHistory.length-1].StrokeColour,true);
	}

	$scope.populateCanvas=function(nodeList) {
		for(var index=0;index<nodeList.length;index++) {
			var canvasCtx=$scope.canvasList[nodeList[index].CanvasIndex];

			$scope.drawNode(nodeList[index]);
			canvasCtx.Nodes.push(nodeList[index]);
		}
	}

	$scope.initAndAddCanvas=function(index) {
		var canvas=document.getElementById("canvas_"+index);
		var ctx=canvas.getContext("2d");
		ctx.Nodes=[];
		ctx.LineHistory=[];
		ctx.BatteryLevelTextHistory=[];

		$scope.canvasList.push(ctx);
	}

	$scope.newGuid=function() {
		return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g,function(c) {
			var r=Math.random()*16|0,v=c==="x"? r:(r&0x3|0x8);
			return v.toString(16);
		});
	}

	$scope.reset=function(tabIndex) {
		$scope.canvasList[tabIndex].clearRect(0,0,500,500);
		$scope.canvasList[tabIndex].Nodes=[];
		$scope.canvasList[tabIndex].LineHistory=[];
		$scope.canvasList[tabIndex].BatteryLevelTextHistory=[];
		$scope.nodeNumber="";
		$scope.messageNumber="";
		$scope.simSpeedNumber="";

		$scope.pushOutputMessage("User","Reset applied to Session "+tabIndex+".");
	}

	$scope.pushOutputMessage=function(tag,message) {
		var outputMessage={
			Id: $scope.newGuid(),
			Tag: tag,
			Message: message
		};
		$scope.outputMessages.push(outputMessage);
	}

	$scope.runTest=function(tabIndex) {
		$scope.pushOutputMessage("User","Test Initiated for Session "+tabIndex+".");

		var testNodeList=[
			{
				Id: $scope.newGuid(),
				FillColour: "#FFFFFF",
				BorderWidth: 2,
				StrokeColour: "#FF0000",
				Radius: 10,
				CenterX: 30,
				CenterY: 50,
				CanvasIndex: tabIndex,
				BatteryLevel: 99,
				IsFinal: false
			},
			{
				Id: $scope.newGuid(),
				FillColour: "#000000",
				BorderWidth: 4,
				StrokeColour: "#FF0000",
				Radius: 10,
				CenterX: 30,
				CenterY: 125,
				CanvasIndex: tabIndex,
				BatteryLevel: 99,
				IsFinal: false
			},
			{
				Id: $scope.newGuid(),
				FillColour: "#000000",
				BorderWidth: 4,
				StrokeColour: "#FF0000",
				Radius: 10,
				CenterX: 150,
				CenterY: 280,
				CanvasIndex: tabIndex,
				BatteryLevel: 99,
				IsFinal: false
			},
			{
				Id: $scope.newGuid(),
				FillColour: "#000000",
				BorderWidth: 4,
				StrokeColour: "#00FF00",
				Radius: 10,
				CenterX: 220,
				CenterY: 370,
				CanvasIndex: tabIndex,
				BatteryLevel: 99,
				IsFinal: true
			}
		];

		$scope.nodeNumber=testNodeList.length;
		$scope.messageNumber=1;
		$scope.simSpeedNumber=200;
		$scope.populateCanvas(testNodeList);

		$scope.drawMessageLine(testNodeList[0],testNodeList[1],"#FF0000");

		$timeout(function() {
			$scope.drawMessageLine(testNodeList[1],testNodeList[2],"#FF0000");
		},$scope.simSpeedNumber);

		$timeout(function() {
			$scope.drawMessageLine(testNodeList[2],testNodeList[3],"#FF0000");
		},$scope.simSpeedNumber*2);

		$timeout(function() {
			testNodeList[2].BatteryLevel=84;
			$scope.updateBatteryLevel(testNodeList[2]);
		},$scope.simSpeedNumber*3);
	}

	angular.element(document).ready(function() {
		for(var index=0;index<3;index++) {
			$scope.initAndAddCanvas(index);
		}
		$scope.mainHub=$.connection.mainHub;
		$.connection.hub.start();
		$scope.mainHub.client.broadcastOutputMessage=function(outputMessageJson) {
			var outputMessage=angular.fromJson(outputMessageJson);

			$scope.pushOutputMessage(outputMessage.Tag,outputMessage.Message);
			$scope.$apply();
		};

		$scope.mainHub.client.sendMessageBetweenTwoNodes=function(nodeStartJson,nodeEndJson) {
			var nodeStart=angular.fromJson(nodeStartJson);
			var nodeEnd=angular.fromJson(nodeEndJson);

			$scope.drawMessageLine(nodeStart,nodeEnd,"#FF00FF");
			$scope.$apply();
		};

		$scope.mainHub.client.updateBatteryLevel=function(nodeJson) {
			var node=angular.fromJson(nodeJson);

			$scope.updateBatteryLevel(node);
			$scope.$apply();
		};
	});
}]);