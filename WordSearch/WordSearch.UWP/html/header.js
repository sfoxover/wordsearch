// on ready handler
$(document).ready(function () {    

    header.signalNativeApp("OnReady");

});

class Header {
    constructor() {
        this.waitForHeaderCallbackCreation().then(function () {
            console.log('Native object loaded');
        }).catch(function (error) {
            console.log('Loading mock data');
            header.loadMockData();
        });
    }   

    // use promise to wait for C# headerJSCallback object to be created
    waitForHeaderCallbackCreation() {
        try {
            return new Promise(function (resolve, reject) {
                setTimeout(function () {
                    return reject(new Error("waitForHeaderCallbackCreation timed out."));
                }, 1000);
                (function waitForHeaderCallback() {
                    try {
                        if (headerJSCallback) {
                            return resolve();
                        }
                    }
                    catch (err) {
                        err;
                    }
                    setTimeout(waitForHeaderCallback, 100);
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
            this.waitForHeaderCallbackCreation().then(function () {
                // format error message into json
                var msgObj = new Object();
                msgObj.Message = "Error";
                msgObj.Data = err;
                var json = JSON.stringify(msgObj);
                // call native code
                headerJSCallback(json);
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
            this.waitForHeaderCallbackCreation().then(function () {
                // format error message into json
                var msgObj = new Object();
                msgObj.Message = "LogMsg";
                msgObj.Data = info;
                var json = JSON.stringify(msgObj);
                // call native code
                headerJSCallback(json);
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
            this.makeTable($("#wordsList"), data);

            // test fireworks
            setTimeout(function () {
                fireworks.start(50, 25);
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
            var table = $("<table/>").addClass('headerTable');
            var row = $("<tr/>");
            $.each(items, function (rowCount, item) {
                var word = item.Text;
                var wordDiv = $("<div/>");
                if (item.IsWordCompleted)
                    wordDiv.addClass('wordCompleteDiv');
                else
                    wordDiv.addClass('wordDiv');
                wordDiv.text(word);
                var td = $("<td/>");
                td.append(wordDiv);
                row.append(td);
            });
            table.append(row);
            return container.html(table);
        }
        catch(err) {
            this.handleError(err);
        }
    }

    // pass message and data to native app
    signalNativeApp(msg, data) {
        try {
            this.waitForHeaderCallbackCreation().then(function () {
                // format message into json
                var msgObj = new Object();
                msgObj.Message = msg;
                msgObj.Data = data;
                var json = JSON.stringify(msgObj);
                // call native code
                headerJSCallback(json);
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
                case "LoadWordsHeader":
                    {
                        this.makeTable($("#wordsList"), data);
                        break;
                    }
                case "OnUpdateScore":
                    {
                        $("#score").text(data);
                        break;
                    }
                case "OnUpdateTime":
                    {
                        $("#timeClock").text(data);
                        break;
                    }
                case "OnWordComplete":
                    {
                        // show fireworks
                        fireworks.start(data.Start, data.End);
                        setTimeout(function () {
                            fireworks.pause();
                        }, 5000);
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
// global Header object
var header = new Header();

