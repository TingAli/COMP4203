﻿app.controller("indexController",["$scope","dataService","$window","$timeout","$filter",function($scope,context,$window,$timeout,$filter) {
	$scope.runData={};
	$scope.locks={};
	$scope.outputMessages=[];
	$scope.canvasList=[];
	$scope.notification={};

	$scope.generate=function(tabIndex) {
		context.generate($scope.runData.nodeNumber,$scope.runData.messageNumber,$scope.runData.simSpeedNumber,$scope.runData.nodeRange,
			$scope.runData.pureSelfishNodeNumber,$scope.runData.partialSelfishNodeNumber,tabIndex)
			.then(function() {
				$scope.locks.GenerateButton=true;
				$scope.locks.RunButton=false;
				$scope.locks.DemoButtons=true;
				$scope.locks.Inputs=true;
			});
	}

	$scope.initiateRun=function(tabIndex) {

		context.run($scope.runData.nodeNumber,$scope.runData.messageNumber,$scope.runData.simSpeedNumber,$scope.runData.nodeRange,
			$scope.runData.pureSelfishNodeNumber,$scope.runData.partialSelfishNodeNumber,tabIndex)
			.then(function() {
				$scope.runData.isRunning=true;
				$scope.locks.GenerateButton=true;
				$scope.locks.RunButton=true;
				$scope.locks.DemoButtons=true;
				$scope.locks.Inputs=true;
			});
	}

	$scope.drawNode=function(node) {
		var canvasCtx=$scope.canvasList[node.CanvasIndex];

		canvasCtx.beginPath();
		canvasCtx.arc(node.CenterX,node.CenterY,node.Radius,0,2*Math.PI,false);
		canvasCtx.fillStyle=node.FillColour;
		canvasCtx.fill();
		canvasCtx.lineWidth=node.BorderWidth;
		canvasCtx.setLineDash([0]);
		canvasCtx.strokeStyle=node.StrokeColour;
		canvasCtx.stroke();

		$scope.drawNodeRange(node);
		$scope.drawBatteryLevel(node);
	}

	$scope.drawNodeRange=function(node) {
		var canvasCtx=$scope.canvasList[node.CanvasIndex];
		var nodeRangeHistoryObject={
			Id: node.Id,
			X: node.CenterX,
			Y: node.CenterY,
			IsVisible: true
		};

		canvasCtx.beginPath();
		canvasCtx.setLineDash([5]);
		canvasCtx.arc(node.CenterX,node.CenterY,$scope.runData.nodeRange,0,2*Math.PI,false);
		canvasCtx.strokeStyle="#D3D3D3";
		canvasCtx.stroke();

		canvasCtx.NodeRangeHistory.push(nodeRangeHistoryObject);
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

		canvasCtx.beginPath();
		canvasCtx.moveTo(nodeStart.CenterX,nodeStart.CenterY);
		canvasCtx.lineTo(nodeEnd.CenterX,nodeEnd.CenterY);
		canvasCtx.lineTo(nodeEnd.CenterX-headlen*Math.cos(angle-Math.PI/6),nodeEnd.CenterY-headlen*Math.sin(angle-Math.PI/6));
		canvasCtx.moveTo(nodeEnd.CenterX,nodeEnd.CenterY);
		canvasCtx.lineTo(nodeEnd.CenterX-headlen*Math.cos(angle+Math.PI/6),nodeEnd.CenterY-headlen*Math.sin(angle+Math.PI/6));
		canvasCtx.lineWidth=2;
		canvasCtx.setLineDash([0]);
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
			Id: node.Id,
			X: batteryLevelX,
			Y: batteryLevelY,
			IsVisible: true
		};

		canvasCtx.beginPath();
		canvasCtx.font="12px Arial";
		canvasCtx.fillStyle="red";
		canvasCtx.fillText(node.BatteryLevel,batteryLevelX,batteryLevelY);
		canvasCtx.setLineDash([0]);
		canvasCtx.stroke();

		canvasCtx.BatteryLevelTextHistory.push(batteryLevelTextHistoryObject);
	}

	$scope.updateBatteryLevel=function(node) {
		var foundHistory=$filter("filter")($scope.canvasList[node.CanvasIndex].BatteryLevelTextHistory,{ Id: node.Id },true);
		var found=$filter("filter")($scope.canvasList[node.CanvasIndex].Nodes,{ Id: node.Id },true);
		if(found.length&&foundHistory.length) {
			foundHistory[0].IsVisible=false;
			found[0].BatteryLevel=parseFloat(Math.round(node.BatteryLevel*100)/100).toFixed(2);
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
		ctx.NodeRangeHistory=[];

		$scope.canvasList.push(ctx);
	}

	$scope.newGuid=function() {
		return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g,function(c) {
			var r=Math.random()*16|0,v=c==="x"? r:(r&0x3|0x8);
			return v.toString(16);
		});
	}

	$scope.reset=function() {
		context.reset()
			.then(function() {
				$scope.runData.isRunning=false;
				$scope.locks.GenerateButton=false;
				$scope.locks.RunButton=true;
				$scope.locks.DemoButtons=false;
				$scope.locks.Inputs=false;
				for(var index=0;index<$scope.canvasList.length; index++ )
				{
					$scope.canvasList[index].clearRect(0,0,500,500);
					$scope.canvasList[index].Nodes=[];
					$scope.canvasList[index].LineHistory=[];
					$scope.canvasList[index].BatteryLevelTextHistory=[];
					$scope.canvasList[index].NodeRangeHistory=[];
				}
				$scope.runData.nodeNumber=0;
				$scope.runData.pureSelfishNodeNumber=0;
				$scope.runData.partialSelfishNodeNumber=0;
				$scope.runData.messageNumber=0;
				$scope.runData.simSpeedNumber=0;
				$scope.runData.nodeRange=200;
			});
	}

	$scope.pushOutputMessage=function(tag,message) {
		var outputMessage={
			Id: $scope.newGuid(),
			Tag: tag,
			Message: message
		};
		$scope.outputMessages.push(outputMessage);
	}

	$scope.runDemo=function(tabIndex) {
		$scope.runData.isRunning=true;
		$scope.locks.GenerateButton=true;
		$scope.locks.RunButton=true;
		$scope.locks.DemoButtons=true;
		$scope.locks.Inputs=true;

		$scope.runData.nodeNumber=4;
		$scope.runData.pureSelfishNodeNumber=0;
		$scope.runData.partialSelfishNodeNumber=0;
		$scope.runData.messageNumber=1;
		$scope.runData.simSpeedNumber=2000;
		$scope.runData.nodeRange=200;

		context.demo(tabIndex)
			.then(function() {
			});
	}

	$scope.runTest=function(tabIndex) {
		$scope.runData.isRunning=true;
		$scope.locks.GenerateButton=true;
		$scope.locks.RunButton=true;
		$scope.locks.DemoButtons=true;
		$scope.locks.Inputs=true;

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
				BatteryLevel: 1000,
				IsFinal: false,
				range: 200
			},
			{
				Id: $scope.newGuid(),
				FillColour: "#000000",
				BorderWidth: 2,
				StrokeColour: "#FF0000",
				Radius: 10,
				CenterX: 30,
				CenterY: 125,
				CanvasIndex: tabIndex,
				BatteryLevel: 1000,
				IsFinal: false,
				range: 200
			},
			{
				Id: $scope.newGuid(),
				FillColour: "#000000",
				BorderWidth: 2,
				StrokeColour: "#FF0000",
				Radius: 10,
				CenterX: 150,
				CenterY: 280,
				CanvasIndex: tabIndex,
				BatteryLevel: 1000,
				IsFinal: false,
				range: 200
			},
			{
				Id: $scope.newGuid(),
				FillColour: "#000000",
				BorderWidth: 2,
				StrokeColour: "#00FF00",
				Radius: 10,
				CenterX: 220,
				CenterY: 370,
				CanvasIndex: tabIndex,
				BatteryLevel: 1000,
				IsFinal: true,
				range: 200
			}
		];

		$scope.runData.nodeNumber=testNodeList.length;
		$scope.runData.pureSelfishNodeNumber=0;
		$scope.runData.partialSelfishNodeNumber=0;
		$scope.runData.messageNumber=1;
		$scope.runData.simSpeedNumber=200;
		$scope.runData.nodeRange=200;
		$scope.populateCanvas(testNodeList);

		$scope.drawMessageLine(testNodeList[0],testNodeList[1],"#FF0000");

		$timeout(function() {
			$scope.drawMessageLine(testNodeList[1],testNodeList[2],"#FF0000");
		},$scope.runData.simSpeedNumber);

		$timeout(function() {
			$scope.drawMessageLine(testNodeList[2],testNodeList[3],"#FF0000");
		},$scope.runData.simSpeedNumber*2);

		$timeout(function() {
			testNodeList[2].BatteryLevel=84;
			$scope.updateBatteryLevel(testNodeList[2]);

			$scope.runData.isRunning=false;
			$scope.locks.GenerateButton=false;
			$scope.locks.RunButton=true;
			$scope.locks.DemoButtons=false;
			$scope.locks.Inputs=false;
		},$scope.runData.simSpeedNumber*3);
	}

	$scope.addWarningNotification=function(warning) {
		$scope.notification.Warnings.Selected=0;
		$scope.notification.Warnings.push(warning);
	}

	$scope.addWarningNotifications=function() {
		$scope.addWarningNotification("LOL");
		$scope.addWarningNotification("HAHA");
		$scope.addWarningNotification("HAHA2");
		$scope.addWarningNotification("HAHA3");
	}

	$scope.changeWarningNotificationSelected=function() {
		if($scope.notification.Warnings.Selected<$scope.notification.Warnings.length-1) {
			$scope.notification.Warnings.Selected++;
		} else {
			$scope.notification.Warnings.Selected--;
		}
	}

	angular.element(document).ready(function() {
		$timeout(function() {
			$scope.runData.isRunning=false;

			$scope.locks.GenerateButton=false;
			$scope.locks.RunButton=true;
			$scope.locks.DemoButtons=false;
			$scope.locks.Inputs=false;

			$scope.notification.Warnings=[];
			$scope.runData.nodeNumber=0;
			$scope.runData.pureSelfishNodeNumber=0;
			$scope.runData.partialSelfishNodeNumber=0;
			$scope.runData.messageNumber=0;
			$scope.runData.simSpeedNumber=0;
			$scope.runData.nodeRange=200;

			for(var index=0;index<3;index++) {
				$scope.initAndAddCanvas(index);
			}
		},500);

		$scope.mainHub=$.connection.mainHub;
		$.connection.hub.start();

		$scope.mainHub.client.broadcastOutputMessage=function(outputMessageJson) {
			var outputMessage=angular.fromJson(outputMessageJson);

			$scope.pushOutputMessage(outputMessage.Tag,outputMessage.Message);
			$scope.$apply();
		};

		$scope.mainHub.client.sendMessageBetweenTwoNodes=function(nodeStartJson,nodeEndJson,lineColour) {
			var nodeStart=angular.fromJson(nodeStartJson);
			var nodeEnd=angular.fromJson(nodeEndJson);

			$scope.runData.isRunning=true;
			$scope.drawMessageLine(nodeStart,nodeEnd,lineColour);
			$scope.$apply();
		};

		$scope.mainHub.client.updateBatteryLevel=function(nodeJson) {
			var node=angular.fromJson(nodeJson);

			$scope.runData.isRunning=true;
			$scope.updateBatteryLevel(node);
			$scope.$apply();
		};

		$scope.mainHub.client.populateNodes=function(nodeListJson) {
			var nodeList=angular.fromJson(nodeListJson);

			$scope.populateCanvas(nodeList);
			$scope.$apply();
		};

		$scope.mainHub.client.completeRun=function(tabIndex) {
			$scope.runData.isRunning=false;

			jQuery("#latest-output").addClass("greenBorder");
			$timeout(function() {
				jQuery("#latest-output").removeClass("greenBorder");
			},2500);

			$scope.$apply();
		};
	});
}]);