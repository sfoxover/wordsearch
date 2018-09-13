﻿// on ready handler
$(document).ready(function () {
    try {
        tiles.signalNativeApp('ping');
    }
    catch (err) {
        console.log('Error in tiles.signalNativeApp, ' + err);
    }

    // handle high score save click
    $('#saveHighScoreButton').click(() => tiles.saveHighScoreClick());

    $('#highScoreModal').on('keypress', function (e) {
        if (e.keyCode === 13) {
            e.preventDefault();
            tiles.saveHighScoreClick();
        }
    });
});

$(window).resize(function () {
    try {
        tiles.resizeTiles();
    }
    catch (err) {
        console.log('Error in tiles.resize, ' + err);
    }
});

class Tiles {
    constructor() {
        this.rowCount = 0;
        this.columnCount = 0;
        this.waitForTilesCallbackCreation().then(function () {
            console.log('Native object loaded');
        }).catch(function (error) {
            console.log('Native object timed out');
        });
    }   

    // use promise to wait for C# window.jsBridge object to be created
    waitForTilesCallbackCreation() {
        try {
            return new Promise(function (resolve, reject) {
                var timerId = setTimeout(function () {
                    return reject(new Error("waitForTilesCallbackCreation timed out."));
                }, 5000);
                (function waitForTilesCallback() {
                    try {
                        if (window.jsBridge) {
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
            this.waitForTilesCallbackCreation().then(function () {
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

    // resize to fix html page
    resizeTiles() {
        try {
            var divBottomMarginValue = parseInt(getComputedStyle(document.documentElement, null).getPropertyValue('--div-margin-bottom'));
            var tileWidth = (window.innerWidth / this.rowCount);
            var tileHeight = ((window.innerHeight - divBottomMarginValue) / this.columnCount);
            // use min of height or width to ensure min tiles required
            var tileSize = Math.trunc(Math.min(tileWidth, tileHeight));
            var paddedSize = tileSize;
            // adjust for 2px border
            var borderSizeValue = parseInt(getComputedStyle(document.documentElement, null).getPropertyValue('--div-border-size'));
            if (borderSizeValue <= 0)
                borderSizeValue = 1;
            tileSize -= borderSizeValue * 2;
            $(".letterTDStyle").width(tileSize + 'px');
            $(".letterTDStyle").height(tileSize + 'px');

            tileSize -= borderSizeValue * 2;
            $(".letterDiv").width(tileSize + 'px');
            $(".letterDiv").height(tileSize + 'px');
            $(".letterDiv").css('line-height', tileSize + 'px');

            $(".letterDivSelected").width(tileSize + 'px');
            $(".letterDivSelected").height(tileSize + 'px');
            $(".letterDivSelected").css('line-height', tileSize + 'px');
            // size table
            var width = paddedSize * this.rowCount;
            var height = paddedSize * this.columnCount;
            $(".tilesTable").width(width + 'px');
            $(".tilesTable").height(height + 'px');     
            // use smaller font for large number of tiles
            if (this.rowCount >= 16) {
                $(".letterDiv").css('font-size', '1em');
                $(".letterDivSelected").css('font-size', '1em');
            }
            else if (this.rowCount >= 12) {
                $(".letterDiv").css('font-size', '1em');
                $(".letterDivSelected").css('font-size', '1em');
            }
            return tileSize;
        }
        catch (err) {
            this.handleError(err);
        }
    }

    // create word list table
    makeTable(container, items) {
        try {
            // store values in 2 dimential array
            var collection = new Array();
            for (var x = 0; x < items.length; x++) {
                var column = items[x];              
                collection[x] = new Array();
                for (var y = 0; y < column.length; y++) {
                    var tile = column[y];  
                    collection[x][y] = tile;                    
                }                
            }
            this.rowCount = collection.length;
            this.columnCount = collection[0].length;

            // create table elements
            var table = $("<table/>").addClass('tilesTable');           
            for (y = 0; y < this.columnCount; y++) {
                var tr = $("<tr/>");
                for (x = 0; x < this.rowCount; x++) {
                    var item = collection[x][y];
                    // create div
                    var div = $("<div/>");
                    if (item.LetterSelected)
                        div.addClass('letterDivSelected');
                    else
                        div.addClass('letterDiv');
                    div.text(item.Letter);
                    div.attr('id', 'tile_' + x + '_' + y);
                    var td = $("<td/>");
                    td.append(div);
                    td.addClass('letterTDStyle');
                    tr.append(td);
                    // add div click handler
                    var ClickHandler = {
                        value: null,
                        callback: function () {
                            tiles.handleTileClick(this);
                        }
                    };
                    div.click($.proxy(ClickHandler.callback, item));
                }
                table.append(tr);
            }     

            this.resizeTiles();            
            container.html(table);

            // Signal page is ready
            var timer = setTimeout(() => {
                clearTimeout(timer);
                tiles.signalNativeApp('tilePageReady');
            }, 1000);
        }
        catch(err) {
            this.handleError(err);
        }
    }

    // update tile clicked states
    updateTileStates(container, items) {
        try {
            $.each(items, function (x, rows) {
                $.each(rows, function (y, row) {
                    var div = '#tile_' + x + '_' + y;
                    if (row.LetterSelected)
                        $(div).attr("class", 'letterDivSelected');
                    else
                        $(div).attr("class", 'letterDiv');
                });
            }); 
        }
        catch (err) {
            this.handleError(err);
        }
    }

    // handle tile clicks
    handleTileClick(row) {
        this.signalNativeApp('tileClick', row);
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
                case "LoadTiles":
                    {
                        this.makeTable($("#tilesList"), data);
                        tiles.resizeTiles();
                        break;
                    }
                case "UpdateTileSelectedSates":
                    {
                        this.updateTileStates($("#tilesList"), data);
                        break;
                    }
                case "OnGameCompleted":
                    {
                        // load fireworks
                        fireworks.start(0, 100);
                        setTimeout(function () {
                            fireworks.pause();
                        }, 20000);
                        setTimeout(function () {
                            // show high score dialog
                            tiles.showHighScoreDialog(data);
                        }, 5000);
                        $('#FireworksCanvas').hide();
                        $('#FireworksCanvas').fadeIn(1000);                     
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

    showHighScoreDialog(data) {
        try {
            $('#labelRank').html('You are ranked <b>#' + data.Rank + '</b> for your score of ' + data.Score);
            $('#highScoreModal').modal();
            $('#highScoreModal').on('shown.bs.modal', ()=> {
                $('#hightscore-name').trigger('focus');
            });
        }
        catch (err) {
            this.handleError(err);
        }
    }

    // handle high score dialog click
    saveHighScoreClick() {
        try {
            var name = $('#hightscore-name').val();
            tiles.signalNativeApp('hightscoreName', name);
        }
        catch (err) {
            this.handleError(err);
        }
    }

}
// global object
var tiles = new Tiles();

