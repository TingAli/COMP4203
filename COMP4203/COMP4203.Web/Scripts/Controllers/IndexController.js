app.controller("indexController",["$scope","dataService","$window","$timeout","$filter","$uibModal","$log","$document",
	function($scope,context,$window,$timeout,$filter,$uibModal,$log,$document) {
		// Objects used throughout file
		$scope.runData={};
		$scope.locks={};
		$scope.outputMessages=[];
		$scope.canvasList=[];
		$scope.graphs=[];

		// Chart configuration
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

		// Chart configuration
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

		// Chart configuration
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

		// Chart configuration
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

		// Run function to initiate run in the backend
		$scope.initiateRun=function(tabIndex) {
			// Locking mechanism locking everything whilst running
			$scope.runData.isRunning=true;
			$scope.locks.RunButton=true;
			$scope.locks.DemoButtons=true;
			$scope.locks.Inputs=true;

			// Forces execution number to be 1 when execution mode is off
			if(!$scope.runData.IsExecutionMode) {
				$scope.runData.executionNumber=1;
			}

			// Ajax call to call the run function in the backend
			context.run($scope.runData.nodeNumber,$scope.runData.messageNumber,$scope.runData.simSpeedNumber,$scope.runData.nodeRange,
				$scope.runData.pureSelfishNodeNumber,$scope.runData.partialSelfishNodeNumber,$scope.runData.executionNumber,tabIndex)
				.then(function(response) {
					// Assigns returned list of graphs to the graphs object
					$scope.graphs=response.data;

					// Timeout used to allow time for processing data
					$timeout(function() {
						// Looping through list of graphs and then updating relevant graphs in the graphs tab
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
					},800); // 800ms delay
				});
		}

		// Function to draw one node to its canvas
		$scope.drawNode=function(node) {
			var canvasCtx=$scope.canvasList[node.CanvasIndex];

			// Draws node to canvas
			canvasCtx.beginPath();
			canvasCtx.arc(node.CenterX,node.CenterY,node.Radius,0,2*Math.PI,false);
			canvasCtx.fillStyle=node.FillColour;
			canvasCtx.fill();
			canvasCtx.lineWidth=node.BorderWidth;
			canvasCtx.setLineDash([0]);
			canvasCtx.strokeStyle=node.StrokeColour;
			canvasCtx.stroke();

			// Draws node range
			$scope.drawNodeRange(node);
			// Draws battery level
			$scope.drawBatteryLevel(node);
		}

		// Draws node range for node onto its respective canvas
		$scope.drawNodeRange=function(node) {
			var canvasCtx=$scope.canvasList[node.CanvasIndex];
			// Adds node's range as a history object
			var nodeRangeHistoryObject={
				Id: node.Id,
				X: node.CenterX,
				Y: node.CenterY,
				IsVisible: true
			};

			// Draws node range
			canvasCtx.beginPath();
			canvasCtx.setLineDash([5]);
			canvasCtx.arc(node.CenterX,node.CenterY,$scope.runData.nodeRange,0,2*Math.PI,false);
			canvasCtx.strokeStyle="#D3D3D3";
			canvasCtx.stroke();

			canvasCtx.NodeRangeHistory.push(nodeRangeHistoryObject);
		}

		// Draws transmission line between two nodes
		$scope.drawMessageLine=function(nodeStart,nodeEnd,strokeColour,isRedraw) {
			var canvasCtx=$scope.canvasList[nodeStart.CanvasIndex];
			// Arrow head length
			var headlen=10;
			// Arrow angle
			var angle=Math.atan2(nodeEnd.CenterY-nodeStart.CenterY,nodeEnd.CenterX-nodeStart.CenterX);
			var lineHistoryObject={
				NodeStart: nodeStart,
				NodeEnd: nodeEnd,
				StrokeColour: strokeColour,
				IsVisible: true
			};

			// Sets all message lines to false to allow the new arrow to be drawn and visible alone
			$scope.setMessageLinesToFalse(nodeStart.CanvasIndex);

			// Draws message line
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

			// Redraws current state if redraw is false
			if(!isRedraw) {
				$scope.reDrawCurrentState(nodeStart.CanvasIndex);
			}
		}

		// Sets message lines to false
		$scope.setMessageLinesToFalse=function(canvasIndex) {
			var canvasCtx=$scope.canvasList[canvasIndex];

			// All line history objects set to false to not be redrawn
			for(var lineIndex=0;lineIndex<canvasCtx.LineHistory.length;lineIndex++) {
				canvasCtx.LineHistory[lineIndex].IsVisible=false;
			}
		}

		// Draws battery level
		$scope.drawBatteryLevel=function(node) {
			var canvasCtx=$scope.canvasList[node.CanvasIndex];
			// Assigns battery level location on canvas
			var batteryLevelX=node.CenterX+(node.Radius*1.15);
			var batteryLevelY=node.CenterY-(node.Radius*1.15);
			var batteryLevelTextHistoryObject={
				Id: node.Id,
				X: batteryLevelX,
				Y: batteryLevelY,
				IsVisible: true
			};

			// Draws battery level
			canvasCtx.beginPath();
			canvasCtx.font="12px Arial";
			canvasCtx.fillStyle="red";
			canvasCtx.fillText(node.BatteryLevel,batteryLevelX,batteryLevelY);
			canvasCtx.setLineDash([0]);
			canvasCtx.stroke();

			canvasCtx.BatteryLevelTextHistory.push(batteryLevelTextHistoryObject);
		}

		// Updates an existing node to a new battery level on the UI
		$scope.updateBatteryLevel=function(node) {
			// Searches for node in canvas using history
			var foundHistory=$filter("filter")($scope.canvasList[node.CanvasIndex].BatteryLevelTextHistory,{ Id: node.Id },true);
			var found=$filter("filter")($scope.canvasList[node.CanvasIndex].Nodes,{ Id: node.Id },true);
			if(found.length&&foundHistory.length) {
				// Sets that battery level history to not be visible later
				foundHistory[0].IsVisible=false;
				found[0].BatteryLevel=parseFloat(Math.round(node.BatteryLevel*100)/100).toFixed(2);
			}

			// Redraws
			$scope.reDrawCurrentState(node.CanvasIndex);
		}

		// Redraws canvas state
		$scope.reDrawCurrentState=function(canvasIndex) {
			var canvasCtx=$scope.canvasList[canvasIndex];

			// Clears canvas
			canvasCtx.clearRect(0,0,500,500);

			// Draws nodes again
			for(var nodeIndex=0;nodeIndex<canvasCtx.Nodes.length;nodeIndex++) {
				$scope.drawNode(canvasCtx.Nodes[nodeIndex]);
			}

			// Draws message line
			$scope.drawMessageLine(
				canvasCtx.LineHistory[canvasCtx.LineHistory.length-1].NodeStart,
				canvasCtx.LineHistory[canvasCtx.LineHistory.length-1].NodeEnd,
				canvasCtx.LineHistory[canvasCtx.LineHistory.length-1].StrokeColour,true);
		}

		// Populates canvas
		$scope.populateCanvas=function(nodeList) {
			// Loop through nodes to be drawn
			for(var index=0;index<nodeList.length;index++) {
				var canvasCtx=$scope.canvasList[nodeList[index].CanvasIndex];

				// Draws node
				$scope.drawNode(nodeList[index]);
				canvasCtx.Nodes.push(nodeList[index]);
			}
		}

		// Initialises canvases with extra properties
		$scope.initAndAddCanvas=function(index) {
			var canvas=document.getElementById("canvas_"+index);
			var ctx=canvas.getContext("2d");
			ctx.Nodes=[];
			ctx.LineHistory=[];
			ctx.BatteryLevelTextHistory=[];
			ctx.NodeRangeHistory=[];

			$scope.canvasList.push(ctx);
		}

		// Allows frontend creation of Guids
		$scope.newGuid=function() {
			return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g,function(c) {
				var r=Math.random()*16|0,v=c==="x"? r:(r&0x3|0x8);
				return v.toString(16);
			});
		}

		// Resets everything back to default state
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

		// Resets the canvas specified
		$scope.resetCanvas=function(tabIndex) {
			$scope.canvasList[tabIndex].clearRect(0,0,500,500);
			$scope.canvasList[tabIndex].Nodes=[];
			$scope.canvasList[tabIndex].LineHistory=[];
			$scope.canvasList[tabIndex].BatteryLevelTextHistory=[];
			$scope.canvasList[tabIndex].NodeRangeHistory=[];
		}

		// Pushes a message to the output pane
		$scope.pushOutputMessage=function(tag,message) {
			var outputMessage={
				Id: $scope.newGuid(),
				Tag: tag,
				Message: message
			};
			$scope.outputMessages.push(outputMessage);
		}

		// Runs a test to show the canvas UI is working with nodes and arrows etc
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

			// Simulated message transmission on frontend solely for testing purposes of canvas on browser
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

		// Called when all elements are loaded and assigns default variables
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

			// Starts SignalR MainHub
			$scope.mainHub=$.connection.mainHub;
			$.connection.hub.start();

			// SignalR functions that the backend can call to modify the frontend UI etc
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