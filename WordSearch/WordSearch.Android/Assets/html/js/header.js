// on ready handler
$(document).ready(function () {    
    header.signalNativeApp("LoadWordsHeader");   
});

class Header extends Signal {
    constructor() {
        super();
        this._restartAnim = null;
    }   

    // create word list table
    makeTable(container, items) {
        try {
            var count = items.length;
            var table = $("<table/>");
            if (count === 4)
                table.addClass('headerTable');
            else 
                table.addClass('headerTable2');
            var row = $("<tr/>");
            var addedCount = 0;
            $.each(items, function (rowCount, item) {
                var word = item.Text;
                var wordDiv = $("<div/>");
                if (item.IsWordCompleted)
                    wordDiv.addClass('wordCompleteDiv');
                else if (item.IsWordHidden)
                    wordDiv.addClass('wordHiddenDiv');
                else
                    wordDiv.addClass('wordDiv');
                wordDiv.text(word);
                var td = $("<td/>");
                td.append(wordDiv);
                row.append(td);
                if (count >= 16)
                    wordDiv.css('font-size', '1em');
                else if (count >= 8)
                    wordDiv.css('font-size', '1em');
                addedCount++;
                // add new table column
                if ((count === 8 && addedCount === 4) || (count >= 16 && addedCount === 8)) {
                    table.append(row);
                    row = $("<tr/>");
                }
            });
            table.append(row);
            return container.html(table);
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
                        // Change word to strike out after 1 second
                        setTimeout(function () {
                            header.signalNativeApp("LoadWordsHeader");
                        }, 1000);
                        // Animate word
                        header.animateWord(data.Word);
                        break;
                    }
                case "OnGameCompleted":
                    {
                        $('#winner').fadeIn(1000);
                        $('#wordsList').fadeOut(500);
                        break;
                    }
                case "SubtractPenaltyScore":
                    {
                        this.animateWord(data);
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

    // Animate completed word
    animateWord(text) {
        try {
            $('#animeWord').text(text);
            $('.ml15').show();
            if (this._restartAnim !== null) {
                this._restartAnim.restart();
            }
            else {
                this._restartAnim = anime.timeline({ loop: false })
                    .add({
                        targets: '.ml15 .word',
                        scale: [14, 1],
                        opacity: [0, 1],
                        easing: "easeOutCirc",
                        duration: 800,
                        delay: function (el, i) {
                            return 800 * i;
                        }
                    }).add({
                        targets: '.ml15',
                        opacity: 0,
                        duration: 1000,
                        easing: "easeOutExpo",
                        delay: 1000
                    });
            }
        }
        catch (err) {
            this.logError(err);
        }
    }
}

// global Header object
var header = new Header();

