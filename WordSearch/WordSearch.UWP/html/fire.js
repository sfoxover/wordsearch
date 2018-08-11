// on ready handler
$(document).ready(function () {
});

class Fire {

    constructor() {
        this.CANVAS_NAME = "FireworksCanvas";
        this._paused = false;
    }

    start(wordPos, wordTotal) {
        var canvas = $("#" + this.CANVAS_NAME);
        // highlight just word        
        var per = 100 / wordTotal;
        if (wordTotal > 8) {
            per = 100 / 8;            
            if (wordPos > 8) {
                canvas.css("top", '75%');
                canvas.css("bottom", '100%');
                canvas.css("left", per * (wordPos - 8) + '%');
            }
            else {
                canvas.css("top", '50%');
                canvas.css("bottom", '75%');
                canvas.css("left", per * wordPos + '%');
            }
            canvas.css("width", per + '%');            
        }
        else {
            
            canvas.css("top", '50px');
            canvas.css("left", per * wordPos + '%');
            canvas.css("width", per + '%');
            canvas.css("bottom", '100%');
        }
        canvas.show();

        if (this._paused) {
            this.resume();
        }
        else {
            this._canvas = document.getElementById(this.CANVAS_NAME);
            this._canvas.width = window.innerWidth;
            this._canvas.height = window.innerHeight;
            this._context = this._canvas.getContext('2d');
            this._context.globalCompositeOperation = "lighter";
            this.loadImage();
        }
    }

    loadImage() {
        var image = new Image();
        image.onload = function (e) {
            fire.createProton(e.target);
            fire.tick();
        };
        image.src = 'fire.png';
    }

    createProton(image) {
        this._proton = new Proton;
        this._emitter = new Proton.Emitter();
        this._emitter.rate = new Proton.Rate(new Proton.Span(5, 13), .1);

        this._emitter.addInitialize(new Proton.Mass(1));
        this._emitter.addInitialize(new Proton.Body(image));
        this._emitter.addInitialize(new Proton.P(new Proton.CircleZone(this._canvas.width / 2, this._canvas.height, 10)));
        this._emitter.addInitialize(new Proton.Life(5, 7));
        this._emitter.addInitialize(new Proton.V(new Proton.Span(2, 3), new Proton.Span(0, 30, true), 'polar'));

        this._emitter.addBehaviour(new Proton.Scale(1, .2));
        this._emitter.addBehaviour(new Proton.Alpha(1, .2));
        this._emitter.emit();
        this._proton.addEmitter(this._emitter);

        this._renderer = new Proton.CanvasRenderer(this._canvas);
        this._proton.addRenderer(this._renderer);
    }

    tick() {
        if (!fire._paused) {
            requestAnimationFrame(fire.tick);
            fire._proton.update();
        }
    }

    pause() {
        this._paused = true;
        $("#" + this.CANVAS_NAME).hide();
    }

    resume() {
        this._paused = false;
        $("#" + this.CANVAS_NAME).show();
        this.tick();
    }
}

// global object
var fire = new Fire();
