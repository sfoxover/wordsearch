// on ready handler
$(document).ready(function () {
});

class Fireworks {

    constructor() {
        this.CANVAS_NAME = "FireworksCanvas";
        this._paused = false;
    }

    start(startPos, endPos) {

        var canvas = $("#" + this.CANVAS_NAME);
        // highlight just word
        canvas.css("left", startPos + '%');
        canvas.css("width", endPos +'%');
        canvas.show();

        if (this._paused) {
            this.resume();
        }
        else {
            this._canvas = document.getElementById(this.CANVAS_NAME);
            this._canvas.width = window.innerWidth;
            this._canvas.height = window.innerHeight;
            this._context = this._canvas.getContext('2d');
            this.createProton();
            this.tick();
        }
    }

    createProton(image) {
        this._proton = new Proton;
        this._emitter = new Proton.Emitter();
        this._emitter.rate = new Proton.Rate(new Proton.Span(1, 3), 1);

        this._emitter.addInitialize(new Proton.Mass(1));
        this._emitter.addInitialize(new Proton.Radius(2, 4));
        this._emitter.addInitialize(new Proton.P(new Proton.LineZone(10, this._canvas.height, this._canvas.width - 10, this._canvas.height)));
        this._emitter.addInitialize(new Proton.Life(1, 1.5));
        this._emitter.addInitialize(new Proton.V(new Proton.Span(4, 6), new Proton.Span(0, 0, true), 'polar'));

        this._emitter.addBehaviour(new Proton.Gravity(1));
        this._emitter.addBehaviour(new Proton.Color('#ff0000', 'random'));
        this._emitter.emit();
        this._proton.addEmitter(this._emitter);

        this._renderer = new Proton.CanvasRenderer(this._canvas);
        this._renderer.onProtonUpdate = function () {
            fireworks._context.fillStyle = "rgba(0, 0, 0, 0.1)";
            fireworks._context.fillRect(0, 0, fireworks._canvas.width, fireworks._canvas.height);
        };
        this._proton.addRenderer(this._renderer);

        ////NOTICE :you can only use two emitters do this effect.In this demo I use more emitters want to test the emtter's life
        this._proton.addEventListener(Proton.PARTICLE_DEAD, function (particle) {
            if (Math.random() < .7)
                fireworks.createFirstEmitter(particle);
            else
                fireworks.createSecondEmitter(particle);
        });
    }

    createFirstEmitter(particle) {
        var subemitter = new Proton.Emitter();
        subemitter.rate = new Proton.Rate(new Proton.Span(250, 300), 1);
        subemitter.addInitialize(new Proton.Mass(1));
        subemitter.addInitialize(new Proton.Radius(1, 2));
        subemitter.addInitialize(new Proton.Life(1, 3));
        subemitter.addInitialize(new Proton.V(new Proton.Span(2, 4), new Proton.Span(0, 360), 'polar'));

        subemitter.addBehaviour(new Proton.RandomDrift(10, 10, .05));
        subemitter.addBehaviour(new Proton.Alpha(1, 0));
        subemitter.addBehaviour(new Proton.Gravity(3));
        var color = Math.random() > .3 ? Proton.MathUtils.randomColor() : 'random';
        subemitter.addBehaviour(new Proton.Color(color));

        subemitter.p.x = particle.p.x;
        subemitter.p.y = particle.p.y;
        subemitter.emit('once', true);
        this._proton.addEmitter(subemitter);
    }

    createSecondEmitter(particle) {
        var subemitter = new Proton.Emitter();
        subemitter.rate = new Proton.Rate(new Proton.Span(100, 120), 1);

        subemitter.addInitialize(new Proton.Mass(1));
        subemitter.addInitialize(new Proton.Radius(4, 8));
        subemitter.addInitialize(new Proton.Life(1, 2));
        subemitter.addInitialize(new Proton.V([1, 2], new Proton.Span(0, 360), 'polar'));

        subemitter.addBehaviour(new Proton.Alpha(1, 0));
        subemitter.addBehaviour(new Proton.Scale(1, .1));
        subemitter.addBehaviour(new Proton.Gravity(1));
        var color = Proton.MathUtils.randomColor();
        subemitter.addBehaviour(new Proton.Color(color));

        subemitter.p.x = particle.p.x;
        subemitter.p.y = particle.p.y;
        subemitter.emit('once', true);
        this._proton.addEmitter(subemitter);
    }

    tick() {
        if (!fireworks._paused) {
            requestAnimationFrame(fireworks.tick);
            fireworks._proton.update();
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
var fireworks = new Fireworks();
