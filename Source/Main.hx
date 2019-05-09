package;

import openfl.events.MouseEvent;
import openfl.events.Event;
import openfl.utils.Assets;
import openfl.display.Bitmap;
import openfl.display.Sprite;
import haxe.Timer;

class Main extends Sprite
{
	var bitmap : Bitmap;
	var ringSprites:Array<{ spr:Sprite, isDrag:Bool }>;
	var dir = 1;
	var lastTime:Float = 0.0;

	//

	public function new()
	{
		super();

		// please remove all this, i was just testing stuff
		bitmap = new Bitmap( Assets.getBitmapData("assets/openfl.png") );
		addChild( bitmap );

		ringSprites = new Array<{ spr:Sprite, isDrag:Bool }>();
		var ringBitmapData = Assets.getBitmapData("assets/ring.png");
		for ( i in 0...25 ){
			var ringBitmap = new Bitmap(ringBitmapData);
			var ringSprite = new Sprite();
			var info = { spr:ringSprite, isDrag:false };
			ringSprite.addChild(ringBitmap);
			ringSprite.buttonMode = true;
			ringSprite.addEventListener(MouseEvent.MOUSE_DOWN, 	(e) -> { info.isDrag = true;  info.spr.startDrag(); });
			ringSprite.addEventListener(MouseEvent.MOUSE_UP, 	(e) -> { info.isDrag = false; info.spr.stopDrag(); });
			ringSprite.x = stage.stageWidth * 0.5;
			ringSprite.y = stage.stageHeight * 0.5;
			ringSprites.push( info );
			addChild(ringSprite);
		}
		
		lastTime = Timer.stamp();
		addEventListener(Event.ENTER_FRAME, test);
	}

	static function moveTowards(cX:Float, cY:Float, tX:Float, tY:Float, maxDistDelta:Float) {
		var aX = tX - cX;
		var aY = tY - cY;
		var magnitude = Math.sqrt((aX * aX) + (aY * aY));
		if (magnitude <= maxDistDelta || magnitude == 0.0) { return { x:tX, y:tY }; }
		return { x:cX + aX / magnitude * maxDistDelta, y:cY + aY / magnitude * maxDistDelta };
	}

	function test(event:Event) {
		var t = Timer.stamp();
		var dt = t - lastTime;
		lastTime = t;

		var i = 0;
		var tau = Math.PI * 2;
		var tauByLength = tau / ringSprites.length;
		var radiusX = 100 + Math.sin( t ) * 50;
		var radiusY = 100 + Math.sin( t * 1.2 ) * 50;
		for ( info in ringSprites ){
			if ( !info.isDrag ) {
				var tX = Math.sin( t/100 + tauByLength * i ) * radiusX + stage.stageWidth/2;
				var tY = Math.cos( t*3 + tauByLength * i ) * radiusY + stage.stageHeight/2;
				var xy = moveTowards(info.spr.x, info.spr.y, tX, tY, 20.0);
				info.spr.x = xy.x;
				info.spr.y = xy.y;
			}
			++i;
		}
		
		bitmap.x += dir * 400.0 * dt; // trying to make stutter less noticable by using delta time
		if ( bitmap.x + bitmap.width > stage.stageWidth )
			dir = -1;
		if ( bitmap.x < 0 )
			dir = 1;
	}
}