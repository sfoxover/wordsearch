// on ready handler
$(document).ready(function () {    
    tiles.signalNativeApp('ping');

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
    tiles.resizeTiles();
});

class Tiles extends Signal {
    constructor() {
        super();
        this.rowCount = 0;
        this.columnCount = 0;
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
            if (tileSize < 16) {
                $(".letterDiv").css('font-size', '1em');
                $(".letterDivSelected").css('font-size', '1em');
            }
            else {
                $(".letterDiv").css('font-size', '1.5em');
                $(".letterDivSelected").css('font-size', '1.5em');
            }
            return tileSize;
        }
        catch (err) {
            this.logError(err);
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
            this.logError(err);
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
            this.logError(err);
        }
    }

    // handle tile clicks
    handleTileClick(row) {
        this.signalNativeApp('tileClick', row);
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
                        this.logError("Unknown message from app, " + msg);
                        break;
                    }
            }
        }
        catch (err) {
            this.logError(err);
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
            this.logError(err);
        }
    }

    // handle high score dialog click
    saveHighScoreClick() {
        try {
            var name = $('#hightscore-name').val();
            tiles.signalNativeApp('hightscoreName', name);
        }
        catch (err) {
            this.logError(err);
        }
    }

}
// global object
var tiles = new Tiles();

