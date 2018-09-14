// Class used to receive messages from C# code.
class Signal {
    constructor() {
        this.waitForCallbackCreation().then(function () {
            console.log('Native object loaded');
        }).catch(function (error) {
            console.log('Native object timed out');
        });
    }

    // use promise to wait for C# object to be created
    waitForCallbackCreation() {
        try {
            return new Promise(function (resolve, reject) {
                var timerId = setTimeout(function () {
                    return reject(new Error("waitForCallbackCreation timed out."));
                }, 5000);
                (function waitForCallback() {
                    try {
                        if (window.invokeCSharpAction || window.jsBridge) {
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
            this.logError(err);
        }
    }

    // handle errors
    logError(err) {      
        this.signalNativeApp("Error", err.message);
        console.log(err);
    }

    // log message to native app
    logInfo(info) {       
        this.signalNativeApp("LogMsg", info);
        console.log(info);
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
                if (window.invokeCSharpAction)
                    invokeCSharpAction(json);
                else if (window.jsBridge)
                    jsBridge.invokeAction(json);
            }).catch(function (error) {
                console.log(error);
            });
        }
        catch (err) {
            console.log(err);
        }
    }
}

