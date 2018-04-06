app.controller("indexController",["$scope","dataService","$window","$timeout","$filter","$uibModal","$log","$document",
	function($scope,context,$window,$timeout,$filter,$uibModal,$log,$document) {
		$scope.runData={};
		$scope.locks={};
		$scope.outputMessages=[];
		$scope.canvasList=[];
		$scope.graphs=[];

		$scope.chartAEEDConfig={
			chart: {
				type: "line"
			},
			series: [{
				data: [],
				name: "DSR"
			},
			{
				data: [],
				name: "SA-DSR"
			},
			{
				data: [],
				name: "MSA-DSR"
			}],
			title: {
				text: "AEED"
			},
			xAxis: {
				data: [],
				title: {
					text: "Number Of Executions"
				}
			},
			yAxis: {
				title: {
					text: "AEED Value"
				}
			},
			average: {
				dsr: 0,
				sadsr: 0,
				msadsr: 0
			}
		};

		$scope.chartNROConfig={
			chart: {
				type: "line"
			},
			series: [{
				data: [],
				name: "DSR"
			},
			{
				data: [],
				name: "SA-DSR"
			},
			{
				data: [],
				name: "MSA-DSR"
			}],
			title: {
				text: "NRO"
			},
			xAxis: {
				data: [],
				title: {
					text: "Number Of Executions"
				}
			},
			yAxis: {
				title: {
					text: "NRO Value"
				}
			},
			average: {
				dsr: 0,
				sadsr: 0,
				msadsr: 0
			}
		};

		$scope.chartBDDConfig={
			chart: {
				type: "line"
			},
			series: [{
				data: [],
				name: "DSR"
			},
			{
				data: [],
				name: "SA-DSR"
			},
			{
				data: [],
				name: "MSA-DSR"
			}],
			title: {
				text: "BDD"
			},
			xAxis: {
				data: [],
				title: {
					text: "Number Of Executions"
				}
			},
			yAxis: {
				title: {
					text: "BDD Value"
				}
			},
			average: {
				dsr: 0,
				sadsr: 0,
				msadsr: 0
			}
		};

		$scope.chartPDRConfig={
			chart: {
				type: "line"
			},
			series: [{
				data: [],
				name: "DSR"
			},
			{
				data: [],
				name: "SA-DSR"
			},
			{
				data: [],
				name: "MSA-DSR"
			}],
			title: {
				text: "PDR"
			},
			xAxis: {
				data: [],
				title: {
					text: "Number Of Executions"
				}
			},
			yAxis: {
				title: {
					text: "PDR Value"
				}
			},
			average: {
				dsr: 0,
				sadsr: 0,
				msadsr: 0
			}
		};

		$scope.initiateRun=function(tabIndex) {
			$scope.runData.isRunning=true;
			$scope.locks.RunButton=true;
			$scope.locks.DemoButtons=true;
			$scope.locks.Inputs=true;

			if(!$scope.runData.IsExecutionMode) {
				$scope.runData.executionNumber=1;
			}

			context.run($scope.runData.nodeNumber,$scope.runData.messageNumber,$scope.runData.simSpeedNumber,$scope.runData.nodeRange,
				$scope.runData.pureSelfishNodeNumber,$scope.runData.partialSelfishNodeNumber,$scope.runData.executionNumber,tabIndex)
				.then(function(response) {
					$scope.graphs=response.data;

					$timeout(function() {
						for(var index=0;index<$scope.graphs[0].XAxisExecutionsNumber.length;index++) {
							$scope.chartAEEDConfig.xAxis.data.push($scope.graphs[0].XAxisExecutionsNumber[index]);
							$scope.chartAEEDConfig.series[0].data.push($scope.graphs[0].YAxisValuesDsr[index]);
							$scope.chartAEEDConfig.series[1].data.push($scope.graphs[0].YAxisValuesSadsr[index]);
							$scope.chartAEEDConfig.series[2].data.push($scope.graphs[0].YAxisValuesMsadsr[index]);
							$scope.chartAEEDConfig.average.dsr=$scope.graphs[0].AverageDsr;
							$scope.chartAEEDConfig.average.sadsr=$scope.graphs[0].AverageSadsr;
							$scope.chartAEEDConfig.average.msadsr=$scope.graphs[0].AverageMsadsr;

							$scope.chartNROConfig.xAxis.data.push($scope.graphs[1].XAxisExecutionsNumber[index]);
							$scope.chartNROConfig.series[0].data.push($scope.graphs[1].YAxisValuesDsr[index]);
							$scope.chartNROConfig.series[1].data.push($scope.graphs[1].YAxisValuesSadsr[index]);
							$scope.chartNROConfig.series[2].data.push($scope.graphs[1].YAxisValuesMsadsr[index]);
							$scope.chartNROConfig.average.dsr=$scope.graphs[1].AverageDsr;
							$scope.chartNROConfig.average.sadsr=$scope.graphs[1].AverageSadsr;
							$scope.chartNROConfig.average.msadsr=$scope.graphs[1].AverageMsadsr;

							$scope.chartBDDConfig.xAxis.data.push($scope.graphs[2].XAxisExecutionsNumber[index]);
							$scope.chartBDDConfig.series[0].data.push($scope.graphs[2].YAxisValuesDsr[index]);
							$scope.chartBDDConfig.series[1].data.push($scope.graphs[2].YAxisValuesSadsr[index]);
							$scope.chartBDDConfig.series[2].data.push($scope.graphs[2].YAxisValuesMsadsr[index]);
							$scope.chartBDDConfig.average.dsr=$scope.graphs[2].AverageDsr;
							$scope.chartBDDConfig.average.sadsr=$scope.graphs[2].AverageSadsr;
							$scope.chartBDDConfig.average.msadsr=$scope.graphs[2].AverageMsadsr;

							$scope.chartPDRConfig.xAxis.data.push($scope.graphs[3].XAxisExecutionsNumber[index]);
							$scope.chartPDRConfig.series[0].data.push($scope.graphs[3].YAxisValuesDsr[index]);
							$scope.chartPDRConfig.series[1].data.push($scope.graphs[3].YAxisValuesSadsr[index]);
							$scope.chartPDRConfig.series[2].data.push($scope.graphs[3].YAxisValuesMsadsr[index]);
							$scope.chartPDRConfig.average.dsr=$scope.graphs[3].AverageDsr;
							$scope.chartPDRConfig.average.sadsr=$scope.graphs[3].AverageSadsr;
							$scope.chartPDRConfig.average.msadsr=$scope.graphs[3].AverageMsadsr;
						}
					},800);
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
					$scope.runData.IsExecutionMode=false;
					$scope.locks.RunButton=false;
					$scope.locks.DemoButtons=false;
					$scope.locks.Inputs=false;
					for(var index=0;index<$scope.canvasList.length;index++) {
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
					$scope.runData.executionNumber=1;
				});
		}

		$scope.resetCanvas=function(tabIndex) {
			$scope.canvasList[tabIndex].clearRect(0,0,500,500);
			$scope.canvasList[tabIndex].Nodes=[];
			$scope.canvasList[tabIndex].LineHistory=[];
			$scope.canvasList[tabIndex].BatteryLevelTextHistory=[];
			$scope.canvasList[tabIndex].NodeRangeHistory=[];
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
			$scope.runData.isRunning=true;
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
			},$scope.runData.simSpeedNumber*3);
		}

		angular.element(document).ready(function() {
			$timeout(function() {
				$scope.runData.isRunning=false;
				$scope.runData.IsExecutionMode=false;

				$scope.locks.RunButton=false;
				$scope.locks.DemoButtons=false;
				$scope.locks.Inputs=false;

				$scope.runData.nodeNumber=0;
				$scope.runData.pureSelfishNodeNumber=0;
				$scope.runData.partialSelfishNodeNumber=0;
				$scope.runData.messageNumber=0;
				$scope.runData.simSpeedNumber=0;
				$scope.runData.nodeRange=200;
				$scope.runData.executionNumber=1;

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

			$scope.mainHub.client.resetCanvas=function(tabIndex) {
				$scope.resetCanvas(tabIndex);
				$scope.$apply();
			};
		});
	}]);