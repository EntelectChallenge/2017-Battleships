angular.module("ReplayZ", [])
    .controller("ReplayZController", [
        "$q", "$http", "$interval", function ($q, $http, $interval) {
            // Variables
            var scope = this;
            var urlRoot = "api/";

            // Scope Variables
            scope.name = "Entelect ReplayZ";
            scope.selectedReplayItem = "";
            scope.selectedRoundItem = "";
            scope.simulation = 0;

            scope.showFiles = true;
            scope.showReplays = true;

            scope.defaultPlayerProperties = {
                place: "",
                command: "",
                log: "",
                map: "",
                state: "",
                showShips: true
            };

            scope.playerA = angular.copy(scope.defaultPlayerProperties);

            scope.playerB = angular.copy(scope.defaultPlayerProperties);

            scope.summary = {
                engine: "",
                roundInfo: "",
                state: ""
            };

            list("").then(
                function (result) {
                    scope.replayList = result;
                });

            // Functions

            function list(folder) {
                var deferred = $q.defer();
                $http.get(urlRoot + "File/List/" + folder).then(
                    function (result) {
                        deferred.resolve(result.data);
                    },
                    function (error) {
                        deferred.reject(error.data.message);
                    }
                );
                return deferred.promise;
            };

            function read (file) {
                var deferred = $q.defer();
                $http.get(urlRoot + "File/Read/" + file).then(
                    function (result) {
                        deferred.resolve(result.data);
                    },
                    function (error) {
                        deferred.reject(error.data.message);
                    }
                );
                return deferred.promise;
            };

            function updatePlayerA() {
                var location = scope.selectedReplayItem + "/" + scope.selectedRoundItem + "/";
                read(location + "A/place.txt").then(
                    function (result) {
                        scope.playerA.place = result;
                    });
                read(location + "A/command.txt").then(
                    function (result) {
                        scope.playerA.command = result;
                    });
                read(location + "A/log.txt").then(
                    function (result) {
                        scope.playerA.log = result;
                    });
                read(location + "A - map.txt").then(
                    function (result) {
                        scope.playerA.map = result;
                    });
                read(location + "A/state.json").then(
                    function (result) {
                        scope.playerA.state = result;
                    });
            };

            function updatePlayerB() {
                var location = scope.selectedReplayItem + "/" + scope.selectedRoundItem + "/";
                read(location + "B/place.txt").then(
                    function (result) {
                        scope.playerB.place = result;
                    });
                read(location + "B/command.txt").then(
                    function (result) {
                        scope.playerB.command = result;
                    });
                read(location + "B/log.txt").then(
                    function (result) {
                        scope.playerB.log = result;
                    });
                read(location + "B - map.txt").then(
                    function (result) {
                        scope.playerB.map = result;
                    });
                read(location + "B/state.json").then(
                    function (result) {
                        scope.playerB.state = result;
                    });
            };

            function updateSummary() {
                var location = scope.selectedReplayItem + "/" + scope.selectedRoundItem + "/";
                read(location + "engine.log").then(
                    function (result) {
                        scope.summary.engine = result;
                    });
                read(location + "roundInfo.json").then(
                    function (result) {
                        scope.summary.roundInfo = result;
                    });
                read(location + "state.json").then(
                    function (result) {
                        scope.summary.state = result;
                        getPlayerPlaceBoard("playerAships", result.MapDimension, result.Player1Map, scope.playerA.showShips);
                        getPlayerPlaceBoard("playerBships", result.MapDimension, result.Player2Map, scope.playerB.showShips);
                        read(location + "A/command.txt").then(
                            function (commandA) {
                                showCommand("playerBships", result.MapDimension, commandA);
                            });
                        read(location + "B/command.txt").then(
                            function (commandB) {
                                showCommand("playerAships", result.MapDimension, commandB);
                            });
                    });
            };

            // Scope Functions

            scope.selectReplay = function (replay) {
                scope.selectedReplayItem = replay;
                list(scope.selectedReplayItem).then(
                    function (result) {
                        scope.roundsList = result;
                        scope.selectRound(scope.roundsList[0].folderName);
                    });
            };

            scope.selectRound = function (round) {
                scope.selectedRoundItem = round;
                updatePlayerA();
                updatePlayerB();
                updateSummary();
            };

            // Battle Ships
            function getPlayerPlaceBoard(board, mapDimension, player, showShips) {

                var sizeOfMap = Math.ceil((50 / mapDimension) * 10);
                var canvas = document.getElementById(board);
                var ctx = canvas.getContext("2d");
                ctx.clearRect(0, 0, canvas.width, canvas.height);

                var colorShip = "rgba(0,0,0,1)";
                var colorWater = "rgba(100,100,255,1)";
                var colorMiss = "rgba(150,150,255,1)";
                var colorHit = "rgba(255,0,0,1)";

                function drawField(x, y, color) {
                    if (x >= 1 && x <= mapDimension && y <= mapDimension && y >= 1) {
                        var ctx = canvas.getContext("2d");
                        ctx.fillStyle = color;
                        ctx.fillRect(fieldToCoords(x, y)[0], fieldToCoords(x, y)[1], sizeOfMap-2, sizeOfMap-2);
                    }
                    return [x, y];
                }

                function drawPlayGround() {
                    if (canvas.getContext) {
                        for (var cell = 0; cell < player.Cells.length; cell++) {
                            var c = player.Cells[cell];
                            drawField(c.X + 1, c.Y + 1, colorWater);
                            if (c.Occupied && showShips) {
                                drawField(c.X + 1, c.Y + 1, colorShip);
                            }
                            if (c.Hit) {
                                drawField(c.X + 1, c.Y + 1, colorMiss);
                            }
                            if (c.Occupied && c.Hit) {
                                drawField(c.X + 1, c.Y + 1, colorHit);
                            }
                        }
                    }
                }

                function fieldToCoords(x, y) {
                    x = (x * sizeOfMap) - sizeOfMap + 1;
                    y = (y * sizeOfMap) + 1 - sizeOfMap;
                    return [x, y];
                }

                drawPlayGround();
            };

            function showCommand(board, mapDimension, command) {

                var sizeOfMap = Math.ceil((50 / mapDimension) * 10);
                var canvas = document.getElementById(board);

                var split = command.split(',');
                var colorHighLight = "rgba(0,255,0,0.4)";

                function drawField(x, y, color) {
                    if (x >= 1 && x <= mapDimension && y <= mapDimension && y >= 1) {
                        var ctx = canvas.getContext("2d");
                        ctx.fillStyle = color;
                        ctx.fillRect(fieldToCoords(x, y)[0], fieldToCoords(x, y)[1], sizeOfMap - 2, sizeOfMap - 2);
                    }
                    return [x, y];
                }

                function highlightField(x, y) {
                    drawField(x, y, colorHighLight);
                }

                function fieldToCoords(x, y) {
                    x = (x * sizeOfMap) - sizeOfMap + 1;
                    y = (y * sizeOfMap) + 1 - sizeOfMap;
                    return [x, y];
                }

                highlightField(parseInt(split[1]) + 1, parseInt(split[2]) + 1);
            };

            scope.prevRound = function () {
                for (var i = 0; i < scope.roundsList.length; i++) {
                    if (scope.roundsList.length === i) {
                        return 0;
                    }
                    if (scope.roundsList[i + 1].folderName === scope.selectedRoundItem) {
                        scope.selectRound(scope.roundsList[i].folderName);
                        return 1;
                    }
                }
                return 0;
            };

            scope.nextRound = function () {
                for (var i = 0; i < scope.roundsList.length; i++) {
                    if (i === 0) {
                        continue;
                    }
                    if (scope.roundsList[i - 1].folderName === scope.selectedRoundItem) {
                        scope.selectRound(scope.roundsList[i].folderName);
                        return 1;
                    }
                }
                return 0;
            };

            var simulation;

            scope.simulate = function (seconds) {
                scope.simulation = seconds;
                if (seconds === 0) {
                    scope.stopSimulation();
                } else {
                    if (angular.isDefined(simulation)) scope.stopSimulation();
                    simulation = $interval(function () {
                        var result = scope.nextRound();
                        if (result === 0) {
                            scope.simulate(0);
                        }
                    }, seconds * 1000);
                }
            };

            scope.stopSimulation = function () {
                if (angular.isDefined(simulation)) {
                    $interval.cancel(simulation);
                    simulation = undefined;
                }
            };
        }
    ]);