// on ready handler
$(document).ready(function () {    

    tiles.signalNativeApp("LoadTiles");

});

class Tiles {
    constructor() {
        this.waitForTilesCallbackCreation().then(function () {
            console.log('Native object loaded');
        }).catch(function (error) {
            console.log('Loading mock data');
            tiles.loadMockData();
        });
    }   

    // use promise to wait for C# tilesJSCallback object to be created
    waitForTilesCallbackCreation() {
        try {
            return new Promise(function (resolve, reject) {
                var timerId = setTimeout(function () {
                    return reject(new Error("waitForTilesCallbackCreation timed out."));
                }, 3000);
                (function waitForTilesCallback() {
                    try {
                        if (tilesJSCallback) {
                            clearTimeout(timerId);
                            return resolve();
                        }
                    }
                    catch (err) {
                        err;
                    }
                    setTimeout(waitForTilesCallback, 100);
                })();
            });
        }
        catch (err) {
            this.handleError(err);
        }
    }

    // handle errors
    handleError(err) {
        try {
            this.waitForTilesCallbackCreation().then(function () {
                // format error message into json
                var msgObj = new Object();
                msgObj.Message = "Error";
                msgObj.Data = err;
                var json = JSON.stringify(msgObj);
                // call native code
                tilesJSCallback(json);
            }).catch(function (error) {
                console.log(error);
            });
        }
        catch (err) {
            console.error(err);
        }
    }

    // log message to native app
    logMsg(info) {
        try {
            this.waitForTilesCallbackCreation().then(function () {
                // format error message into json
                var msgObj = new Object();
                msgObj.Message = "LogMsg";
                msgObj.Data = info;
                var json = JSON.stringify(msgObj);
                // call native code
                tilesJSCallback(json);
            }).catch(function (error) {
                console.log(error);
            });
        }
        catch (err) {
            console.error(err);
        }
    }    

    // load mok data for browser testing
    loadMockData() {
        try {
            var data = JSON.parse('[{ "Text": "friday", "Row": 3, "Column": 2, "IsWordCompleted": false }, { "Text": "july", "Row": 8, "Column": 4, "IsWordCompleted": false }, { "Text": "car", "Row": 4, "Column": 6, "IsWordCompleted": false }, { "Text": "africa", "Row": 0, "Column": 7, "IsWordCompleted": false }]');
            this.makeTable($("#tilesList"), data);

            // test fireworks
            setTimeout(function () {
                fireworks.start(0, 100);
                setTimeout(function () {
                    fireworks.pause();
                    }, 5000);
            }, 2000);
        }
        catch(err) {
            this.handleError(err);
        }
    }

    // create word list table
    makeTable(container, items) {
        try {
            var table = $("<table/>").addClass('tilesTable');
            $.each(items, function (rowsCount, rows) {
                var tr = $("<tr/>");
                $.each(rows, function (rowCount, row) {
                    var div = $("<div/>");
                    if (row.LetterSelected)
                        div.addClass('letterDivSelected');
                    else
                        div.addClass('letterDiv');
                    div.text(row.Letter);
                    var td = $("<td/>");
                    td.append(div);
                    tr.append(td);
                });
                table.append(tr);
            });
            return container.html(table);
        }
        catch(err) {
            this.handleError(err);
        }
    }

    // pass message and data to native app
    signalNativeApp(msg, data) {
        try {
            this.waitForTilesCallbackCreation().then(function () {
                // format message into json
                var msgObj = new Object();
                msgObj.Message = msg;
                msgObj.Data = data;
                var json = JSON.stringify(msgObj);
                // call native code
                tilesJSCallback(json);
            }).catch(function (error) {
                console.log(error);
            });
        }
        catch (err) {
            this.handleError(err);
        }
    }

    // handle message from native app
    handleMsgFromApp(json) {
        try {
            var msgObj = JSON.parse(json);
            var msg = msgObj.Message;
            var data = msgObj.Data;
            switch (msg) {
                case "LoadTiles":
                    {
                        this.makeTable($("#tilesList"), data);
                        break;
                    }
                default:
                    {
                        this.handleError("Unknown message from app, " + msg);
                        break;
                    }
            }
        }
        catch (err) {
            this.handleError(err);
        }
    }

}
// global object
var tiles = new Tiles();

