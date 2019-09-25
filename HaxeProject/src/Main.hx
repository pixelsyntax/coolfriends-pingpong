package;

import h2d.Object;
import h2d.Bitmap;
import hxd.Event.EventKind;
import h2d.Graphics;
import hxd.Key;
import h2d.Text;
import hxd.Timer;
#if js
import js.html.CanvasElement;
import js.Browser;
#end

class Main extends hxd.App {
	public static final VERSION = "v0.0.1";
	//
	public static var instance(default, null):Main;
	//
#if debug
	public var debugTxt(default, null):Text;
	public var debugGfx(default, null):Graphics;
	var layerDebug:Object;
#end
#if js
    var canvas:CanvasElement;
#end
	var ring:Bitmap;

	//

	override function init() {
#if js
  	 	canvas = cast Browser.document.getElementById("webgl");
        Browser.document.ondrag = e -> { e.preventDefault(); }
#end

		s2d.defaultSmooth = true;
		hxd.Window.getInstance().propagateKeyEvents = true;
		hxd.Window.getInstance().addEventTarget(onEvent);
		engine.backgroundColor = 0x112233;
		engine.autoResize = true;

		// create the game
		
		ring = new Bitmap(Layout.getTile("ring"), s2d);
		ring.setScale(2.0);
		ring.setPosition(200.0, 200.0);
		
		// debug information

#if debug
		layerDebug = new Object(s2d);
		debugGfx = new Graphics(layerDebug);
		debugTxt = new Text(Layout.getFont(), layerDebug);
		debugTxt.setScale(0.5);
		debugTxt.setPosition(10.0, 10.0);
#end

        onResize();
	}

    //

	override function update(dt:Float) {
#if js
        if (canvas != null) {
            canvas.style.width = js.Browser.window.innerWidth + "px";
            canvas.style.height = js.Browser.window.innerHeight + "px";
        }
#end

	ring.rotate(dt * Math.PI);

#if debug
		debugGfx.clear();
		debugTxt.text = "PINGPONG " + Main.VERSION + " | " + s2d.width + "x" + s2d.height + " | " + engine.drawCalls + " | "
			+ " Scale:" + Helpers.floatToStringPrecision(Layout.SCALE, 2) + " | " + Timer.frameCount + " | " + Helpers.floatToStringPrecision(engine.fps, 1);
#end
	}

	//

	//

	function onEvent(event:hxd.Event):Void {
		if (event.kind == EventKind.EKeyDown) {
			if (event.keyCode >= Key.F1 && event.keyCode <= Key.F12) { return; } // don't block F keys
			if (event.keyCode >= Key.NUMBER_0 && event.keyCode <= Key.NUMBER_9) { return; } // don't block numbers
		}
	}

	override function onResize():Void {
		// automatic resize to fit to virtual resolution
		var factor = (s2d.height / Layout.RESOLUTION.y);
		Layout.SCALE = 1.0 * factor;
		if (s2d.width / factor < Layout.RESOLUTION.x) { Layout.SCALE *= (s2d.width / factor) / Layout.RESOLUTION.x; }
		s2d.setScale(Layout.SCALE);

#if debug
		if (debugGfx != null) { debugGfx.setScale(1.0 / Layout.SCALE); }
#end
    }

	// 

	static function main() {
		hxd.Res.initEmbed();
		instance = new Main();
	}
}