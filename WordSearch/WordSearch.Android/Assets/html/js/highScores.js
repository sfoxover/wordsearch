// on ready handler
$(document).ready(function () {    
    highScores.signalNativeApp('ping');

    // close window
    $('#closeButton').click(() => highScores.closeWindow());

    // clear Scores
    $('#clearScores').click(() => highScores.clearScores());
});

class HighScores {
    constructor() {
        this.waitForCallbackCreation().then(function () {
            console.log('Native object loaded');
        }).catch(function (error) {
            console.log('Loading mock data');
        });
    }   

    // use promise to wait for C# window.jsBridge object to be created
    waitForCallbackCreation() {
        try {
            return new Promise(function (resolve, reject) {
                var timerId = setTimeout(function () {
                    return reject(new Error("waitForCallbackCreation timed out."));
                }, 3000);
                (function waitForCallback() {
                    try {
                        if (window.jsBridge) {
                            clearTimeout(timerId);
                            return resolve();
                        }
                    }
                    catch (err) {
                        err;
                    }
                    setTimeout(waitForCallback, 100);
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
            this.waitForCallbackCreation().then(function () {
                // format error message into json
                var msgObj = new Object();
                msgObj.Message = "Error";
                msgObj.Data = err.message;
                var json = JSON.stringify(msgObj);
                // call native code
                jsBridge.invokeAction(json);
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
            this.waitForCallbackCreation().then(function () {
                // format error message into json
                var msgObj = new Object();
                msgObj.Message = "LogMsg";
                msgObj.Data = info;
                var json = JSON.stringify(msgObj);
                // call native code
                jsBridge.invokeAction(json);
            }).catch(function (error) {
                console.log(error);
            });
        }
        catch (err) {
            console.error(err);
        }
    }    

    // create word list table
    makeTable(container, items) {
        try {
            // create table elements
            var table = $('#highScoreTable');   
            var body = $('<tbody/>');
            var rank = 1;
            $.each(items, function (rowCount, item) {
                var row = $("<tr/>");
                var name = item.Name;
                var score = item.Points;
                var date = item.Date;
                row.append($("<td/>").text(rank).addClass('text-center'));
                row.append($("<td/>").text(name).addClass('text-center'));
                row.append($("<td/>").text(score).addClass('text-center'));
                row.append($("<td/>").text(date).addClass('text-center'));
                body.append(row);
                rank++;
            });
            $("#highScoreTable tbody").remove();
            table.append(body);
        }
        catch(err) {
            this.handleError(err);
        }
    }   

    // pass message and data to native app
    signalNativeApp(msg, data) {
        try {
            this.waitForCallbackCreation().then(function () {
                // format message into json
                var msgObj = new Object();
                msgObj.Message = msg;
                msgObj.Data = data;
                var json = JSON.stringify(msgObj);
                // call native code
                jsBridge.invokeAction(json);
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
                case "LoadHighScores":
                    {
                        this.makeTable($("#highScores"), data);
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

    // close window
    closeWindow() {
        try {
            highScores.signalNativeApp('closeWindow', name);
        }
        catch (err) {
            this.handleError(err);
        }
    }

    // clear high scores
    clearScores() {
        try {
            highScores.signalNativeApp('clearScores');
        }
        catch (err) {
            this.handleError(err);
        }
    }
}

// global object
var highScores = new HighScores();

