$(document).ready(function() {    
    header.signalOnReady();
});

class Header {
    constructor() {
    }

    signalOnReady() {
        try {
            this.waitForHeaderCallbackCreation().then(function () {
                headerJSCallback("OnReady");
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
                headerJSCallback("ERROR: " + err);
            });
        }
        catch (err) {
            console.error(err);
        }
    }
}

var header = new Header();