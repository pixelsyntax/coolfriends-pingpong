package;

import h3d.col.Bounds;
import h3d.mat.Data.Filter;
import h3d.scene.fwd.DirLight;
import h3d.scene.fwd.PointLight;
import h3d.prim.UV;
import h3d.Vector;
import h3d.col.Point;
import h3d.prim.Quads;
import h3d.scene.Mesh;
import h3d.mat.Material;
import h2d.Object;
import hxd.Event.EventKind;
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
	var layerDebug:Object;
#end
#if js
    var canvas:CanvasElement;
#end
	var light:PointLight;

	var avatar : Mesh;
	var input_movement : Vector;
	var input_deadzone : Float = 0.2;
	//

	override function init() {
		super.init();

#if js
  	 	canvas = cast Browser.document.getElementById("webgl");
        Browser.document.ondrag = e -> { e.preventDefault(); }
		hxd.Window.getInstance().propagateKeyEvents = true;
#end

		s2d.defaultSmooth = true;
		hxd.Window.getInstance().addEventTarget(onEvent);
		engine.backgroundColor = 0x112233;
		engine.autoResize = true;

		input_movement = new Vector( 0, 0, 0, 0 );

		// create the floor

		var floorQuad = new h3d.prim.Quads(
			[ new Point(-0.5, -0.5, 0.0), new Point(0.5, -0.5, 0.0), new Point(-0.5, 0.5, 0.0), new Point(0.5, 0.5, 0.0) ], // points
			[ new UV(0, 0), new UV(1, 0), new UV(0, 1), new UV(1, 1) ], // uvs
			[ new Point(0, 0, 1), new Point(0, 0, 1), new Point(0, 0, 1), new Point(0, 0, 1) ] // normals
		);
		var floorTextures = [ for (i in 1...5) Layout.getTexture("floor" + i) ];
		for (t in floorTextures) { t.filter	= Filter.Nearest; } // make it pixely
		var floorMaterials = [ for (t in floorTextures) Material.create(t) ];
		for (y in 0...5) {
			for (x in 0...5) {
				var mat = Helpers.randomElement(floorMaterials);
				var mesh = new Mesh(floorQuad, mat, s3d);
				mesh.setPosition(x - 2, y - 2, 0.0);
			}
		}

		// create the light		
		light = new PointLight(s3d);
		light.color.setColor(0xccffdd);
		light.params.set(0, 0.25, 0.1);
		//new DirLight(new h3d.Vector(0.5, 0.5, -0.5), s3d);
		s3d.lightSystem.ambientLight.set(0.3, 0.3, 0.3);

		//Player avatar
		var cube = h3d.prim.Cube.defaultUnitCube();
		cube.addNormals();
		cube.addUVs();
		var mat_avatar = h3d.mat.Material.create( hxd.Res.gfx.ring.toTexture() );
		avatar = new Mesh( cube, mat_avatar, s3d );
		avatar.scale( 0.5 );
		avatar.setPosition( 0, 0, 0.25 );
		

#if debug
		// debug information
		layerDebug = new Object(s2d);
		debugTxt = new Text(Layout.getFont(), layerDebug);
		debugTxt.setScale(0.5);
		debugTxt.setPosition(10.0, 10.0);
#end

		//

        onResize();
	}

    //

	override function update(dt:Float) {
		var time = Timer.frameCount * 0.02;

		s3d.camera.pos.set(Math.cos(time) * 7, Math.sin(time) * 7, 4 + 0.7 * Math.sin(time));
		s3d.camera.target.set(0.0, 0.0, 0.0);
		light.setPosition(Math.sin(time * 1.1) * 1.0, Math.cos(time * 1.11) * 0.5, 2.0);

#if js
		// automatically resize canvas to browser window size
        if (canvas != null) {
            canvas.style.width = js.Browser.window.innerWidth + "px";
            canvas.style.height = js.Browser.window.innerHeight + "px";
        }
#end

#if debug
		debugTxt.text = "PINGPONG " + Main.VERSION + " | " + s2d.width + "x" + s2d.height + " | " + engine.drawCalls + " | "
			+ " Scale:" + Helpers.floatToStringPrecision(Layout.SCALE, 2) + " | " + Timer.frameCount + " | " + Helpers.floatToStringPrecision(engine.fps, 1)
			+ "\nPress C to switch between ortho and perspective cam";
#end

		if (Key.isPressed(Key.C)) {
			var aspect = s2d.width / s2d.height;
			s3d.camera.orthoBounds = s3d.camera.orthoBounds != null ? null : Bounds.fromValues(-2.5 * aspect, -2.5, 0, 5 * aspect, 5, 80);
		}

		//Move player
		if ( input_movement.length() > input_deadzone ){
			var movement : Vector = input_movement.clone();
			movement.scale3( dt );
			avatar.setPosition( avatar.x + movement.x, avatar.y + movement.y, avatar.z + movement.z );
			//Point player towards direction of movement
			avatar.setDirection( movement );
		}

		
	}

	//

	//

	function onEvent(event:hxd.Event):Void {
		if (event.kind == EventKind.EKeyDown) {
			if (event.keyCode >= Key.F1 && event.keyCode <= Key.F12) { return; } // don't block F keys
			if (event.keyCode >= Key.NUMBER_0 && event.keyCode <= Key.NUMBER_9) { return; } // don't block numbers
			switch( event.keyCode ){
				case Key.W | Key.UP:
					input_movement.y = -1;
				case Key.S | Key.DOWN:
					input_movement.y = 1;
				case Key.A | Key.LEFT:
					input_movement.x = -1;
				case Key.D | Key.RIGHT:
					input_movement.x = 1;
			}
		} else if ( event.kind == EventKind.EKeyUp ){
			switch( event.keyCode ){
				case Key.W | Key.UP | Key.S | Key.DOWN:
					input_movement.y = 0;
				case Key.A | Key.LEFT | Key.D | Key.RIGHT:
					input_movement.x = 0;
			}
		}
	}

	override function onResize():Void {
		// automatic resize content to fit to virtual resolution
		var factor = (s2d.height / Layout.RESOLUTION.y);
		Layout.SCALE = 1.0 * factor;
		if (s2d.width / factor < Layout.RESOLUTION.x) { Layout.SCALE *= (s2d.width / factor) / Layout.RESOLUTION.x; }
		s2d.setScale(Layout.SCALE);
    }

	// 

	static function main() {
		hxd.Res.initEmbed();
		instance = new Main();
	}
}