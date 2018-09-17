// on ready handler
$(document).ready(function () {
    // Ping ready message
    highScores.signalNativeApp('ping');
    // close window
    $('#closeButton').click(() => highScores.closeWindow());
    // clear Scores
    $('#clearScores').click(() => highScores.clearScores());
});

class HighScores extends Signal {
    constructor() {
        super();
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
            this.logError(err);
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
                        this.logError("Unknown message from app, " + msg);
                        break;
                    }
            }
        }
        catch (err) {
            this.logError(err);
        }
    }

    // close window
    closeWindow() {
        try {
            highScores.signalNativeApp('closeWindow', name);
        }
        catch (err) {
            this.logError(err);
        }
    }

    // clear high scores
    clearScores() {
        try {
            highScores.signalNativeApp('clearScores');
        }
        catch (err) {
            this.logError(err);
        }
    }
}

// global object
var highScores = new HighScores();

