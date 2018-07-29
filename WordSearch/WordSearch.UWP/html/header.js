// on ready handler
$(document).ready(function () {    
    header.signalNativeApp("OnReady");
});

class Header {
    constructor() {
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
            });
        }
        catch (err) {
            this.handleError(err);
        }
    }

    // use promise to wait for C# headerJSCallback object to be created
    waitForHeaderCallbackCreation() {
        try {
            return new Promise(function (resolve, reject) {
                (function waitForHeaderCallback() {
                    if (headerJSCallback)
                        return resolve();
                    setTimeout(waitForHeaderCallback, 30);
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
                msgObj.Data = err.Message;
                var json = JSON.stringify(msgObj);
                // call native code
                headerJSCallback(json);
            });
        }
        catch (err) {
            console.error(err);
        }
    }

    // handle message from native app
    handleMsgFromApp(json) {
        try {
            var msgObj = JSON.parse(json);
            var msg = msgObj.Message;
            var data = msgObj.Data;
        }
        catch (err) {
            this.handleError(err);
        }
    }

}
// global Header object
var header = new Header();